using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Windows;
using MPDisplay.Common.Controls;
using MPDisplay.Common.Controls.PropertyGrid;
using SkinEditor;
using SkinEditor.Dialogs;
using MPDisplay.Common.ExtensionMethods;
using MPDisplay.Common.Utils;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for SkinEditorView.xaml
    /// </summary>
    public partial class SkinEditorView : EditorViewModel
    {
        #region Variables

        private string _selectedStyle;
        private XmlWindow _currentXmlWindow;
        private XmlDialog _currentXmlDialog;
        private object _selectedTreeItem;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinEditorView"/> class.
        /// </summary>
        public SkinEditorView()           
        {
            InitializeComponent();
            CreateContextMenuCommands();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public SkinEditorViewSettings Settings
        {
            get { return EditorSettings as SkinEditorViewSettings; }
        }

        /// <summary>
        /// Gets all Windows and Dialogs.
        /// </summary>
        public IEnumerable<IXmlControlHost> AllWindows
        {
            get { return Windows.Cast<IXmlControlHost>().Concat(Dialogs.Cast<IXmlControlHost>()); }
        }
        
        /// <summary>
        /// Gets the windows.
        /// </summary>
        public ObservableCollection<XmlWindow> Windows
        {
            get { return SkinInfo.Windows; }
        }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        public ObservableCollection<XmlDialog> Dialogs
        {
            get { return SkinInfo.Dialogs; }
        }

        /// <summary>
        /// Gets or sets the current XML window.
        /// </summary>
        public XmlWindow CurrentXmlWindow
        {
            get { return _currentXmlWindow; }
            set { _currentXmlWindow = value; NotifyPropertyChanged("CurrentXmlWindow"); }
        }

        /// <summary>
        /// Gets or sets the current XML dialog.
        /// </summary>
        public XmlDialog CurrentXmlDialog
        {
            get { return _currentXmlDialog; }
            set { _currentXmlDialog = value; NotifyPropertyChanged("CurrentXmlDialog"); }
        }

        public XmlControl CurrentXmlControl
        {
            get { return (XmlControl)GetValue(CurrentXmlControlProperty); }
            set { SetValue(CurrentXmlControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentXmlControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentXmlControlProperty =
            DependencyProperty.Register("CurrentXmlControl", typeof(XmlControl), typeof(SkinEditorView), new PropertyMetadata(null));
        private string _selectedLanguage;

        /// <summary>
        /// Gets or sets the selected tree item.
        /// </summary>
        public object SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set { _selectedTreeItem = value; NotifyPropertyChanged("SelectedTreeItem"); }
        }

        /// <summary>
        /// Gets the styles.
        /// </summary>
        public IEnumerable<string> Styles
        {
            get { return SkinInfo.Styles.Select(x => x.Key); }
        }

        /// <summary>
        /// Gets or sets the selected style.
        /// </summary>
        public string SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                _selectedStyle = value;
                if (!string.IsNullOrEmpty(_selectedStyle))
                {
                    SkinInfo.SetStyle(_selectedStyle);
                }
                NotifyPropertyChanged("SelectedStyle");
            }
        }

        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                _selectedLanguage = value;
                if (!string.IsNullOrEmpty(_selectedLanguage))
                {
                    SkinInfo.SetLanguage(_selectedLanguage);
                }
                NotifyPropertyChanged("SelectedLanguage");
            }
        }

        #endregion

        #region EditorViewModelBase Overrides

        /// <summary>
        /// Gets the title.
        /// </summary>
        public override string Title
        {
            get { return "Skin Editor"; }
        }

        public override void Initialize()
        {
            NotifyPropertyChanged("Styles");
            NotifyPropertyChanged("Settings");
            NotifyPropertyChanged("AllWindows");
            SelectedStyle = SkinInfo.CurrentStyle;
            SelectedLanguage = SkinInfo.CurrentLanguage;
        }

        public override void OnModelOpen()
        {
            base.OnModelOpen();
            NotifyPropertyChanged("Styles");
            SkinInfo.SetStyle(_selectedStyle);
        }

        public override void OnModelClose()
        {
            base.OnModelClose();
        }

        #endregion

        #region TreeView

        /// <summary>
        /// Trees the view_ selected item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != e.OldValue)
            {
                SelectedTreeItem = e.NewValue;
                if (e.NewValue is XmlWindow)
                {
                    CurrentXmlControl = null;
                    CurrentXmlDialog = null;
                    CurrentXmlWindow = e.NewValue as XmlWindow;
                }
                else if (e.NewValue is XmlDialog)
                {
                    CurrentXmlControl = null;
                    CurrentXmlWindow = null;
                    CurrentXmlDialog = e.NewValue as XmlDialog;
                }
                else if (e.NewValue is XmlControl)
                {
                    CurrentXmlControl = null;
                    var control = e.NewValue as XmlControl;
                    if (control != null)
                    {
                        var parent = GetControlParent(control);
                        if (parent is XmlWindow && (CurrentXmlWindow == null || (parent as XmlWindow).Id != CurrentXmlWindow.Id))
                        {
                            CurrentXmlDialog = null;
                            CurrentXmlWindow = parent as XmlWindow;
                        }
                        if (parent is XmlDialog && (CurrentXmlDialog == null || (parent as XmlDialog).Id != CurrentXmlDialog.Id))
                        {
                            CurrentXmlWindow = null;
                            CurrentXmlDialog = parent as XmlDialog;
                        }
                    }
                    CurrentXmlControl = control;
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the TreeViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ContentPresenter)
            {
                TreeViewItem item = sender as TreeViewItem;
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        private void SelectTreeItem(object item)
        {
            if (item != null)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    var treeItem = skinTree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    if (treeItem != null)
                    {
                        treeItem.IsSelected = true;
                    }
                }, DispatcherPriority.Background);
            }
        }

        #endregion

        public bool IsControlHostSelected
        {
            get { return SelectedTreeItem is IXmlControlHost; }
        }

        private IXmlControlHost GetControlParent(XmlControl control)
        {
            foreach (var parent in AllWindows)
            {
                if (parent.Controls.Contains(control))
                {
                    return parent;
                }
                foreach (var group in parent.Controls.GetControls().OfType<XmlGroup>())
                {
                    if (group.Controls.Contains(control))
                    {
                        return group;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        /// <typeparam name="T">type of control</typeparam>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private XmlControl CreateControl(Type controlType, int id, int windowId)
        {
            var controlStyle = Settings.DesignerStyle.GetDesignerStyle(controlType);

            if (controlType == typeof(XmlButton))
            {
                return new XmlButton
                {
                    Id = id,
                    Name = "NewButton",
                    Width = 200,
                    Height = 100,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlButtonStyle ?? new XmlButtonStyle(),
                    LabelText = "NewButton"
                };
            }

            if (controlType == typeof(XmlGroup))
            {
                return new XmlGroup
                {
                    Id = id,
                    Name = "NewGroup",
                    Width = 200,
                    Height = 200,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlGroupStyle ?? new XmlGroupStyle(),
                    Controls = new ObservableCollection<XmlControl>()
                };
            }

            if (controlType == typeof(XmlRectangle))
            {
                return new XmlRectangle
                {
                    Id = id,
                    Name = "NewRectangle",
                    Width = 200,
                    Height = 200,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    BackgroundBrush = new XmlColorBrush { Color = "Black" }
                };
            }

            if (controlType == typeof(XmlLabel))
            {
                return new XmlLabel
                {
                    Id = id,
                    Name = "NewLabel",
                    Width = 200,
                    Height = 40,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlLabelStyle ?? new XmlLabelStyle(),
                    LabelText = "New Label"
                };
            }

            if (controlType == typeof(XmlImage))
            {
                return new XmlImage
                {
                    Id = id,
                    Name = "NewImage",
                    Width = 200,
                    Height = 200,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlImageStyle ?? new XmlImageStyle()
                };
            }

            if (controlType == typeof(XmlProgressBar))
            {
                return new XmlProgressBar
                {
                    Id = id,
                    Name = "NewProgressBar",
                    Width = 400,
                    Height = 80,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ProgressValue = "60",
                    ControlStyle = controlStyle as XmlProgressBarStyle ?? new XmlProgressBarStyle(),
                };
            }

            if (controlType == typeof(XmlGuide))
            {
                return new XmlGuide
                {
                    Id = id,
                    Name = "NewGuide",
                    Width = 400,
                    Height = 80,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlGuideStyle ?? new XmlGuideStyle(),
                };
            }

            if (controlType == typeof(XmlEqualizer))
            {
                return new XmlEqualizer
                {
                    Id = id,
                    Name = "NewEqualizer",
                    Width = 400,
                    Height = 80,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XMLEqualizerStyle ?? new XMLEqualizerStyle(),
                };
            }

            if (controlType == typeof(XmlList))
            {
                var coverFlowItemStyle = Settings.DesignerStyle.GetDesignerStyle("CoverFlowItemStyle") as XmlListItemStyle;
                var verticalItemStyle = Settings.DesignerStyle.GetDesignerStyle("VerticalItemStyle") as XmlListItemStyle;
                var horizontalItemStyle = Settings.DesignerStyle.GetDesignerStyle("HorizontalItemStyle") as XmlListItemStyle;
                return new XmlList
                {
                    Id = id,
                    Name = "NewList",
                    Width = 700,
                    Height = 500,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlListStyle ?? new XmlListStyle(),
                    CoverFlowItemStyle = coverFlowItemStyle ?? new XmlListItemStyle(),
                    VerticalItemStyle = verticalItemStyle ?? new XmlListItemStyle(),
                    HorizontalItemStyle = horizontalItemStyle ?? new XmlListItemStyle(),
                    ListLayout = XmlListLayout.Auto,
                };
            }
            HasPendingChanges = true;
            return null;
        }



        #region ContextMenu

        public ICommand AddControlCommand { get; internal set; }
        public ICommand CopyControlCommand { get; internal set; }
        public ICommand PasteControlCommand { get; internal set; }
        public ICommand DeleteControlCommand { get; internal set; }
        public ICommand MoveControlCommand { get; internal set; }

        /// <summary>
        /// Populates the context menu.
        /// </summary>
        private void CreateContextMenuCommands()
        {
            AddControlCommand = new RelayCommand(param => AddControl((Type)param), param => CanExecuteAddControlCommand());
            CopyControlCommand = new RelayCommand(param => CopyControl(bool.Parse(param.ToString())), param => CanExecuteCopyControlCommand());
            PasteControlCommand = new RelayCommand(param => PasteControl(), param => CanExecutePasteControlCommand());
            DeleteControlCommand = new RelayCommand(param => DeleteControl(), param => CanExecuteDeleteControlCommand());
            MoveControlCommand = new RelayCommand(param => MoveControl(bool.Parse(param.ToString())), param => CanExecuteMoveControlCommand());
        }

        /// <summary>
        /// Determines whether AddControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if AddControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteAddControlCommand()
        {
            return IsControlHostSelected;
        }

        /// <summary>
        /// Adds a control.
        /// </summary>
        /// <typeparam name="T">type of control</typeparam>
        private void AddControl(Type controlType)
        {
            if (SelectedTreeItem is XmlWindow)
            {
                var window = SelectedTreeItem as XmlWindow;
                int id = window.Controls.Any() ? window.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                window.Controls.Add(CreateControl(controlType, id, window.Id));
            }
            else if (SelectedTreeItem is XmlDialog)
            {
                var dialog = SelectedTreeItem as XmlDialog;
                int id = dialog.Controls.Any() ? dialog.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                dialog.Controls.Add(CreateControl(controlType, id, dialog.Id));
            }
            else if (SelectedTreeItem is XmlGroup)
            {
                var group = SelectedTreeItem as XmlGroup;
                int newId = GetControlParent(CurrentXmlControl).Controls.GetControls().Max(c => c.Id) + 1;
                group.Controls.Add(CreateControl(controlType, newId, group.WindowId));
            }
            HasPendingChanges = true;
        }

        /// <summary>
        /// Determines whether CopyControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if CopyControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteCopyControlCommand()
        {
            return CurrentXmlControl != null;
        }

        /// <summary>
        /// Copies the control.
        /// </summary>
        /// <param name="isCut">if set to <c>true</c> [is cut].</param>
        private void CopyControl(bool isCut)
        {
            if (SelectedTreeItem is XmlControl)
            {
                SkinClipboard.SetData(SelectedTreeItem as XmlControl, !isCut);
            }
        }

        /// <summary>
        /// Determines whether PasteControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if PasteControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecutePasteControlCommand()
        {
            return SkinClipboard.HasData && IsControlHostSelected;
        }

        /// <summary>
        /// Pastes the control.
        /// </summary>
        private void PasteControl()
        {
            if (IsControlHostSelected && SkinClipboard.HasData)
            {
                var control = SkinClipboard.GetData();
                if (SkinClipboard.IsCut)
                {
                    var parent = GetControlParent(control);
                    if (parent != null)
                    {
                        parent.Controls.Remove(control);
                    }
                }

                int newId = 0;
                var newParent = SelectedTreeItem as IXmlControlHost;
                if (newParent is XmlGroup)
                {
                    var parentWindow = CurrentXmlWindow as IXmlControlHost ?? CurrentXmlDialog as IXmlControlHost;
                    if (parentWindow != null)
                    {
                        newId = parentWindow.Controls.Any() ? parentWindow.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                    }
                }
                else
                {
                    newId = newParent.Controls.Any() ? newParent.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                }


                control.Id = newId;
                control.WindowId = _currentXmlWindow != null ? _currentXmlWindow.Id : _currentXmlDialog.Id;

                if (control is XmlGroup)
                {
                    foreach (var item in (control as XmlGroup).Controls.GetControls())
                    {
                        newId++;
                        item.Id = newId;
                        item.WindowId = control.WindowId;
                    }
                }
                newParent.Controls.Add(control);

                SkinClipboard.ClearData();
                HasPendingChanges = true;
            }
        }
        

        /// <summary>
        /// Determines whether DeleteControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if DeleteControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteDeleteControlCommand()
        {
            return SelectedTreeItem != null;
        }

        /// <summary>
        /// Deletes the control.
        /// </summary>
        private void DeleteControl()
        {
            if (SelectedTreeItem != null)
            {
                if (SelectedTreeItem is XmlWindow)
                {
                    SkinInfo.Windows.Remove(SelectedTreeItem as XmlWindow);
                    NotifyPropertyChanged("AllWindows");
                }
                else if (SelectedTreeItem is XmlDialog)
                {
                    SkinInfo.Dialogs.Remove(SelectedTreeItem as XmlDialog);
                    NotifyPropertyChanged("AllWindows");
                }
                else if (SelectedTreeItem is XmlControl)
                {
                    var control = SelectedTreeItem as XmlControl;
                    var parent = GetControlParent(control);
                    if (parent != null)
                    {
                        parent.Controls.Remove(control);
                    }
                }
                HasPendingChanges = true;
            }
        }

        /// <summary>
        /// Determines whether MoveControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if MoveControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteMoveControlCommand()
        {
            return CurrentXmlControl != null;
        }

        /// <summary>
        /// Moves the control.
        /// </summary>
        /// <param name="up">if set to <c>true</c> [up].</param>
        private void MoveControl(bool up)
        {
            if (SelectedTreeItem is XmlControl)
            {
                var control = SelectedTreeItem as XmlControl;
                var parent = GetControlParent(control);
                if (parent != null)
                {
                    int currentIndex = parent.Controls.IndexOf(control);
                    int newIndex = up ? Math.Max(0, currentIndex - 1) : Math.Min(parent.Controls.Count - 1, currentIndex + 1);
                    parent.Controls.Move(currentIndex, newIndex);
                }
            }
        }

        /// <summary>
        /// On Skins context menu opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ContextMenuEventArgs"/> instance containing the event data.</param>
        private void SkinContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            if (source is Grid || (source is TextBlock && source.Tag != null && source.Tag.ToString() == "Group"))
            {
                e.Handled = true;
                return;
            }
        }

        #endregion

    

     
        private void Button_AddWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindowDialog = new NewWindowDialog(SkinInfo, Settings.DesignerStyle);
            if (newWindowDialog.ShowDialog() == true)
            {
                NotifyPropertyChanged("AllWindows");
                HasPendingChanges = true;
                SelectTreeItem(newWindowDialog.NewWindow);
            }
        }

     

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            HasPendingChanges = true;
        }

    

   


    



     

    

     

  
     
    }






    #region SkinClipboard

    /// <summary>
    /// 
    /// </summary>
    public static class SkinClipboard
    {
        private static XmlControl _clipboardControl;
        private static bool _isCopy = false;

        /// <summary>
        /// Gets a value indicating whether the clipboard has data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has data; otherwise, <c>false</c>.
        /// </value>
        public static bool HasData
        {
            get { return _clipboardControl != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the clipboard content is cut.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cut; otherwise, <c>false</c>.
        /// </value>
        public static bool IsCut
        {
            get { return !_isCopy; }
        }

        /// <summary>
        /// Sets the clipboard data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="isCopy">if set to <c>true</c> [is copy].</param>
        public static void SetData(XmlControl data, bool isCopy = true)
        {
            _isCopy = isCopy;
            _clipboardControl = data;
        }

        /// <summary>
        /// Gets the clipboard data.
        /// </summary>
        /// <returns></returns>
        public static XmlControl GetData()
        {
            return _isCopy ? _clipboardControl.CreateCopy() : _clipboardControl;
        }

        /// <summary>
        /// Clears the clipboard data.
        /// </summary>
        public static void ClearData()
        {
            _clipboardControl = null;
        }
    }

    #endregion
}
