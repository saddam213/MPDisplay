using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;
using Timer = System.Threading.Timer;

namespace SkinHelper
{
    [PluginIcons("SkinHelper.Resources.Enabled.png", "SkinHelper.Resources.Disable.png")]
    public class SkinHelper : IPlugin, ISetupForm
    {
        #region vars

       // public static ILog Log;

        #endregion

        #region IPlugin Implementation

        public string Author()
        {
            return "Sa_ddam213";
        }

        public bool CanEnable()
        {
            return true;
        }

        public bool DefaultEnabled()
        {
            return true;
        }

        public string Description()
        {
            return "Skin Design Helper";
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = string.Empty;
            strButtonImage = string.Empty;
            strButtonImageFocus = string.Empty;
            strPictureImage = string.Empty;
            return false;
        }

        public int GetWindowId()
        {
            return 0;
        }

        public bool HasSetup()
        {
            return true;
        }

        public string PluginName()
        {
            return "Skin Design Helper";
        }

        public void ShowPlugin()
        {

        }


        public void Stop()
        {

        }

        internal Timer DialogTimer;

        public void Start()
        {
            if (Directory.Exists(RegistrySettings.ProgramDataPath))
            {
                var logdir = RegistrySettings.ProgramDataPath + @"Logs\SkinHelper";
                if (!Directory.Exists(logdir))
                {
                    Directory.CreateDirectory(logdir);
                }

                LoggingManager.AddLog(new FileLogger(logdir, "Action", LogLevel.Debug), "Action");
                LoggingManager.AddLog(new FileLogger(logdir, "Property", LogLevel.Debug), "Property");
                LoggingManager.AddLog(new FileLogger(logdir, "ListItem", LogLevel.Debug), "ListItem");
                LoggingManager.AddLog(new FileLogger(logdir, "Dialog", LogLevel.Debug), "Dialog");
                LoggingManager.AddLog(new FileLogger(logdir, "Window", LogLevel.Debug), "Window");
            }
            else
            {
                MessageBox.Show(@"Could Not Locate MPDisplay Data Path '" + RegistrySettings.ProgramDataPath + @"'" + Environment.NewLine + @"If Problem Persists Please Reinstall MPDisplay++",
                    @"Install Error", MessageBoxButtons.OK);
                Stop();
                return;
            }
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;
            GUIWindowManager.OnActivateWindow += GUIWindowManager_OnActivateWindow;
            GUIWindowManager.OnNewAction += GUIWindowManager_OnNewAction;
            DialogTimer = new Timer(ProcessDialogThread, null, 1000, 250);
        }

        #endregion

        #region Methods

        private int _lastFocusedControl = -1;
        private readonly object _syncObject = new object();

        void GUIWindowManager_OnNewAction(Action action)
        {
            lock (_syncObject)
            {
                if (GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx) != null)
                {
                    try
                    {
                        var id = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx).GetFocusControlId();
                        if (id != _lastFocusedControl)
                        {
                            _lastFocusedControl = id;
                            LoggingManager.GetLog(typeof(SkinHelper), "Window").Message(LogLevel.Info, "-[FocusedControl] - ControlId: {0}", _lastFocusedControl);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                LoggingManager.GetLog(typeof(SkinHelper), "Action").Message(LogLevel.Debug, "-[Action] - ActionId: {0}", action.wID);
            }
        }

        void GUIWindowManager_OnActivateWindow(int windowId)
        {
            lock (_syncObject)
            {
                LoggingManager.GetLog(typeof(SkinHelper), "Window").Message(LogLevel.Info, "-[Window] - WindowId: {0}", windowId);
            }
        }

        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            lock (_syncObject)
            {
                if (tag == null || tag.StartsWith("#SkinInfo.")) return;
                LoggingManager.GetLog(typeof(SkinHelper), "Property").Message(LogLevel.Debug, "-[Property] - Tag: {0}, TagValue: {1}", tag, tagValue);
                if (tag.Equals("#currentmodule") && GUIWindowManager.IsRouted)
                {
                    LoggingManager.GetLog(typeof(SkinHelper), "Dialog").Message(LogLevel.Debug, "-[Dialog] - DialogId: {0}", GUIWindowManager.RoutedWindow);
                }

                if (tag.Equals("#selectedindex"))
                {
                    GetListItemInfo();
                }

                if (tag.Equals("#highlightedbutton") && !string.IsNullOrEmpty(tagValue))
                {
                    GetButtonMenuItemInfo();
                }
            }
        }

        private static void GetButtonMenuItemInfo()
        {
            try
            {
                var currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx);
                if (currentWindow == null) return;

                var focusedControl = currentWindow.GetControl(currentWindow.GetFocusControlId());
                if (focusedControl != null && (focusedControl.GetType() == typeof(GUIButtonControl) || focusedControl.GetType() == typeof(GUICheckButton) 
                                               || focusedControl.GetType() == typeof(GUIMenuButton) || focusedControl.GetType() == typeof(GUISelectButtonControl)
                                               || focusedControl.GetType() == typeof(GUISortButtonControl) || focusedControl.GetType() == typeof(GUISpinButton) || focusedControl.GetType() == typeof(GUIUpDownButton)))
                    // || focusedControl.GetType() == typeof(GUIToggleButtonControl) || focusedControl.GetType() == typeof(GUIUpDownButton)))
                {
                    RefectButton(focusedControl);
                }
            }
            catch
            {
                // ignored
            }
        }

        private static void GetListItemInfo()
        {
            try
            {
                var currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx);

                var focusedControl = currentWindow?.GetControl(currentWindow.GetFocusControlId());
                if (focusedControl == null) return;

                if (focusedControl.GetType() == typeof(GUIFacadeControl))
                {
                    var listItems = GetFacadeListItems(focusedControl as GUIFacadeControl);
                    if (listItems != null && listItems.Count > 1)
                    {
                        RefectListItem(listItems[1]);
                    }
                }
                else if (focusedControl.GetType() == typeof(GUIListControl))
                {
                    var guiListControl = focusedControl as GUIListControl;
                    if (guiListControl == null) return;

                    var listItems = guiListControl.ListItems;
                    if (listItems != null && listItems.Count > 1)
                    {
                        RefectListItem(listItems[1]);
                    }
                }
                else if (focusedControl.GetType() == typeof(GUIPlayListItemListControl))
                {
                    var guiPlayListItemListControl = focusedControl as GUIPlayListItemListControl;
                    if (guiPlayListItemListControl == null) return;

                    var listItems = guiPlayListItemListControl.ListItems;
                    if (listItems != null && listItems.Count > 1)
                    {
                        RefectListItem(listItems[1]);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Gets the correct items form the facades layout type
        /// </summary>
        /// <param name="facade">The current facade</param>
        /// <returns>The listitems from the facade</returns>
        private static List<GUIListItem> GetFacadeListItems(GUIFacadeControl facade)
        {
            if (facade == null) return new List<GUIListItem>();

            switch (facade.CurrentLayout)
            {
                case GUIFacadeControl.Layout.AlbumView:
                    return facade.AlbumListLayout.ListItems;
                case GUIFacadeControl.Layout.CoverFlow:
                    return facade.FilmstripLayout.ListItems;
                case GUIFacadeControl.Layout.Filmstrip:
                    return facade.FilmstripLayout.ListItems;
                case GUIFacadeControl.Layout.LargeIcons:
                    return facade.ThumbnailLayout.ListItems;
                case GUIFacadeControl.Layout.List:
                    return facade.ListLayout.ListItems;
                case GUIFacadeControl.Layout.Playlist:
                    return facade.PlayListLayout.ListItems;
                case GUIFacadeControl.Layout.SmallIcons:
                    return facade.ThumbnailLayout.ListItems;
                default:
                    return null;
            }
        }

        private bool _isWorking;
        private bool _isDialogVisible;
        private GUIWindow _currentDialog;
        private int _currentDialogId = -1;

        /// <summary>
        /// Processes the dialog thread.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ProcessDialogThread(object state)
        {
            try
            {
                if (_isWorking) return;
                _isWorking = true;
                if ((GUIWindowManager.IsRouted && !_isDialogVisible) || (GUIWindowManager.IsRouted && _isDialogVisible && _currentDialogId != GUIWindowManager.RoutedWindow))
                {
                    _currentDialog = GUIWindowManager.GetWindow(GUIWindowManager.RoutedWindow);
                    if (_currentDialog != null)
                    {
                        _isDialogVisible = true;
                        _currentDialogId = GUIWindowManager.RoutedWindow;
                        SendDialogProperties();
                    }
                }
                else if (!GUIWindowManager.IsRouted && _isDialogVisible)
                {
                    _isDialogVisible = false;
                }
                _isWorking = false;
            }
            catch
            {
                // ignored
            }
        }


        /// <summary>
        /// Sends the dialog properties.
        /// </summary>
        private void SendDialogProperties()
        {
            try
            {
                if (_currentDialog == null) return;
                foreach (var item in _currentDialog.Children)
                {
                    try
                    {
                        if (item.GetType() == typeof(GUIImage))
                        {
                            var image = item as GUIImage;
                            if (image != null)
                            {
                                LoggingManager.GetLog(typeof(SkinHelper), "Dialog").Message(LogLevel.Info,
                                    $"[Dialog]-[DialogProperties] - Dialog property sent, Tag: #Dialog.Image{item.GetID}, TagValue: {image.FileName}");
                            }
                        }
                        else
                        {
                            var label = ReflectionHelper.GetPropertyValue<string>(item, "Label", null);
                            if (!string.IsNullOrEmpty(label))
                            {
                                LoggingManager.GetLog(typeof(SkinHelper), "Dialog").Message(LogLevel.Info,
                                    $"[Dialog]-[DialogProperties] - Dialog property sent, Tag: #Dialog.Label{item.GetID}, TagValue: {label}");
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
        }


        private static void RefectListItem(GUIListItem listItem)
        {
            try
            {
                if (listItem == null) return;

                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail]{0}", Environment.NewLine);
                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail] - Plugin: {0}", GUIWindowManager.ActiveWindow);
                foreach (var property in listItem.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
                {
                    try
                    {
                        LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail] - PropertyName: {0}, PropertyValue: {1}", property.Name, property.GetValue(listItem, null));
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (listItem.TVTag != null)
                {
                    foreach (var property in listItem.TVTag.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
                    {
                        try
                        {
                            LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail] - PropertyName: {0}, PropertyValue: {1}", property.Name, property.GetValue(listItem.TVTag, null));
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail]{0}", Environment.NewLine);
            }
            catch
            {
                // ignored
            }
        }

        private static void RefectButton(GUIControl control)
        {
            try
            {
                if (control == null) return;

                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]{0}", Environment.NewLine);
                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail] - Plugin: {0}, ControlId: {1}", GUIWindowManager.ActiveWindow, control.GetID);
                foreach (var property in control.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
                {
                    try
                    {
                        LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]-[{0}] - PropertyName: {1}, PropertyValue: {2}", control.Type, property.Name, property.GetValue(control, null));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]{0}", Environment.NewLine);
            }
            catch
            {
                // ignored
            }
        }

        #endregion

    }
}

