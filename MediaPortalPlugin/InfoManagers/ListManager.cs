using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System.Reflection;
using MediaPortalPlugin.PluginHelpers;
using Common.Helpers;
using Common.Settings;
using Common.Logging;

namespace MediaPortalPlugin.InfoManagers
{
    public class ListManager
    {
       #region Singleton Implementation

        private static ListManager instance;

        private ListManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(ListManager));
        }

        public static ListManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ListManager();
                }
                return instance;
            }
        }

        #endregion

        private Common.Logging.Log Log;
        private static bool _groupFocused;
        private static bool _listFocused;
        private static bool _isRecheckingListItems;
        private static bool _isRecheckingListType;
        private static bool _checkItemsNeedsRefresh = false;
        private static int _focusedControlId = -1;
        private List<APIListType> _registeredListTypes = new List<APIListType>();
        private List<GUIFacadeControl> _facadeControls = new List<GUIFacadeControl>();
        private List<GUIListControl> _listControls = new List<GUIListControl>();
        private List<GUIGroup> _groupControls = new List<GUIGroup>();
        private GUIMenuControl _menuControl;
        private PluginSettings _settings;
        private static int _currentBatchId = 0;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            GUIWindowManager.OnNewAction += GUIWindowManager_OnNewAction;
            GUIWindowManager.Receivers += GUIWindowManager_Receivers;
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;
            _menuCotrolMoveUp = typeof(GUIMenuControl).GetMethod("OnUp", BindingFlags.Instance | BindingFlags.NonPublic);
            _menuCotrolMoveDown = typeof(GUIMenuControl).GetMethod("OnDown", BindingFlags.Instance | BindingFlags.NonPublic);
            _menuControlButtonList = typeof(GUIMenuControl).GetField("_buttonList", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void Shutdown()
        {
            GUIWindowManager.OnNewAction -= GUIWindowManager_OnNewAction;
            GUIWindowManager.Receivers -= GUIWindowManager_Receivers;
            GUIPropertyManager.OnPropertyChanged -= GUIPropertyManager_OnPropertyChanged;
        }

        public void RegisterWindowListTypes(List<APIListType> list)
        {
            Log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list types...");
            _registeredListTypes = new List<APIListType>(list.Distinct());
            foreach (var listType in _registeredListTypes)
            {
                Log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list type: {0}", listType);
            }
            Log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list types complete.");
            RegisterWindowListTypes();
        }

   

        public void SetWindowListControls()
        {
            if (WindowManager.Instance.CurrentWindow != null)
            {
                _facadeControls = new List<GUIFacadeControl>(WindowManager.Instance.CurrentWindow.GetControls<GUIFacadeControl>());
                _listControls = new List<GUIListControl>(WindowManager.Instance.CurrentWindow.GetControls<GUIListControl>());
                _groupControls = new List<GUIGroup>(WindowManager.Instance.CurrentWindow.GetControls<GUIGroup>()
                    .Where(g => !g.Children.GetControls().Any(c => c is GUIFacadeControl || c is GUIListControl)));
                _menuControl = WindowManager.Instance.CurrentWindow.GetControls<GUIMenuControl>().FirstOrDefault();
                RegisterWindowListTypes();
            }
        }

        public void ClearWindowListControls()
        {
            if (_registeredListTypes.Any())
            {
                _registeredListTypes.ForEach(t => SendList(t, APIListLayout.Vertical, new List<APIListItem>()));
            }
            _facadeControls.Clear();
            _listControls.Clear();
            _groupControls.Clear();
            _menuControl = null;
            _focusedControlId = -1;
            _groupFocused = false;
            _listFocused = false;
            _isRecheckingListItems = false;
            _isRecheckingListType = false;
            _currentLayout = APIListLayout.Vertical;
        }

        public void OnActionMessageReceived(APIActionMessage actionMessage)
        {
            var action = actionMessage.ListAction;
            if (action != null)
            {
                bool isSelect = action.ActionType == APIListActionType.SelectedItem;
                if (action.ItemListType == APIListType.List)
                {
                    SetFacdeSelectedItem(action, isSelect);
                    SetListSelectedItem(action, isSelect);
                }

                if (action.ItemListType == APIListType.GroupMenu)
                {
                    SetGroupSelectedItem(action, isSelect);
                }

                if (action.ItemListType == APIListType.Menu)
                {
                    SetMenuControlSelectedItem(action, isSelect);
                }
            }
        }

        private void RegisterWindowListTypes()
        {
            if (_registeredListTypes.Any())
            {
              
                if (_registeredListTypes.Contains(APIListType.List))
                {
                    SendFacadeList();
                    SendListControlList();
                    SendFacdeSelectedItem();
                }

                if (_registeredListTypes.Contains(APIListType.GroupMenu))
                {
                    SendGroupList();
                    SendGroupSelectedItem();
                }

                if (_registeredListTypes.Contains(APIListType.Menu))
                {
                    SendMenuControlList();
                }
            }
        }

        private void GUIWindowManager_Receivers(GUIMessage message)
        {
            if (_registeredListTypes.Any())
            {
                switch (message.Message)
                {
                    case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                        if (_listFocused && !_isRecheckingListItems)
                        {
                            SendFacdeSelectedItem();
                            SendListSelectedItem();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
            if (_registeredListTypes.Any())
            {
                switch (action.wID)
                {
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_DOWN:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_LEFT:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_RIGHT:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_UP:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_ACTIONMENU:
                        CheckForListTypeChange();
                        break;
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOUSE_CLICK:
                        CheckForListItemChanges();
                        break;
                    default:
                        break;
                }
            }
        }

        private void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
        
            if (_registeredListTypes.Any())
            {
             
                if (tag.Equals("#facadeview.layout"))
                {
                    SendFacadeLayout();
                }

                if (tag.Equals("#highlightedbutton"))
                {
                    if (_registeredListTypes.Contains(APIListType.Menu))
                    {
                        SendMenuControlSelectedItem();
                    }
                }
            }
        }

       

        public void CheckForListItemChanges()
        {
            if (_registeredListTypes.Contains(APIListType.List))
            {
                if (!_isRecheckingListItems)
                {
                    _isRecheckingListItems = true;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        Thread.Sleep(500);
                        if (_facadeControls.Any())
                        {
                            CheckFacadeForChanges(WindowManager.Instance.CurrentWindow.GetID); 
                        }
                        else
                        {
                            CheckListsForChanges( ); 
                        }
                        _isRecheckingListItems = false;
                        if (_checkItemsNeedsRefresh)
                        {
                            _checkItemsNeedsRefresh = false;
                            CheckForListItemChanges();
                        }
                    });
                }
                else
                {
                    _checkItemsNeedsRefresh = true;
                }
            }
        }

        private void CheckForListTypeChange()
        {
            if (!_isRecheckingListType)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    _isRecheckingListType = true;
                    if (_registeredListTypes.Any())
                    {
                        if (_focusedControlId != WindowManager.Instance.CurrentWindowFocusedControlId)
                        {
                            _focusedControlId = WindowManager.Instance.CurrentWindowFocusedControlId;


                            if (_registeredListTypes.Contains(APIListType.GroupMenu))
                            {
                                if (!_groupFocused && _groupControls.Any(g => g.Children.GetControls().Any(c => c.Focus)))
                                {
                                    _groupFocused = true;
                                    SendGroupList();
                                    SendGroupSelectedItem();
                                    _isRecheckingListType = false;
                                    return;
                                }

                                if (_groupFocused && !_groupControls.Any(g => g.Children.GetControls().Any(c => c.Focus)))
                                {
                                    _groupFocused = false;
                                    SendList(APIListType.GroupMenu, APIListLayout.Vertical, new List<APIListItem>());
                                }

                                if (_groupFocused)
                                {
                                    SendGroupSelectedItem();
                                    _isRecheckingListType = false;
                                    return;
                                }
                            }

                            if (_registeredListTypes.Contains(APIListType.List))
                            {
                                _listFocused = _facadeControls.Any(f => f.Focus) || _listControls.Any(f => f.Focus) || _facadeControls.Any(f => f.Count > 0) || _listControls.Any(f => f.Count > 0);

                                if (_listFocused)
                                {
                                    _groupFocused = false;
                                    SendFacdeSelectedItem();
                                    SendListSelectedItem();
                                    _isRecheckingListType = false;
                                    return;
                                }
                            }

                          
                        }

                        SendFacadeLayout();

                       
                    }
                    _isRecheckingListType = false;
                });
            }
        }

        #region Facade


        private object _sync = new object();
        private void CheckFacadeForChanges(int WindowID)
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade != null)
            {
                int currentCount = -1;
                DateTime timeout = DateTime.Now.AddSeconds(30);
                while (WindowID == WindowManager.Instance.CurrentWindow.GetID && ( currentFacade.SelectedListItem == null || currentCount < 0 || currentCount != GetCount(currentFacade)))
                {
                    Log.Message(LogLevel.Debug, "Facade not ready, Waiting 250ms");
                    currentCount = GetCount(currentFacade);
                    Thread.Sleep(250);
                    if (DateTime.Now > timeout)
                    {
                        Log.Message(LogLevel.Debug, "Facade not ready, TIMEOUT");
                        break;
                    }
                }

                SendFacadeList();
                SendFacdeSelectedItem();
            }
        }

        private int GetCount(GUIFacadeControl facade)
        {
            try
            {
                int count = 0;
                 SupportedPluginManager.GUISafeInvoke(() =>
                    {
                       count =  ReflectionHelper.GetFieldValue<List<GUIListItem>>(facade, "_itemList", new List<GUIListItem>()).Count;
                    });
                 return count == 0 ? -1 : count;
            }
            catch (Exception)
            {
            }
            return -1;
        }

        private void SendFacadeList()
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus) ?? _facadeControls.FirstOrDefault(f => f.Count > 0);
            if (currentFacade != null)
            {
                var layout = GetAPIListLayout(currentFacade);
                SendList(APIListType.List, layout, GetAPIListItems(currentFacade, layout));
            }
        }

        private void SendFacdeSelectedItem()
        {
            var currentFacde = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacde != null && currentFacde.SelectedListItem != null)
            {
                SendSelectedItem(APIListType.List, currentFacde.SelectedListItem.Label, currentFacde.SelectedListItemIndex);

                SendSkinEditorData(currentFacde.SelectedListItem);
            }
        }

        private void SetFacdeSelectedItem(APIListAction item, bool isSelect)
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade == null)
            {
                foreach (var listcontrol in _facadeControls)
                {
                    if (listcontrol.Count > 0 && item.ItemIndex < listcontrol.Count && listcontrol[item.ItemIndex].Label == item.ItemText)
                    {
                        currentFacade = listcontrol;
                        break;
                    }
                }
            }

            if (currentFacade != null)
            {
                if (item.ItemIndex <= currentFacade.Count)
                {
                    currentFacade.Focus = true;
                    currentFacade.SelectedListItemIndex = item.ItemIndex;
                    if (isSelect)
                    {
                        SupportedPluginManager.GUISafeInvoke(() =>
                        {
                            currentFacade.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f));
                            CheckForListItemChanges();
                        });
                    }
                }
            }
        }

        private void SendFacadeLayout()
        {
            if (_registeredListTypes.Contains(APIListType.List))
            {
                var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
                if (currentFacade != null)
                {
                    var layout = GetAPIListLayout(currentFacade);
                    if (_currentLayout != layout)
                    {
                        _currentLayout = layout;
                        SendLayout(_currentLayout);
                    }
                }
            }
        }

        private void SendSkinEditorData(GUIListItem item)
        {
            if (item != null && MessageService.Instance.IsSkinEditorConnected)
            {
                try
                {
                    var data = new List<string[]>();
                    foreach (var property in item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)))
                    {
                        data.Add(new string[] { property.Name, (string)property.GetValue(item, null) });
                    }

                    if (item.TVTag != null)
                    {
                        foreach (var property in item.TVTag.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)))
                        {
                            data.Add(new string[] { "TVTag." + property.Name, (string)property.GetValue(item.TVTag, null) });
                        }
                    }

                    MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                    {
                        DataType = APISkinEditorDataType.ListItem,
                        ListItemData = data
                    });
                }
                catch 
                {
                                       
                }
            }
        }

        #endregion

        #region ListControl

        private void CheckListsForChanges()
        {
            var currentListControl = _listControls.FirstOrDefault(f => f.Focus);
            if (currentListControl != null)
            {
                DateTime timeout = DateTime.Now.AddSeconds(20);
                while (currentListControl.SelectedListItem == null && DateTime.Now < timeout)
                {
                    Thread.Sleep(250);
                    Log.Message(LogLevel.Debug, "ListControl not ready, Waiting 250ms");
                }
                Thread.Sleep(250);
                SendListControlList();
            }
        }


        private void SendListControlList()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus) ?? _listControls.FirstOrDefault(l => l.Count > 0);
            if (currentList != null)
            {
                SendList(APIListType.List, APIListLayout.Vertical, GetAPIListItems(currentList, APIListLayout.Vertical));
            }
        }


        private void SendListSelectedItem()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList != null && currentList.SelectedListItem != null)
            {
                SendSelectedItem(APIListType.List, currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);

                SendSkinEditorData(currentList.SelectedListItem);
            }
        }

        private void SetListSelectedItem(APIListAction item, bool isSelect)
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList != null)
            {
                if (item.ItemIndex <= currentList.Count)
                {
                    currentList.SelectedListItemIndex = item.ItemIndex;
                    if (isSelect)
                    {
                        SupportedPluginManager.GUISafeInvoke(() => 
                        {
                            currentList.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f));
                            CheckForListItemChanges();
                        });
                    }
                }
            }
        }

        #endregion

        #region Group

        private void SendGroupList()
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus)) ?? _groupControls.FirstOrDefault(g => g.Children.Count > 0);
            if (currentGroup != null)
            {
                SendList(APIListType.GroupMenu, APIListLayout.Vertical, GetAPIListItems(currentGroup));
            }
        }

        private void SetGroupSelectedItem(APIListAction item, bool isSelect)
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus));
            if (currentGroup != null)
            {
                var currentFocus = currentGroup.Children.GetControls().FirstOrDefault(c => c.Focus);
                if (currentFocus != null)
                {
                    currentFocus.Selected = false;
                    currentFocus.Focus = false;
                }

                foreach (var newFocus in currentGroup.Children.GetControls())
                {
                    if ((newFocus is GUIButtonControl && (newFocus as GUIButtonControl).Label == item.ItemText)
                        || (newFocus is GUIMenuButton && (newFocus as GUIMenuButton).SelectedItemLabel == item.ItemText)
                        || (newFocus is GUICheckButton && (newFocus as GUICheckButton).Label == item.ItemText))
                    {
                        newFocus.Selected = true;
                        newFocus.Focus = true;
                        if (isSelect)
                        {
                            SupportedPluginManager.GUISafeInvoke(() => newFocus.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f)));
                        }
                        break;
                    }
                }
            }
        }

        private void SendGroupSelectedItem()
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus));
            if (currentGroup != null)
            {
                string label = string.Empty;
                int index = 0;
                foreach (var item in currentGroup.Children.GetControls().Where(c => c is GUIButtonControl || c is GUIMenuButton || c is GUICheckButton))
                {
                    if (item.Focus)
                    {
                        if (item is GUIButtonControl)
                        {
                            label = (item as GUIButtonControl).Label;
                        }
                        if (item is GUIMenuButton)
                        {
                            label = (item as GUIMenuButton).SelectedItemLabel;
                        }
                        if (item is GUICheckButton)
                        {
                            label = (item as GUICheckButton).Label;
                        }
                        break;
                    }
                    index++;
                }
                if (!string.IsNullOrEmpty(label))
                {
                    SendSelectedItem(APIListType.GroupMenu, label, index);
                }
            }
        }

        #endregion

        #region MenuControl

        private void CheckMenuControlForChanges()
        {
            if (_menuControl != null)
            {
                SendMenuControlList();
            }
        }


        private void SendMenuControlList()
        {
            if (_menuControl != null)
            {
                SendList(APIListType.Menu, GetMenuControlLayout(_menuControl), GetAPIListItems(_menuControl));
            }
        }

        private APIListLayout GetMenuControlLayout(GUIMenuControl menuControl)
        {
            if (ReflectionHelper.GetFieldValue<bool>(menuControl, "_horizontal", false))
            {
                return APIListLayout.Horizontal;
            }
            return APIListLayout.Vertical;
        }
        
        //private int _lastMenuCotrolItemIndex = -1;
        private bool _movingMenuControl = false;
        private MethodInfo _menuCotrolMoveUp;
        private MethodInfo _menuCotrolMoveDown;
        private FieldInfo _menuControlButtonList;

        private void SetMenuControlSelectedItem(APIListAction item, bool isSelect)
        {
            if (_menuControl != null)
            {
                _movingMenuControl = true;
                if (item.ItemIndex <= _menuControl.ButtonInfos.Count)
                {
                    var buttonList = _menuControlButtonList.GetValue(_menuControl) as List<GUIButtonControl>;
                    if (buttonList != null)
                    {
                        var newIndex = buttonList.FindIndex(b => b.Label == item.ItemText);
                        if (newIndex != -1)
                        {
                            bool isMoveDown = newIndex > _menuControl.FocusedButton;
                            int moveCount = isMoveDown
                                ? newIndex - _menuControl.FocusedButton
                                : _menuControl.FocusedButton - newIndex;

                            for (int i = 0; i < moveCount; i++)
                            {
                                if (isMoveDown)
                                {
                                    if (_menuCotrolMoveDown != null)
                                    {
                                        SupportedPluginManager.GUISafeInvoke(() => _menuCotrolMoveDown.Invoke(_menuControl, null));
                                    }
                                }
                                else
                                {
                                    if (_menuCotrolMoveUp != null)
                                    {
                                        SupportedPluginManager.GUISafeInvoke(() => _menuCotrolMoveUp.Invoke(_menuControl, null));
                                    }
                                }
                            }

                            if (isSelect)
                            {
                                SupportedPluginManager.GUISafeInvoke(() => GUIWindowManager.ActivateWindow(_menuControl.ButtonInfos[item.ItemIndex].PluginID));
                            }
                        }
                    }
                }
                _movingMenuControl = false;
            }
        }

        private void SendMenuControlSelectedItem()
        {
            try
            {
                if (_menuControl != null && !_movingMenuControl)
                {
                    var menuButtons = _menuControlButtonList.GetValue(_menuControl) as List<GUIButtonControl>;
                    if (menuButtons != null)
                    {
                        SendSelectedItem(APIListType.Menu, menuButtons[_menuControl.FocusedButton].Label, -1);
                    }
                }
            }
            catch (Exception ex)
            {
             Log.Exception("Here",ex);
            }
        }

        #endregion

        #region Heplers

        public List<APIListItem> GetAPIListItems(GUIGroup actionMenu)
        {
            var returnValue = new List<APIListItem>();
            if (actionMenu != null)
            {
                int index = 0;
                foreach (var item in actionMenu.Children.GetControls())
                {
                    string label = string.Empty;
                    if (item is GUIButtonControl)
                    {
                        label = (item as GUIButtonControl).Label;
                    }
                    if (item is GUIMenuButton)
                    {
                        label = (item as GUIMenuButton).SelectedItemLabel;
                    }
                    if (item is GUICheckButton)
                    {
                        label = (item as GUICheckButton).Label;
                    }

                    if (!string.IsNullOrEmpty(label))
                    {
                        returnValue.Add(new APIListItem { Index = index, Label = label, });
                        index++;
                    }
                }
            }
            return returnValue;
        }



        public List<APIListItem> GetAPIListItems(GUIFacadeControl facade, APIListLayout layout)
        {
            var returnValue = new List<APIListItem>();
            if (facade != null)
            {
                var items = new List<GUIListItem>();
                for (int i = 0; i < facade.Count; i++)
                {
                    items.Add(facade[i]);
                }
              
                if (WindowManager.Instance.CurrentPlugin != null)
                {
                    returnValue = WindowManager.Instance.CurrentPlugin.GetListItems(items, layout);
                }

                if (!returnValue.Any() && items.Any())
                {
                    int index = 0;
                    foreach (var item in items)
                    {
                        returnValue.Add(new APIListItem
                        {
                            Index = index,
                            Label = item.Label,
                            Label2 = item.Label2,
                            Label3 = item.Label3,
                            Image = GetItemImageBytes(item, layout)
                        });
                        index++;
                    }
                }
            }
            return returnValue;
        }

        public List<APIListItem> GetAPIListItems(GUIListControl listcontrol, APIListLayout layout)
        {
            var returnValue = new List<APIListItem>();
            if (listcontrol != null)
            {


                var items = new List<GUIListItem>();
                for (int i = 0; i < listcontrol.Count; i++)
                {
                    items.Add(listcontrol[i]);
                }

                if (WindowManager.Instance.CurrentPlugin != null)
                {
                    returnValue = WindowManager.Instance.CurrentPlugin.GetListItems(items, layout);
                }

                if (!returnValue.Any() && items.Any())
                {
                    int index = 0;
                    foreach (var item in items)
                    {
                        returnValue.Add(new APIListItem
                        {
                            Index = index,
                            Label = item.Label,
                            Label2 = item.Label2,
                            Label3 = item.Label3,
                            Image = GetItemImageBytes(item, layout)
                        });
                        index++;
                    }
                }
            }
            return returnValue;
        }

        public List<APIListItem> GetAPIListItems(GUIMenuControl menuControl)
        {
            var returnValue = new List<APIListItem>();
            if (menuControl != null)
            {
                int index = 0;
                foreach (var button in menuControl.ButtonInfos)
                {
                    returnValue.Add(new APIListItem
                   {
                       Index = index,
                       Label = button.Text
                   });
                    index++;
                }
            }
            return returnValue;
        }

        private APIListLayout _currentLayout;

        public APIListLayout GetAPIListLayout(GUIFacadeControl facade)
        {
            if (facade != null)
            {
                switch (facade.CurrentLayout)
                {
                    case GUIFacadeControl.Layout.AlbumView:
                    case GUIFacadeControl.Layout.CoverFlow:
                        return APIListLayout.CoverFlow;
                    case GUIFacadeControl.Layout.Filmstrip:
                    case GUIFacadeControl.Layout.LargeIcons:
                    case GUIFacadeControl.Layout.SmallIcons:
                        return APIListLayout.Horizontal;
                    case GUIFacadeControl.Layout.List:
                    case GUIFacadeControl.Layout.Playlist:
                        return APIListLayout.Vertical;
                    default:
                        break;
                }
            }
            return APIListLayout.Vertical;
        }

        public APIImage GetItemImageBytes(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (item != null)
            {
                if (item.HasThumbnail)
                {
                    filename = item.ThumbnailImage;
                }
                else if (item.HasIconBig)
                {
                    filename = item.IconImageBig;
                }
                else if (item.HasIcon)
                {
                    filename = item.IconImage;
                }
                else if (item.HasPinIcon)
                {
                    filename = item.PinImage;
                }
                
            }

            return ImageHelper.CreateImage(filename);
        }

        #endregion

        #region Send Messages



        public void SendList(APIListType listType, APIListLayout layout, List<APIListItem> items)
        {
            int count = items != null ? items.Count() : 0;
            _currentBatchId++;
            if (listType == APIListType.List && count >= _settings.ListBatchThreshold)
            {
               
                int batchNo = 1;
                int batchCount = count < _settings.ListBatchSize ? 1 : ((count + _settings.ListBatchSize - 1) / _settings.ListBatchSize);
                for (int i = 0; i < count; i += _settings.ListBatchSize)
                {
                    MessageService.Instance.SendListMessage(new APIListMessage
                    {
                        MessageType = APIListMessageType.List,
                        List = new APIList
                        {
                            BatchNumber = batchNo,
                            BatchId = _currentBatchId,
                            BatchCount = batchCount,
                            ListType = APIListType.List,
                            ListItems = new List<APIListItem>(items.Skip(i).Take(_settings.ListBatchSize)),
                            ListLayout = layout
                        }
                    });
                    batchNo++;
                }
            }
            else
            {
             
                MessageService.Instance.SendListMessage(new APIListMessage
                {
                    MessageType = APIListMessageType.List,
                    List = new APIList
                    {
                        BatchId = _currentBatchId,
                        BatchCount = -1,
                        ListType = listType,
                        ListItems = new List<APIListItem>(items),
                        ListLayout = layout
                    }
                });
            }
            Log.Message(LogLevel.Debug, "[SendList] - ListType: {0}, ItemCount: {1}", listType, count);

        }

        private APIListAction _lastSelectedAction;

        public void SendSelectedItem(APIListType listType, string text, int index)
        {
            var action = new APIListAction
                {
                    ActionType = APIListActionType.SelectedItem,
                    ItemListType = listType,
                    ItemIndex = index,
                    ItemText = text
                };

            if (!action.IsEqual(_lastSelectedAction))
            {

                MessageService.Instance.SendListMessage(new APIListMessage
                {
                    MessageType = APIListMessageType.Action,
                    Action = action
                });
                _lastSelectedAction = action;
                Log.Message(LogLevel.Debug, "[SendSelectedItem] - ListType: {0}, Index: {1}, Text: {2}", listType, index, text);
            }
        }

        private void SendLayout(APIListLayout layout)
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.Action,
                Action = new APIListAction
                {
                    ActionType = APIListActionType.Layout,
                    ItemLayout = layout
                }
            });
            Log.Message(LogLevel.Debug, "[SendLayout] - Layout: {0}", layout);
        }

        #endregion



    
    }
}
