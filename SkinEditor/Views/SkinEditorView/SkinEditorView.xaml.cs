using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Common.Helpers;
using GUISkinFramework;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.Utils;
using SkinEditor.Dialogs;
using SkinEditor.Helpers;
using SkinEditor.Themes;

namespace SkinEditor.Views
{
    /// <summary>
    /// Interaction logic for SkinEditorView.xaml
    /// </summary>
    public partial class SkinEditorView
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
        public SkinEditorView(ConnectionHelper connectionHelper)           
        {
            ConnectionHelper = connectionHelper;

            ConnectCommand = new RelayCommand(async () => await ConnectionHelper.InitializeServerConnection(), () => !ConnectionHelper.IsConnected);
            DisconnectCommand = new RelayCommand(async () => await ConnectionHelper.Disconnect(), () => ConnectionHelper.IsConnected);
            ClearPropertyCommand = new RelayCommand(ConnectionHelper.PropertyData.Clear);
            ClearListItemCommand = new RelayCommand(ConnectionHelper.ListItemData.Clear);
            PropertyEditCommand = new RelayCommand<SkinPropertyItem>(ConnectionHelper.OpenPropertyEditor);

            InitializeComponent();
            CreateContextMenuCommands();
        }

        #endregion

        #region connection

        public InfoEditorViewSettings ConnectionSettings
        {
            get { return ConnectSettings as InfoEditorViewSettings; }
        }

        public ICommand ConnectCommand { get; internal set; }
        public ICommand DisconnectCommand { get; internal set; }
        public ICommand ClearPropertyCommand { get; internal set; }
        public ICommand PropertyEditCommand { get; internal set; }
        public ICommand ClearListItemCommand { get; internal set; }

        public bool IsConnected
        {
            get { return ConnectionHelper.IsConnected; }
        }

        public bool IsMediaPortalConnected
        {
            get { return ConnectionHelper.IsMediaPortalConnected; }
        }

        public ObservableCollection<SkinPropertyItem> PropertyData
        {
            get { return ConnectionHelper.PropertyData; }
        }

        public ObservableCollection<SkinPropertyItem> ListItemData
        {
            get { return ConnectionHelper.ListItemData; }
        }

        public int WindowId
        {
            get { return ConnectionHelper.WindowId; }
        }

        public int DialogId
        {
            get { return ConnectionHelper.DialogId; }
        }

        public int FocusedControlId
        {
            get { return ConnectionHelper.FocusedControlId; }
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
            get { return Windows.Concat(Dialogs.Cast<IXmlControlHost>()); }
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
            ConnectionHelper.Settings = ConnectionSettings;
            ConnectionHelper.Baseclass = this;
            ConnectionHelper.StartSecondTimer();

            NotifyPropertyChanged("Styles");
            NotifyPropertyChanged("Settings");
            NotifyPropertyChanged("AllWindows");
            SelectedStyle = SkinInfo.CurrentStyle;
            SelectedLanguage = SkinInfo.CurrentLanguage;
            TreeView_SelectedItemChanged(null, new RoutedPropertyChangedEventArgs<object>(null, SkinInfo.Windows.FirstOrDefault(w => w.IsDefault)));
        //    SelectedTreeItem = Windows.FirstOrDefault(w => w.IsDefault);
        }

        public override void OnModelOpen()
        {
            base.OnModelOpen();
            NotifyPropertyChanged("Styles");
            SkinInfo.SetStyle(_selectedStyle);
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
            if (e.NewValue == e.OldValue) return;

            SelectedTreeItem = e.NewValue;
            var value = e.NewValue as XmlWindow;
            if (value != null)
            {
                CurrentXmlControl = null;
                CurrentXmlDialog = null;
                CurrentXmlWindow = value;
            }
            else
            {
                var dialog = e.NewValue as XmlDialog;
                if (dialog != null)
                {
                    CurrentXmlControl = null;
                    CurrentXmlWindow = null;
                    CurrentXmlDialog = dialog;
                }
                else
                {
                    var newValue = e.NewValue as XmlControl;
                    if (newValue == null) return;

                    CurrentXmlControl = null;
                    var control = newValue;
                    {
                        var parentW = GetControlParent(control) as XmlWindow;
                        if (parentW != null && (CurrentXmlWindow == null || parentW.Id != CurrentXmlWindow.Id))
                        {
                            CurrentXmlDialog = null;
                            CurrentXmlWindow = parentW;
                        }
                        var parentD = GetControlParent(control) as XmlDialog;
                        if (parentD != null && (CurrentXmlDialog == null || parentD.Id != CurrentXmlDialog.Id))
                        {
                            CurrentXmlWindow = null;
                            CurrentXmlDialog = parentD;
                        }
                    }
                    CurrentXmlControl = control;
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
                foreach (var @group in parent.Controls.GetControls().OfType<XmlGroup>().Where(@group => @group.Controls.Contains(control)))
                {
                    return @group;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="id">The id.</param>
        /// <param name="windowId">The window Id</param>
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
                    LabelFixedText = "1:23:45",
                    LabelMovingText = "0:34:56",
                    ControlStyle = controlStyle as XmlProgressBarStyle ?? new XmlProgressBarStyle()
                };
            }

            if (controlType == typeof(XmlGuide))
            {
                return new XmlGuide
                {
                    Id = id,
                    Name = "NewGuide",
                    Width = 1000,
                    Height = 600,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    ControlStyle = controlStyle as XmlGuideStyle ?? new XmlGuideStyle(),
                    ChannelStyle = Settings.DesignerStyle.GetDesignerStyle(typeof(XmlGuideChannelStyle)) as XmlGuideChannelStyle ?? new XmlGuideChannelStyle(),
                    ProgramStyle = Settings.DesignerStyle.GetDesignerStyle(typeof(XmlGuideProgramStyle)) as XmlGuideProgramStyle ?? new XmlGuideProgramStyle()
                };
            }

            if (controlType == typeof(XmlEqualizer))
            {
                return new XmlEqualizer
                {
                    Id = id,
                    Name = "NewEqualizer",
                    Width = 400,
                    Height = 400,
                    IsWindowOpenVisible = true,
                    WindowId = windowId,
                    EQStyle = XmlEQStyle.SingleBar,
                    LowRangeValue = 100,
                    MedRangeValue = 200,
                    BandCount = 18,
                    BandSpacing = 4,
                    BandBorderSize = 1,
                    BandCornerRadius = 0,
                    FalloffSpeed = 4,
                    FallOffHeight = 2,
                    ControlStyle = controlStyle as XmlEqualizerStyle ?? new XmlEqualizerStyle()
                };
            }

            if (controlType == typeof(XmlList))
            {
                var coverFlowItemStyle = Settings.DesignerStyle.GetDesignerStyle("CoverFlowItemStyle") as XmlListItemStyle;
                var verticalItemStyle = Settings.DesignerStyle.GetDesignerStyle("VerticalItemStyle") as XmlListItemStyle;
                var verticalIconItemStyle = Settings.DesignerStyle.GetDesignerStyle("VerticalIconItemStyle") as XmlListItemStyle;
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
                    VerticalIconItemStyle = verticalIconItemStyle ?? new XmlListItemStyle(),
                    HorizontalItemStyle = horizontalItemStyle ?? new XmlListItemStyle(),
                    ListLayout = XmlListLayout.Auto
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
        public ICommand HideControlCommand { get; internal set; }
        public ICommand UnhideControlCommand { get; internal set; }

        /// <summary>
        /// Populates the context menu.
        /// </summary>
        private void CreateContextMenuCommands()
        {
            AddControlCommand = new RelayCommand(param => AddControl((Type)param), CanExecuteAddControlCommand);
            CopyControlCommand = new RelayCommand<string>(p => CopyControl(bool.Parse(p)), param => CanExecuteCopyControlCommand());
            PasteControlCommand = new RelayCommand(PasteControl,CanExecutePasteControlCommand);
            DeleteControlCommand = new RelayCommand(DeleteControl,CanExecuteDeleteControlCommand);
            MoveControlCommand = new RelayCommand<string>(p => MoveControl(bool.Parse(p)), param => CanExecuteMoveControlCommand());
            HideControlCommand = new RelayCommand(HideControl, CanExecuteHideControlCommand);
            UnhideControlCommand = new RelayCommand(UnhideControl, CanExecuteUnhideControlCommand);
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
        /// <param name="controlType">type of control</param>
        private void AddControl(Type controlType)
        {
            var xmlWindow = SelectedTreeItem as XmlWindow;
            if (xmlWindow != null)
            {
                var window = xmlWindow;
                var id = window.Controls.Any() ? window.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                window.Controls.Add(CreateControl(controlType, id, window.Id));
            }
            else
            {
                var xmlDialog = SelectedTreeItem as XmlDialog;
                if (xmlDialog != null)
                {
                    var dialog = xmlDialog;
                    var id = dialog.Controls.Any() ? dialog.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                    dialog.Controls.Add(CreateControl(controlType, id, dialog.Id));
                }
                else
                {
                    var xmlGroup = SelectedTreeItem as XmlGroup;
                    if (xmlGroup != null)
                    {
                        var group = xmlGroup;
                        var newId = GetControlParent(CurrentXmlControl).Controls.GetControls().Max(c => c.Id) + 1;
                        @group.Controls.Add(CreateControl(controlType, newId, @group.WindowId));
                    }
                }
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
            var data = SelectedTreeItem as XmlControl;
            if (data != null)
            {
                SkinClipboard.SetData(data, !isCut);
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
            if (!IsControlHostSelected || !SkinClipboard.HasData) return;

            var control = SkinClipboard.GetData();
            if (SkinClipboard.IsCut)
            {
                var parent = GetControlParent(control);
                if (parent != null)
                {
                    parent.Controls.Remove(control);
                }
            }

            var newId = 0;
            var newParent = SelectedTreeItem as IXmlControlHost;
            if (newParent is XmlGroup)
            {
                var parentWindow = CurrentXmlWindow ?? CurrentXmlDialog as IXmlControlHost;
                if (parentWindow != null)
                {
                    newId = parentWindow.Controls.Any() ? parentWindow.Controls.GetControls().Max(c => c.Id) + 1 : 1;
                }
            }
            else
            {
                newId = newParent != null && newParent.Controls.Any() ? newParent.Controls.GetControls().Max(c => c.Id) + 1 : 1;
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
            if (newParent != null) newParent.Controls.Add(control);

            SkinClipboard.ClearData();
            HasPendingChanges = true;
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
            if (SelectedTreeItem == null) return;

            var item = SelectedTreeItem as XmlWindow;
            if (item != null)
            {
                SkinInfo.Windows.Remove(item);
                NotifyPropertyChanged("AllWindows");
            }
            else
            {
                var dialog = SelectedTreeItem as XmlDialog;
                if (dialog != null)
                {
                    SkinInfo.Dialogs.Remove(dialog);
                    NotifyPropertyChanged("AllWindows");
                }
                else
                {
                    var xmlControl = SelectedTreeItem as XmlControl;
                    if (xmlControl != null)
                    {
                        var control = xmlControl;
                        var parent = GetControlParent(control);
                        if (parent != null)
                        {
                            parent.Controls.Remove(control);
                        }
                    }
                }
            }
            HasPendingChanges = true;
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
            var xmlControl = SelectedTreeItem as XmlControl;
            if (xmlControl == null) return;

            var control = xmlControl;
            var parent = GetControlParent(control);
            if (parent == null) return;

            var currentIndex = parent.Controls.IndexOf(control);
            var newIndex = up ? Math.Max(0, currentIndex - 1) : Math.Min(parent.Controls.Count - 1, currentIndex + 1);
            parent.Controls.Move(currentIndex, newIndex);
        }

        /// <summary>
        /// Determines whether HideControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if HideControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteHideControlCommand()
        {
            var xmlControl = SelectedTreeItem as XmlControl;
            if (xmlControl == null) return false;

            var control = xmlControl;
            return GetControlsVisible(control, true);
        }

        /// <summary>
        /// Hide the control and all childen.
        /// </summary>
        private void HideControl()
        {
            var xmlControl = SelectedTreeItem as XmlControl;
            if (xmlControl == null) return;

            var control = xmlControl;
            SetControlsVisible(control, false);
        }

        /// <summary>
        /// Determines whether UnhideControlCommand can execute.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if UnhideControlCommand can execute; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteUnhideControlCommand()
        {
            var xmlControl = SelectedTreeItem as XmlControl;
            if (xmlControl == null) return false;

            var control = xmlControl;
            return GetControlsVisible(control, false);
        }

        /// <summary>
        /// Unhide the control and all childen.
        /// </summary>
        private void UnhideControl()
        {
            var xmlControl = SelectedTreeItem as XmlControl;
            if (xmlControl == null) return;

            var control = xmlControl;
            SetControlsVisible(control, true);
            SetParentControlsVisible(control, true);
        }

        // set recursivle the DesignerVisibile property of control and all childs
        private static void SetControlsVisible( XmlControl control, bool value ) 
        {
            if (control == null) return;

            var host = control as XmlGroup;
            if (host != null)
            {
                foreach (var control2 in ((IXmlControlHost) control).Controls)
                {
                    SetControlsVisible(control2, value);
                }
            }
            control.DesignerVisible = value;
        }

        // set recursivly the DesignerVisibility property of all parents of control
        private void SetParentControlsVisible(XmlControl control, bool value)
        {
            if (control == null) return;

            var control2 = GetControlParent(control);
            if (!(control2 is XmlGroup)) return;

            (control2 as XmlGroup).DesignerVisible = value;
            SetParentControlsVisible((control2 as XmlGroup), value);
        }

        // get recursively the property DesignerVisible.
        // if control is a group check if any of the childs has the value 'value'
        private static bool GetControlsVisible(XmlControl control, bool value)
        {
            if (control == null) return false;

            var result = false;
            var host = control as XmlGroup;
            if (host != null)
            {
                result = ((IXmlControlHost) control).Controls.Aggregate(false, (current, control2) => current | GetControlsVisible(control2, value));
            }
            else
            {
                result |= control.DesignerVisible == value;
            }
            return result;
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
            }
        }

        #endregion

        private void Button_AddWindow_Click(object sender, RoutedEventArgs e)
        {
            var newWindowDialog = new NewWindowDialog(SkinInfo, Settings.DesignerStyle);
            if (newWindowDialog.ShowDialog() != true) return;

            NotifyPropertyChanged("AllWindows");
            HasPendingChanges = true;
            SelectTreeItem(newWindowDialog.NewWindow);
        }

        private void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            HasPendingChanges = true;
        }


        //void s_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is TreeViewItem && CurrentXmlControl != null)
        //    {
        //        TreeViewItem draggedItem = sender as TreeViewItem;
        //        if (draggedItem.DataContext is XmlControl)
        //        {
        //            DragDrop.DoDragDrop(draggedItem, CurrentXmlControl, DragDropEffects.Move);
        //        }
        //    }
        //}

        //void listbox1_Drop(object sender, DragEventArgs e)
        //{
        //    if (e.Data != null && _currentXmlWindow != null && CurrentXmlControl != null)
        //    {
        //        try
        //        {
        //            var droppedData = e.Data.GetData(CurrentXmlControl.GetType()) as XmlControl;
        //            var target = ((TreeViewItem)(sender)).DataContext as XmlControl;
        //            if (droppedData != null && target != null)
        //            {
        //                int removedIdx = _currentXmlWindow.Controls.IndexOf(droppedData);
        //                int targetIdx = _currentXmlWindow.Controls.IndexOf(target);
        //                if (removedIdx < targetIdx)
        //                {
        //                    _currentXmlWindow.Controls.Insert(targetIdx + 1, droppedData);
        //                    _currentXmlWindow.Controls.RemoveAt(removedIdx);
        //                }
        //                else
        //                {
        //                    int remIdx = removedIdx + 1;
        //                    if (_currentXmlWindow.Controls.Count + 1 > remIdx)
        //                    {
        //                        _currentXmlWindow.Controls.Insert(targetIdx, droppedData);
        //                        _currentXmlWindow.Controls.RemoveAt(remIdx);
        //                    }
        //                }
        //                SelectTreeItem(CurrentXmlControl);
        //            }
        //        }
        //        catch 
        //        {
        //            MessageBox.Show("Cannot move controls to a new Window/Dialog.");
                  
        //        }
        //    }
        //}
     
    }

    #region SkinClipboard

    /// <summary>
    /// 
    /// </summary>
    public static class SkinClipboard
    {
        private static XmlControl _clipboardControl;
        private static bool _isCopy;

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
