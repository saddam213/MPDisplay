using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GUISkinFramework;
using GUISkinFramework.ExtensionMethods;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Styles;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common;
using SkinEditor.Dialogs;
using MPDisplay.Common.Controls;
using Common.Helpers;
using MPDisplay.Common.ExtensionMethods;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for StyleEditorView.xaml
    /// </summary>
    public partial class StyleEditorView : EditorViewModel
    {
        private string _selectedStyle = "Default";
     
        public StyleEditorView()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Style Editor"; }
        }

        public override void Initialize()
        {
            NotifyPropertyChanged("Styles");
            SelectedStyle = SkinInfo.CurrentStyle;
        }
     

        public StyleEditorViewSettings Settings
        {
            get { return EditorSettings as StyleEditorViewSettings; }
        }

        public IEnumerable<string> Styles
        {
            get { return SkinInfo.Styles.Select(x => x.Key); }
        }

        public string SelectedStyle
        {
            get { return _selectedStyle; }
            set
            { 
                _selectedStyle = value;
                NotifyPropertyChanged("SelectedStyle");
                NotifyPropertyChanged("CurrentBrushStyles");
                NotifyPropertyChanged("CurrentControlStyles");
                NotifyPropertyChanged("CurrentBrushType");
            }
        }

        public Type CurrentBrushType 
        {
            get { return SelectedBrushStyle != null ? SelectedBrushStyle.GetType() : null; }
        }

        #region Brush Styles

        public IEnumerable<Control> BrushContextMenuItems
        {
            get
            {
                var addMenu = CreateContextMenuItem("Add", "Add", null);
                addMenu.Items.Add(CreateContextMenuItem("ColorBrush", "", () => AddBrush<XmlColorBrush>()));
                addMenu.Items.Add(CreateContextMenuItem("GradientBrush", "", () => AddBrush<XmlGradientBrush>()));
                addMenu.Items.Add(CreateContextMenuItem("ImageBrush", "XmlImage", () => AddBrush<XmlImageBrush>()));
                yield return addMenu;
                yield return new Separator { Name = "Add" };
                yield return CreateContextMenuItem("Copy", "Copy", () => CopyBrushStyle());
                yield return CreateContextMenuItem("Rename", "", () => RenameBrushStyle());
                yield return CreateContextMenuItem("Delete", "Delete", () => DeleteBrushStyle());
            }
        }

        private void DeleteBrushStyle()
        {
            if (SelectedBrushStyle != null)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to delete ControlStyle: {0} ?", SelectedBrushStyle.StyleId), "Delete Style", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var style in SkinInfo.Styles.Values)
                    {
                        style.BrushStyles.Remove(style.BrushStyles.FirstOrDefault(s => s.StyleId.Equals(SelectedBrushStyle.StyleId)));
                    }
                    SelectedBrushStyle = null;
                }
            }
        }

        private void RenameBrushStyle()
        {
            if (SelectedBrushStyle != null)
            {
                string newStyle = TextBoxDialog.ShowDialog("Rename Style", "New StyleId", SelectedBrushStyle.StyleId);
                if (!string.IsNullOrEmpty(newStyle))
                {
                    if (SkinInfo.Styles[SelectedStyle].BrushStyles.Any(b => b.StyleId.Equals(newStyle)))
                    {
                        MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    foreach (var styleCollection in SkinInfo.Styles.Values)
                    {
                        var style = styleCollection.BrushStyles.FirstOrDefault(s => s.StyleId.Equals(SelectedBrushStyle.StyleId));
                        if (style != null)
                        {
                            style.StyleId = newStyle;
                        }
                    }

                    SelectedBrushStyle.StyleId = newStyle;
                }
            }
        }

        private void CopyBrushStyle()
        {
            if (SelectedBrushStyle != null)
            {
                string newStyle = TextBoxDialog.ShowDialog("Copy Style", "New StyleId", SelectedBrushStyle.StyleId);
                if (!string.IsNullOrEmpty(newStyle))
                {
                    if (SkinInfo.Styles[SelectedStyle].BrushStyles.Any(b => b.StyleId.Equals(newStyle)))
                    {
                        MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    var newControl = SelectedBrushStyle.CreateCopy();
                    newControl.StyleId = newStyle;
                    foreach (var style in SkinInfo.Styles.Values)
                    {
                        style.BrushStyles.Add(newControl);
                    }
                }
            }
        }


        public ObservableCollection<XmlBrush> CurrentBrushStyles
        {
            get
            {
                if (SkinInfo.Styles.Any())
                {
                    if (!string.IsNullOrEmpty(SelectedStyle) && SkinInfo.Styles.ContainsKey(SelectedStyle))
                    {
                        return SkinInfo.Styles[SelectedStyle].BrushStyles;
                    }
                    return SkinInfo.Styles["Default"].BrushStyles;
                }
                return null; 
            }
        }

        private XmlBrush _selectedBrushStyle;

        public XmlBrush SelectedBrushStyle
        {
            get { return _selectedBrushStyle; }
            set
            {
                _selectedBrushStyle = value;
                NotifyPropertyChanged("SelectedBrushStyle");
                NotifyPropertyChanged("CurrentBrushType");
            }
        }




        private void ColorPickerPanel_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
           NotifyPropertyChanged("SelectedBrushStyle");
        }


        private void GradientBrushEditor_OnGradientBrushChanged_1(XmlGradientBrush brush)
        {
         
           NotifyPropertyChanged("SelectedBrushStyle");
        }


        private void ImageBrushEditor_OnImageBrushChanged(XmlImageBrush imageBrush)
        {
            if (SelectedBrushStyle is XmlImageBrush)
            {
            //   (SelectedBrushStyle as XmlImageBrush).ImageName = imageBrush.ImageName;
            //   (SelectedBrushStyle as XmlImageBrush).ImageStretch = imageBrush.ImageStretch;
                NotifyPropertyChanged("SelectedBrushStyle");
            }
        }

        private void AddBrush<T>() where T : XmlBrush
        {
            var newBrush = Activator.CreateInstance<T>();
            string newStyle = TextBoxDialog.ShowDialog(string.Format("New {0}", newBrush.StyleType), string.Format("{0} StyleId", newBrush.StyleType));
            if (!string.IsNullOrEmpty(newStyle))
            {
                if (SkinInfo.Styles[SelectedStyle].BrushStyles.Any(b => b.StyleId.Equals(newStyle)))
                {
                    MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                newBrush.StyleId = newStyle;
                SkinInfo.Styles[SelectedStyle].BrushStyles.Add(newBrush);
                SelectedBrushStyle = newBrush;
            }
        }

        // HACK: to indicate value changed, but I should fix the brush editors to only report on change
        private void BrushTypeEditGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            HasPendingChanges = true;
        }

        #endregion

        #region Control Styles

        private bool _showInnerControls;
        private bool _showControlResize;
        private XmlControlStyle _selectedControlStyle;
        private XmlControl _controlHost;

        /// <summary>
        /// Gets the current control styles.
        /// </summary>
        public ObservableCollection<XmlControlStyle> CurrentControlStyles
        {
            get
            {
                if (SkinInfo.Styles.Any())
                {
                    if (!string.IsNullOrEmpty(SelectedStyle) && SkinInfo.Styles.ContainsKey(SelectedStyle))
                    {
                        return SkinInfo.Styles[SelectedStyle].ControlStyles;
                    }
                    return SkinInfo.Styles["Default"].ControlStyles;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the selected control style.
        /// </summary>
        public XmlControlStyle SelectedControlStyle
        {
            get { return _selectedControlStyle; }
            set 
            {
                SaveControlStyleSizeFromSettings(_selectedControlStyle);
                _selectedControlStyle = value;
                NotifyPropertyChanged("SelectedControlStyle");
                OnSelectedControlStyleChanged();
            }
        }

        /// <summary>
        /// Gets or sets the control host.
        /// </summary>
        public XmlControl ControlHost
        {
            get { return _controlHost; }
            set { _controlHost = value; NotifyPropertyChanged("ControlHost"); SetInnerControlColors(_showInnerControls); }
        }

        /// <summary>
        /// Gets or sets the current control.
        /// </summary>
        //public GUIControl CurrentControl
        //{
        //    get { return _currentControl; }
        //    set
        //    {
        //        _currentControl = value;
        //        NotifyPropertyChanged("CurrentControl");
        //        SetInnerControlColors(_showInnerControls);
        //    }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether [show control resize].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show control resize]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowControlResize
        {
            get { return _showControlResize; }
            set { _showControlResize = value; NotifyPropertyChanged("ShowControlResize"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show inner controls].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show inner controls]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowInnerControls
        {
            get { return _showInnerControls; }
            set
            {
                _showInnerControls = value;
                SetInnerControlColors(_showInnerControls);
                NotifyPropertyChanged("ShowInnerControls");
            }
        }

        /// <summary>
        /// Called when [selected control style changed].
        /// </summary>
        public void OnSelectedControlStyleChanged()
        {
            ShowControlResize = true;
            if (_selectedControlStyle is XmlButtonStyle)
            {
                ControlHost = new XmlButton
                {
                    ControlStyle = _selectedControlStyle as XmlButtonStyle,
                    LabelText = "MPDisplay",
                    Image = "/SkinEditor;component/Images/MPLogo.png"
                };
            }

            if (_selectedControlStyle is XmlListItemStyle)
            {
                ShowControlResize = false;
                ControlHost = new XmlListItem
                {
                    Height = (_selectedControlStyle as XmlListItemStyle).Height,
                    Width = (_selectedControlStyle as XmlListItemStyle).Width,
                    ControlStyle = _selectedControlStyle as XmlListItemStyle,
                    LabelText = "Avatar",
                    Label2Text = "Label2",
                    Label3Text = "Label3",
                    Image = "/SkinEditor;component/Images/DVDCover.jpg"
                };
            }

            if (_selectedControlStyle is XmlLabelStyle)
            {
                ControlHost = new XmlLabel
                {
                    ControlStyle = _selectedControlStyle as XmlLabelStyle,
                    LabelText = "MPDisplay"
                };
            }

            if (_selectedControlStyle is XmlImageStyle)
            {
                ControlHost = new XmlImage
                {
                    ControlStyle = _selectedControlStyle as XmlImageStyle,
                    Image = "/SkinEditor;component/Images/MPLogo.png"
                };
            }

            if (_selectedControlStyle is XmlGroupStyle)
            {
                ControlHost = new XmlGroup
                {
                    ControlStyle = _selectedControlStyle as XmlGroupStyle,
                    Controls = new ObservableCollection<XmlControl>()
                };
            }

            if (_selectedControlStyle is XmlListStyle)
            {
                ControlHost = new XmlList
                {
                    ControlStyle = _selectedControlStyle as XmlListStyle,
                };
            }

            if (_selectedControlStyle is XmlGuideStyle)
            {
                ControlHost = new XmlGuide
                {
                    ControlStyle = _selectedControlStyle as XmlGuideStyle,
                };
            }

            if (_selectedControlStyle is XmlProgressBarStyle)
            {
                ControlHost = new XmlProgressBar
                {
                    ControlStyle = _selectedControlStyle as XmlProgressBarStyle,
                    Width = 500,
                    Height = 70,
                    ProgressValue = "60"
                };
            }
            SetControlStyleSizeFromSettings(_selectedControlStyle);
        }

        /// <summary>
        /// Handles the ControlStyleValueChanged event of the PropertyGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyValueChangedEventArgs"/> instance containing the event data.</param>
        private void PropertyGrid_ControlStyleValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            var property = e.OriginalSource as PropertyItem;
            if (property != null)
            {
                HasPendingChanges = true;
                if (_selectedControlStyle is XmlListItemStyle)
                {
                    if (property.Name == "Width" || property.Name == "Height")
                    {
                        if (ControlHost != null)
                        {
                            ControlHost.Width = (_selectedControlStyle as XmlListItemStyle).Width;
                            ControlHost.Height = (_selectedControlStyle as XmlListItemStyle).Height;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the inner control colors.
        /// </summary>
        /// <param name="showColors">if set to <c>true</c> [show colors].</param>
        private void SetInnerControlColors(bool showColors)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                if (_selectedControlStyle is XmlButtonStyle || _selectedControlStyle is XmlListItemStyle || _selectedControlStyle is XmlImageStyle)
                {
                    if (controlSurface != null)
                    {
                        if (_selectedControlStyle is XmlListItemStyle)
                        {
                            var text = controlSurface.FindVisualChildren<TextBlock>();
                            if (text != null && text.Any())
                            {
                                int count = 0;
                                var colors = new Color[] { Colors.Purple, Colors.LightBlue, Colors.LimeGreen, Colors.Moccasin, Colors.Tan, Colors.SteelBlue };
                                foreach (var item in text)
                                {
                                    item.Background = new SolidColorBrush(showColors ? colors[count] : Colors.Transparent);
                                    count++;
                                }
                            }
                        }
                        else
                        {
                            var text = controlSurface.FindVisualChildren<TextBlock>().FirstOrDefault();
                            if (text != null)
                            {
                                text.Background = new SolidColorBrush(showColors ? Colors.Orange : Colors.Transparent);
                            }
                        }

                        var image = controlSurface.FindVisualChildren<RoundedImage>().FirstOrDefault();
                        if (image != null && image.Parent is Border)
                        {
                            (image.Parent as Border).Background = new SolidColorBrush(showColors ? Colors.Red : Colors.Transparent);
                        }
                    }
                }
            }, DispatcherPriority.Background);
        }

        /// <summary>
        /// Sets the control style size from settings.
        /// </summary>
        /// <param name="style">The style.</param>
        private void SetControlStyleSizeFromSettings(XmlControlStyle style)
        {
            if (style != null && !(_selectedControlStyle is XmlListItemStyle))
            {
                var setting = Settings.StyleItemSettings.FirstOrDefault(s => s.SkinName.Equals(SkinInfo.SkinName) && s.StyleId.Equals(style.StyleId))
                     ?? CreateNewControlStyleSetting(style);

                if (ControlHost != null)
                {
                    ControlHost.Width = setting.Width;
                    ControlHost.Height = setting.Height;
                }
            }
        }

        /// <summary>
        /// Creates the new control style setting.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        private DesignerStyleSetting CreateNewControlStyleSetting(XmlControlStyle style)
        {
            var setting = new DesignerStyleSetting
            {
                SkinName = SkinInfo.SkinName,
                StyleId = style.StyleId,
                Width = 400,
                Height = 200
            };
            Settings.StyleItemSettings.Add(setting);
            return setting;
        }

        /// <summary>
        /// Saves the control style size from settings.
        /// </summary>
        /// <param name="style">The style.</param>
        private void SaveControlStyleSizeFromSettings(XmlControlStyle style)
        {
            if (style != null && !(_selectedControlStyle is XmlListItemStyle))
            {
                var setting = Settings.StyleItemSettings.FirstOrDefault(s => s.SkinName.Equals(SkinInfo.SkinName) && s.StyleId.Equals(style.StyleId))
                    ?? CreateNewControlStyleSetting(style);

                if (ControlHost != null)
                {
                    setting.Width = ControlHost.Width;
                    setting.Height = ControlHost.Height;
                }
            }
        }

        #region ControlStyle ContextMenu

        /// <summary>
        /// Gets the control context menu items.
        /// </summary>
        public IEnumerable<Control> ControlContextMenuItems
        {
            get
            {
                var addMenu = CreateContextMenuItem("Add", "Add", null);
                addMenu.Items.Add(CreateContextMenuItem("Label Style", "XmlLabel", () => AddControlStyle<XmlLabelStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("Button Style", "XmlButton", () => AddControlStyle<XmlButtonStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("Image Style", "XmlImage", () => AddControlStyle<XmlImageStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("Group Style", "XmlGroup", () => AddControlStyle<XmlGroupStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("Guide Style", "XmlGuide", () => AddControlStyle<XmlGuideStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("ProgressBar Style", "XmlProgressBar", () => AddControlStyle<XmlProgressBarStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("List Style", "XmlList", () => AddControlStyle<XmlListStyle>()));
                addMenu.Items.Add(CreateContextMenuItem("ListItem Style", "XmlList", () => AddControlStyle<XmlListItemStyle>()));
                yield return addMenu;
                yield return new Separator { Name = "Add" };
                yield return CreateContextMenuItem("Copy", "Copy", () => CopyControlStyle());
                yield return CreateContextMenuItem("Rename", "", () => RenameControlStyle());
                yield return CreateContextMenuItem("Delete", "Delete", () => DeleteControlStyle());
            }
        }

        /// <summary>
        /// Copies the control style.
        /// </summary>
        private void CopyControlStyle()
        {
            if (SelectedControlStyle != null)
            {
                 string newStyle = TextBoxDialog.ShowDialog("Copy Style", "New StyleId", SelectedBrushStyle.StyleId);
                 if (!string.IsNullOrEmpty(newStyle))
                 {
                     if (SkinInfo.Styles[SelectedStyle].BrushStyles.Any(b => b.StyleId.Equals(newStyle)))
                     {
                         MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                         return;
                     }
                     var newControl = SelectedControlStyle.CreateCopy();
                     newControl.StyleId = newStyle;
                     foreach (var style in SkinInfo.Styles.Values)
                     {
                         style.ControlStyles.Add(newControl.CreateCopy());
                     }
                 }
            }
        }

        /// <summary>
        /// Deletes the control style.
        /// </summary>
        private void DeleteControlStyle()
        {
            if (SelectedControlStyle != null)
            {
                if (MessageBox.Show(string.Format("Are you sure you want to delete ControlStyle: {0} ?", SelectedControlStyle.StyleId), "Delete Style", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    foreach (var style in SkinInfo.Styles.Values)
                    {
                        style.ControlStyles.Remove(style.ControlStyles.FirstOrDefault(s => s.StyleId.Equals(SelectedControlStyle.StyleId)));
                    }
                    SelectedControlStyle = null;
                }
            }
        }

        /// <summary>
        /// Renames the control style.
        /// </summary>
        private void RenameControlStyle()
        {
            if (SelectedControlStyle != null)
            {
                string newStyle = TextBoxDialog.ShowDialog("Rename Style", "New StyleId", SelectedControlStyle.StyleId);
                if (!string.IsNullOrEmpty(newStyle))
                {
                    if (SkinInfo.Styles[SelectedStyle].ControlStyles.Any(b => b.StyleId.Equals(newStyle)))
                    {
                        MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    foreach (var styleCollection in SkinInfo.Styles.Values)
                    {
                        var style = styleCollection.ControlStyles.FirstOrDefault(s => s.StyleId.Equals(SelectedControlStyle.StyleId));
                        if (style != null)
                        {
                            style.StyleId = newStyle;
                        }
                    }

                    SelectedControlStyle.StyleId = newStyle;
                }
            }
        }

        /// <summary>
        /// Adds the control style.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void AddControlStyle<T>() where T : XmlControlStyle
        {
            try
            {
                var newControl = Activator.CreateInstance<T>();
                string newStyle = TextBoxDialog.ShowDialog(string.Format("New {0}", newControl.StyleType), string.Format("{0} StyleId", newControl.StyleType));
                if (!string.IsNullOrEmpty(newStyle))
                {
                    if (SkinInfo.Styles[SelectedStyle].ControlStyles.Any(b => b.StyleId.Equals(newStyle)))
                    {
                        MessageBox.Show(string.Format("StyleId {0} already exists, please select a new StyleId", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    newControl.StyleId = newStyle;
                    foreach (var style in SkinInfo.Styles.Values)
                    {
                        style.ControlStyles.Add(newControl.CreateCopy());
                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show(string.Format("{0}{2}{2}{1}", ex.Message, ex.StackTrace, Environment.NewLine), "ERROR!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        #endregion

        #endregion









        private void Button_NewStyle_Click(object sender, RoutedEventArgs e)
        {
            string newStyle = TextBoxDialog.ShowDialog("New Style", "Style Name");
            if (!string.IsNullOrEmpty(newStyle))
            {
                if (SkinInfo.Styles.ContainsKey(newStyle))
                {
                    MessageBox.Show(string.Format("Style name '{0}' already exists, please select a new Style name", newStyle), "Style Exists", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                SkinInfo.Styles.Add(newStyle, SkinInfo.Styles["Default"].CreateCopy());
                NotifyPropertyChanged("Styles");
                SelectedStyle = newStyle;
            }
        }

        private void Button_DeleteStyle_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format("Are you sure you want to delete {0} style", SelectedStyle), "Delete Style", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                FileHelpers.TryDelete(SkinInfo.SkinStyleFolder + SelectedStyle + ".xml");
                SkinInfo.Styles.Remove(SelectedStyle);
               
                NotifyPropertyChanged("Styles");
                SelectedStyle = "Default";
            }
        }

        private void Button_SaveStyle_Click(object sender, RoutedEventArgs e)
        {
            SkinInfo.SaveStyles();
            HasPendingChanges = false;
        }


        /// <summary>
        /// Creates a context menu item.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        private MenuItem CreateContextMenuItem(string header, string icon, Action handler)
        {
            var menuItem = new MenuItem();
            menuItem.Header = header;
            menuItem.Icon = new Image { Margin = new Thickness(2), Stretch = System.Windows.Media.Stretch.Uniform, Width = 16, Height = 16, Source = new BitmapImage(new Uri(string.Format(@"/Images/{0}.png", icon), UriKind.RelativeOrAbsolute)) };
            if (handler != null)
            {
                menuItem.Click += (s, e) => handler();
            }
            return menuItem;
        }


        public override void OnModelOpen()
        {
           // SelectedStyle = SkinInfo.CurrentStyle;
        }

        public override void OnModelClose()
        {
            SaveControlStyleSizeFromSettings(_selectedControlStyle);
        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedControlStyleChanged();
        }

     

        private void Grid_PreviewMouseUp_1(object sender, MouseButtonEventArgs e)
        {

        }






       
    }


  
  
}
