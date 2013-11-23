using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;

using System.IO;

using Common.Settings;
using Common.Logging;
using Common.Helpers;

namespace SkinHelper
{
    [PluginIcons("SkinHelper.Resources.Enabled.png", "SkinHelper.Resources.Disable.png")]
    public class SkinHelper : IPlugin, ISetupForm
    {
        #region vars

       // public static ILog Log;

        #endregion

        #region IPlugin Implementation

        public SkinHelper()
        {
        
        }

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

        public void Start()
        {
            if (Directory.Exists(RegistrySettings.ProgramDataPath))
            {
                string logdir = RegistrySettings.ProgramDataPath + @"Logs\SkinHelper";
                if (!Directory.Exists(logdir))
                {
                    Directory.CreateDirectory(logdir);
                }

                Common.Logging.LoggingManager.AddLog(new Common.Logging.FileLogger(logdir, "Action", LogLevel.Debug), "Action");
                Common.Logging.LoggingManager.AddLog(new Common.Logging.FileLogger(logdir, "Property", LogLevel.Debug), "Property");
                Common.Logging.LoggingManager.AddLog(new Common.Logging.FileLogger(logdir, "ListItem", LogLevel.Debug), "ListItem");
                Common.Logging.LoggingManager.AddLog(new Common.Logging.FileLogger(logdir, "Dialog", LogLevel.Debug), "Dialog");
                LoggingManager.AddLog(new Common.Logging.FileLogger(logdir, "Window", LogLevel.Debug), "Window");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Could Not Locate MPDisplay Data Path '" + RegistrySettings.ProgramDataPath + "'" + Environment.NewLine + "If Problem Persists Please Reinstall MPDisplay++", "Install Error", System.Windows.Forms.MessageBoxButtons.OK);
                Stop();
                return;
            }
            GUIPropertyManager.OnPropertyChanged += new GUIPropertyManager.OnPropertyChangedHandler(GUIPropertyManager_OnPropertyChanged);
            GUIWindowManager.OnActivateWindow += new GUIWindowManager.WindowActivationHandler(GUIWindowManager_OnActivateWindow);
            GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);
            _dialogTimer = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessDialogThread), null, 1000, 250);
        }

        #endregion

        #region Methods

        private int _lastFocusedControl = -1;
        private object _syncObject = new object();
        private Common.Logging.Log ActionLog;
        private Common.Logging.Log PropertyLog;
        private Common.Logging.Log ListItemLog;
        private Common.Logging.Log DialogLog;
        private Common.Logging.Log WindowLog;


        void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
            lock (_syncObject)
            {
                if (GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx) != null)
                {
                    try
                    {
                        int id = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx).GetFocusControlId();
                        if (id != _lastFocusedControl)
                        {
                            _lastFocusedControl = id;
                            LoggingManager.GetLog(typeof(SkinHelper), "Window").Message(LogLevel.Info, "-[FocusedControl] - ControlId: {0}", _lastFocusedControl);
                        }
                    }
                    catch { }
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
                if (tag != null && !tag.StartsWith("#SkinInfo."))
                {
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
        }

        private void GetButtonMenuItemInfo()
        {
            try
            {
                var currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx);
                if (currentWindow != null)
                {
                    var focusedControl = currentWindow.GetControl(currentWindow.GetFocusControlId());
                    if (focusedControl != null && (focusedControl.GetType() == typeof(GUIButtonControl) || focusedControl.GetType() == typeof(GUICheckButton) 
                        || focusedControl.GetType() == typeof(GUIMenuButton) || focusedControl.GetType() == typeof(GUISelectButtonControl)
                        || focusedControl.GetType() == typeof(GUISortButtonControl) || focusedControl.GetType() == typeof(GUISpinButton) || focusedControl.GetType() == typeof(GUIUpDownButton)))
                       // || focusedControl.GetType() == typeof(GUIToggleButtonControl) || focusedControl.GetType() == typeof(GUIUpDownButton)))
                    {
                        RefectButton(focusedControl);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void GetListItemInfo()
        {
            try
            {
                var currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindowEx);
                if (currentWindow != null)
                {
                    var focusedControl = currentWindow.GetControl(currentWindow.GetFocusControlId());
                    if (focusedControl != null)
                    {
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
                            var listItems = (focusedControl as GUIListControl).ListItems;
                            if (listItems != null && listItems.Count > 1)
                            {
                                RefectListItem(listItems[1]);
                            }
                        }
                        else if (focusedControl.GetType() == typeof(GUIPlayListItemListControl))
                        {
                            var listItems = (focusedControl as GUIPlayListItemListControl).ListItems;
                            if (listItems != null && listItems.Count > 1)
                            {
                                RefectListItem(listItems[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }









        /// <summary>
        /// Gets the correct items form the facades layout type
        /// </summary>
        /// <param name="facade">The current facade</param>
        /// <returns>The listitems from the facade</returns>
        private List<GUIListItem> GetFacadeListItems(GUIFacadeControl facade)
        {
            if (facade != null)
            {
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
            return new List<GUIListItem>();
        }

        private bool _isWorking = false;
        private bool _isDialogVisible;
        private GUIWindow _currentDialog;
        private int _currentDialogId = -1;
        private System.Threading.Timer _dialogTimer;

        /// <summary>
        /// Processes the dialog thread.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ProcessDialogThread(object state)
        {
            try
            {
                if (!_isWorking)
                {
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
            }
            catch (Exception ex)
            {
               
            }
        }


        /// <summary>
        /// Sends the dialog properties.
        /// </summary>
        private void SendDialogProperties()
        {
            try
            {
                if (_currentDialog != null)
                {
                    foreach (var item in _currentDialog.Children)
                    {
                        try
                        {
                            if (item.GetType() == typeof(GUIImage))
                            {
                                var image = item as GUIImage;
                                if (image != null)
                                {
                                    LoggingManager.GetLog(typeof(SkinHelper), "Dialog").Message(LogLevel.Info, string.Format("[Dialog]-[DialogProperties] - Dialog property sent, Tag: #Dialog.Image{0}, TagValue: {1}", item.GetID, image.FileName));
                                }
                            }
                            else
                            {
                                if (item != null)
                                {
                                    var label = ReflectionHelper.GetPropertyValue<string>(item, "Label", null);
                                    if (!string.IsNullOrEmpty(label))
                                    {
                                        LoggingManager.GetLog(typeof(SkinHelper), "Dialog").Message(LogLevel.Info, string.Format("[Dialog]-[DialogProperties] - Dialog property sent, Tag: #Dialog.Label{0}, TagValue: {1}", item.GetID, label));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              
            }
        }








        private void RefectListItem(GUIListItem listItem)
        {
            try
            {
                if (listItem != null)
                {
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
                            catch { }
                        }
                    }
                    LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[ListItemDetail]{0}", Environment.NewLine);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void RefectButton(GUIControl control)
        {
            try
            {
                if (control != null)
                {
                    LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]{0}", Environment.NewLine);
                    LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail] - Plugin: {0}, ControlId: {1}", GUIWindowManager.ActiveWindow, control.GetID);
                    foreach (var property in control.GetType().GetProperties().Where(p => p.PropertyType == typeof(string)))
                    {
                        try
                        {
                            LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]-[{0}] - PropertyName: {1}, PropertyValue: {2}", control.Type, property.Name, property.GetValue(control, null));
                        }
                        catch { }
                    }
                    LoggingManager.GetLog(typeof(SkinHelper), "ListItem").Message(LogLevel.Warn, "[MenuButtonDetail]{0}", Environment.NewLine);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

    }
}

