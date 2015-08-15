using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using Action = MediaPortal.GUI.Library.Action;
using Log = Common.Log.Log;

namespace MediaPortalPlugin.InfoManagers
{
    public class ListManager
    {
       #region Singleton Implementation

        private static ListManager _instance;

        private ListManager()
        {
            _log = LoggingManager.GetLog(typeof(ListManager));
        }

        public static ListManager Instance
        {
            get { return _instance ?? (_instance = new ListManager()); }
        }

        #endregion

        private static Log _log;
        private static bool _groupFocused;
        private static bool _listFocused;
        private static bool _isRecheckingListItems;
        private static bool _isRecheckingListType;
        private static bool _checkItemsNeedsRefresh;
        private static int _focusedControlId = -1;
        private List<APIListType> _registeredListTypes = new List<APIListType>();
        private List<GUIFacadeControl> _facadeControls = new List<GUIFacadeControl>();
        private List<GUIListControl> _listControls = new List<GUIListControl>();
        private List<GUIGroup> _groupControls = new List<GUIGroup>();
        private GUIMenuControl _menuControl;
        private PluginSettings _settings;
        private static int _currentBatchId;

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
            _log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list types...");
            _registeredListTypes = new List<APIListType>(list.Distinct());
            foreach (var listType in _registeredListTypes)
            {
                _log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list type: {0}", listType);
            }
            _log.Message(LogLevel.Debug, "[RegisterWindowListTypes] - Registering MPDisplay skin list types complete.");
            RegisterWindowListTypes();
        }

   

        public void SetWindowListControls()
        {
            if (WindowManager.Instance.CurrentWindow == null) return;
            _facadeControls = new List<GUIFacadeControl>(WindowManager.Instance.CurrentWindow.GetControls<GUIFacadeControl>());
            _listControls = new List<GUIListControl>(WindowManager.Instance.CurrentWindow.GetControls<GUIListControl>());
            _groupControls = new List<GUIGroup>(WindowManager.Instance.CurrentWindow.GetControls<GUIGroup>()
                .Where(g => !g.Children.GetControls().Any(c => c is GUIFacadeControl || c is GUIListControl)));
            _menuControl = WindowManager.Instance.CurrentWindow.GetControls<GUIMenuControl>().FirstOrDefault();
            RegisterWindowListTypes();
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
            if (action == null) return;
            var isSelect = action.ActionType == APIListActionType.SelectedItem;
            if (action.ItemListType == APIListType.List)
            {
                SetFacadeSelectedItem(action, isSelect);
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

        private void RegisterWindowListTypes()
        {
            if (!_registeredListTypes.Any()) return;
            if (_registeredListTypes.Contains(APIListType.List))
            {
                SendFacadeList();
                SendListControlList();
                SendFacadeSelectedItem();
                ThreadPool.QueueUserWorkItem(o =>
                {
                    Thread.Sleep(2000);
                    SendFacadeLayout();
                 });

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

        private void GUIWindowManager_Receivers(GUIMessage message)
        {
            if (!_registeredListTypes.Any()) return;
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    if (_listFocused && !_isRecheckingListItems)
                    {
                        SendFacadeSelectedItem();
                        SendListSelectedItem();
                    }
                    break;
            }
        }

        private void GUIWindowManager_OnNewAction(Action action)
        {
            if (!_registeredListTypes.Any()) return;
            switch (action.wID)
            {
                case Action.ActionType.ACTION_MOVE_DOWN:
                case Action.ActionType.ACTION_MOVE_LEFT:
                case Action.ActionType.ACTION_MOVE_RIGHT:
                case Action.ActionType.ACTION_MOVE_UP:
                case Action.ActionType.ACTION_SHOW_ACTIONMENU:
                    CheckForListTypeChange();
                    break;
                case Action.ActionType.ACTION_SELECT_ITEM:
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                case Action.ActionType.ACTION_MOUSE_CLICK:
                    CheckForListItemChanges();
                    break;
            }
        }

        private void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (!_registeredListTypes.Any()) return;
            if (tag.Equals("#facadeview.layout"))
            {
                SendFacadeLayout();
                if (WindowManager.Instance.CurrentPlugin != null && !GUIWindowManager.IsSwitchingToNewWindow)
                {
                    if (WindowManager.Instance.CurrentPlugin.MustResendListOnLayoutChange())
                    {
                           var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
                            if (currentFacade != null)
                            {
                                var currentCount = -1;
                                var timeout = DateTime.Now.AddSeconds(10);
                                while (!GUIWindowManager.IsSwitchingToNewWindow && (currentCount <= 0 || currentCount != GetCount(currentFacade)))
                                {
                                    _log.Message(LogLevel.Debug, "List not ready, Waiting 250ms");
                                    currentCount = GetCount(currentFacade);
                                    Thread.Sleep(250);
                                    if (DateTime.Now <= timeout) continue;
                                    _log.Message(LogLevel.Debug, "List still not ready, TIMEOUT");
                                    break;
                                }
                        }
                        SendFacadeList();
                    }
                }            
            }

            if (!tag.Equals("#highlightedbutton")) return;
            if (_registeredListTypes.Contains(APIListType.Menu))
            {
                SendMenuControlSelectedItem();
            }
        }

       
        public void CheckForListItemChanges()
        {
            if (!_registeredListTypes.Contains(APIListType.List)) return;
            if (!_isRecheckingListItems)
            {
                _isRecheckingListItems = true;
                ThreadPool.QueueUserWorkItem(o =>
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
                    if (!_checkItemsNeedsRefresh) return;

                    _checkItemsNeedsRefresh = false;
                    CheckForListItemChanges();
                });
            }
            else
            {
                _checkItemsNeedsRefresh = true;
            }
        }

        private void CheckForListTypeChange()
        {
            if (!_isRecheckingListType)
            {
                ThreadPool.QueueUserWorkItem(o =>
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
                                    SendFacadeSelectedItem();
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

        private void CheckFacadeForChanges(int windowId)
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade == null) return;

            var currentCount = -1;
            var timeout = DateTime.Now.AddSeconds(30);
            while (windowId == WindowManager.Instance.CurrentWindow.GetID && ( currentFacade.SelectedListItem == null || currentCount < 0 || currentCount != GetCount(currentFacade)))
            {
                _log.Message(LogLevel.Debug, "Facade not ready, Waiting 250ms");
                currentCount = GetCount(currentFacade);
                Thread.Sleep(250);
                if (DateTime.Now <= timeout) continue;
                _log.Message(LogLevel.Debug, "Facade not ready, TIMEOUT");
                break;
            }
            SendFacadeList();
            SendFacadeSelectedItem();
        }

        private static int GetCount(GUIFacadeControl facade)
        {
            if (facade == null) return -1;

            try
            {
                var count = 0;
                 SupportedPluginManager.GuiSafeInvoke(() =>
                    {
                       count =  ReflectionHelper.GetFieldValue(facade, "_itemList", new List<GUIListItem>()).Count;
                    });
                 return count == 0 ? -1 : count;
            }
            catch (Exception ex)
            {
                _log.Exception("[GetCount] - An exception occured getting count for GUIFacadeControl {0}", ex, facade.ToString());
            }
            return -1;
        }

        private void SendFacadeList()
        {
            var currentFacade = (_facadeControls.FirstOrDefault(f => f.Focus) ??
                                 _facadeControls.FirstOrDefault(f => f.Count > 0)) ?? _facadeControls.FirstOrDefault();
 
            if (currentFacade == null) return;
            var layout = GetApiListLayout(currentFacade);
            SendList(APIListType.List, layout, GetApiListItems(currentFacade, layout));
        }

        private void SendFacadeSelectedItem()
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade == null || currentFacade.SelectedListItem == null) return;

            SendSelectedItem(APIListType.List, currentFacade.SelectedListItem.Label, currentFacade.SelectedListItemIndex);

            SendSkinEditorData(currentFacade.SelectedListItem);
        }

        private void SetFacadeSelectedItem(APIListAction item, bool isSelect)
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade == null)
            {
                foreach (var listcontrol in _facadeControls.Where(listcontrol => listcontrol.Count > 0 && item.ItemIndex < listcontrol.Count && listcontrol[item.ItemIndex].Label == item.ItemText))
                {
                    currentFacade = listcontrol;
                    break;
                }
            }

            if (currentFacade == null) return;

            if (item.ItemIndex > currentFacade.Count) return;

            currentFacade.Focus = true;
            currentFacade.SelectedListItemIndex = item.ItemIndex;
            if (isSelect)
            {
                SupportedPluginManager.GuiSafeInvoke(() =>
                {
                    currentFacade.OnAction(new Action((Action.ActionType)7, 0f, 0f));
                    CheckForListItemChanges();
                });
            }
        }

        private void SendFacadeLayout()
        {
            if (!_registeredListTypes.Contains(APIListType.List)) return;

            var currentFacade = (_facadeControls.FirstOrDefault(f => f.Focus) ??
                                 _facadeControls.FirstOrDefault(f => f.Count > 0)) ??
                                _facadeControls.FirstOrDefault();

            if (currentFacade == null) return;

            var layout = GetApiListLayout(currentFacade);
            if (_currentLayout == layout) return;

            _currentLayout = layout;
            SendLayout(_currentLayout);
        }

        private static void SendSkinEditorData(GUIListItem item)
        {
            if (item == null || !MessageService.Instance.IsSkinEditorConnected) return;

            try
            {
                var data = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof (string)).Select(property => new[] {property.Name,
                    (string) property.GetValue(item, null)}).ToList();

                if (item.TVTag != null)
                {
                    data.AddRange(item.TVTag.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof (string)).Select(property => new[] {"TVTag." + property.Name,
                        (string) property.GetValue(item.TVTag, null)}));
                }

                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = APISkinEditorDataType.ListItem,
                    ListItemData = data
                });
            }
            catch( Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendSkinEditorData] - An exception occured sendind data for GUIListItem <{0}>. Exception: {1}", item, ex.Message);
            }
        }

        #endregion

        #region ListControl

        private void CheckListsForChanges()
        {
            var currentListControl = _listControls.FirstOrDefault(f => f.Focus);
            if (currentListControl == null) return;

            var timeout = DateTime.Now.AddSeconds(20);
            while (currentListControl.SelectedListItem == null && DateTime.Now < timeout)
            {
                Thread.Sleep(250);
                _log.Message(LogLevel.Debug, "ListControl not ready, Waiting 250ms");
            }
            Thread.Sleep(250);
            SendListControlList();
        }


        private void SendListControlList()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus) ?? _listControls.FirstOrDefault(l => l.Count > 0);
            if (currentList != null)
            {
                SendList(APIListType.List, APIListLayout.Vertical, GetApiListItems(currentList, APIListLayout.Vertical));
            }
        }


        private void SendListSelectedItem()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList == null || currentList.SelectedListItem == null) return;

            SendSelectedItem(APIListType.List, currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);

            SendSkinEditorData(currentList.SelectedListItem);
        }

        private void SetListSelectedItem(APIListAction item, bool isSelect)
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList == null) return;

            if (item.ItemIndex > currentList.Count) return;

            currentList.SelectedListItemIndex = item.ItemIndex;
            if (isSelect)
            {
                SupportedPluginManager.GuiSafeInvoke(() => 
                {
                    currentList.OnAction(new Action((Action.ActionType)7, 0f, 0f));
                    CheckForListItemChanges();
                });
            }
        }

        #endregion

        #region Group

        private void SendGroupList()
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus)) ?? _groupControls.FirstOrDefault(g => g.Children.Count > 0);
            if (currentGroup != null)
            {
                SendList(APIListType.GroupMenu, APIListLayout.Vertical, GetApiListItems(currentGroup));
            }
        }

        private void SetGroupSelectedItem(APIListAction item, bool isSelect)
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus));
            if (currentGroup == null) return;

            var currentFocus = currentGroup.Children.GetControls().FirstOrDefault(c => c.Focus);
            if (currentFocus != null)
            {
                currentFocus.Selected = false;
                currentFocus.Focus = false;
            }

            foreach (var newFocus in currentGroup.Children.GetControls().Where(newFocus => (newFocus is GUIButtonControl && ((GUIButtonControl) newFocus).Label == item.ItemText) ||
                (newFocus is GUIMenuButton && ((GUIMenuButton) newFocus).SelectedItemLabel == item.ItemText) ||
                (newFocus is GUICheckButton && ((GUICheckButton) newFocus).Label == item.ItemText)))
            {
                newFocus.Selected = true;
                newFocus.Focus = true;
                if (isSelect)
                {
                    var focus = newFocus;
                    SupportedPluginManager.GuiSafeInvoke(() => focus.OnAction(new Action((Action.ActionType)7, 0f, 0f)));
                }
                break;
            }
        }

        private void SendGroupSelectedItem()
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus));
            if (currentGroup == null) return;

            var label = string.Empty;
            var index = 0;
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
            if (label != null)
            {
                SendSelectedItem(APIListType.GroupMenu, label, index);
            }
        }

        #endregion

        #region MenuControl

        private void SendMenuControlList()
        {
            if (_menuControl != null)
            {
                SendList(APIListType.Menu, GetMenuControlLayout(_menuControl), GetApiListItems(_menuControl));
            }
        }

        private static APIListLayout GetMenuControlLayout(GUIMenuControl menuControl)
        {
            return ReflectionHelper.GetFieldValue(menuControl, "_horizontal", false) ? APIListLayout.Horizontal : APIListLayout.Vertical;
        }

        //private int _lastMenuCotrolItemIndex = -1;
        private bool _movingMenuControl;
        private MethodInfo _menuCotrolMoveUp;
        private MethodInfo _menuCotrolMoveDown;
        private FieldInfo _menuControlButtonList;

        private void SetMenuControlSelectedItem(APIListAction item, bool isSelect)
        {
            if (_menuControl == null) return;

            _movingMenuControl = true;
            if (item.ItemIndex <= _menuControl.ButtonInfos.Count)
            {
                var buttonList = _menuControlButtonList.GetValue(_menuControl) as List<GUIButtonControl>;
                if (buttonList != null)
                {
                    var newIndex = buttonList.FindIndex(b => b.Label == item.ItemText);
                    if (newIndex != -1)
                    {
                        var isMoveDown = newIndex > _menuControl.FocusedButton;
                        var moveCount = isMoveDown
                            ? newIndex - _menuControl.FocusedButton
                            : _menuControl.FocusedButton - newIndex;

                        for (var i = 0; i < moveCount; i++)
                        {
                            if (isMoveDown)
                            {
                                if (_menuCotrolMoveDown != null)
                                {
                                    SupportedPluginManager.GuiSafeInvoke(() => _menuCotrolMoveDown.Invoke(_menuControl, null));
                                }
                            }
                            else
                            {
                                if (_menuCotrolMoveUp != null)
                                {
                                    SupportedPluginManager.GuiSafeInvoke(() => _menuCotrolMoveUp.Invoke(_menuControl, null));
                                }
                            }
                        }

                        if (isSelect)
                        {
                            SupportedPluginManager.GuiSafeInvoke(() => GUIWindowManager.ActivateWindow(_menuControl.ButtonInfos[item.ItemIndex].PluginID));
                        }
                    }
                }
            }
            _movingMenuControl = false;
        }

        private void SendMenuControlSelectedItem()
        {
            try
            {
                if (_menuControl == null || _movingMenuControl) return;
                var menuButtons = _menuControlButtonList.GetValue(_menuControl) as List<GUIButtonControl>;
                if (menuButtons == null) return;
                if (_menuControl.FocusedButton > -1 && _menuControl.FocusedButton < menuButtons.Count)
                {
                    SendSelectedItem(APIListType.Menu, menuButtons[_menuControl.FocusedButton].Label, -1);
                }
            }
            catch (Exception ex)
            {
             _log.Exception("Exception SendMenuControlSelectedItem(): ",ex);
            }
        }

        #endregion

        #region Helpers

        public List<APIListItem> GetApiListItems(GUIGroup actionMenu)
        {
            var returnValue = new List<APIListItem>();
            if (actionMenu == null) return returnValue;
            var index = 0;
            foreach (var item in actionMenu.Children.GetControls())
            {
                var label = string.Empty;
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

                if (label == null) continue;
                returnValue.Add(new APIListItem { Index = index, Label = label });
                index++;
            }
            return returnValue;
        }



        public List<APIListItem> GetApiListItems(GUIFacadeControl facade, APIListLayout layout)
        {
            var returnValue = new List<APIListItem>();
            if (facade == null) return returnValue;

            var items = new List<GUIListItem>();
            for (var i = 0; i < facade.Count; i++)
            {
                items.Add(facade[i]);
            }
              
            if (WindowManager.Instance.CurrentPlugin != null)
            {
                returnValue = WindowManager.Instance.CurrentPlugin.GetListItems(items, layout);
            }

            if (returnValue.Any() || !items.Any()) return returnValue;

            var index = 0;
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
            return returnValue;
        }

        public List<APIListItem> GetApiListItems(GUIListControl listcontrol, APIListLayout layout)
        {
            var returnValue = new List<APIListItem>();
            if (listcontrol == null) return returnValue;

            var items = new List<GUIListItem>();
            for (var i = 0; i < listcontrol.Count; i++)
            {
                items.Add(listcontrol[i]);
            }

            if (WindowManager.Instance.CurrentPlugin != null)
            {
                returnValue = WindowManager.Instance.CurrentPlugin.GetListItems(items, layout);
            }

            if (returnValue.Any() || !items.Any()) return returnValue;

            var index = 0;
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
            return returnValue;
        }

        public List<APIListItem> GetApiListItems(GUIMenuControl menuControl)
        {
            var returnValue = new List<APIListItem>();
            if (menuControl == null) return returnValue;

            var index = 0;
            foreach (var button in menuControl.ButtonInfos)
            {
                returnValue.Add(new APIListItem
                {
                    Index = index,
                    Label = button.Text
                });
                index++;
            }
            return returnValue;
        }

        private APIListLayout _currentLayout;

        // get APILayout from confuguration string
        private static APIListLayout GetApiListLayout(string layout)
        {
            switch (layout)
            {
                case "Coverflow":
                    return APIListLayout.CoverFlow;
                case "Vertical":
                    return APIListLayout.Vertical;
                case "VerticalIcon":
                    return APIListLayout.VerticalIcon;
                case "Horizontal":
                    return APIListLayout.Horizontal;
            }
            return APIListLayout.Vertical;

        }

        public APIListLayout GetApiListLayout(GUIFacadeControl facade)
        {
            if (facade == null) return APIListLayout.Vertical;

            // get layout for plugins from plugin settings
            if (WindowManager.Instance.CurrentPlugin != null)
            {
                switch (facade.CurrentLayout)
                {
                    case GUIFacadeControl.Layout.AlbumView:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutAlbumview);
                    case GUIFacadeControl.Layout.CoverFlow:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutCoverflow);
                    case GUIFacadeControl.Layout.Filmstrip:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutFilmstrip);
                    case GUIFacadeControl.Layout.SmallIcons:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutSmallIcons);
                    case GUIFacadeControl.Layout.List:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutList);
                    case GUIFacadeControl.Layout.Playlist:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutPlaylist);
                    case GUIFacadeControl.Layout.LargeIcons:
                        return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutLargeIcons);
                }
            }
            else
            {
                switch (facade.CurrentLayout)
                {
                    case GUIFacadeControl.Layout.AlbumView:
                    case GUIFacadeControl.Layout.CoverFlow:
                        return APIListLayout.CoverFlow;
                    case GUIFacadeControl.Layout.Filmstrip:
                    case GUIFacadeControl.Layout.SmallIcons:
                    case GUIFacadeControl.Layout.LargeIcons:
                        return APIListLayout.Horizontal;
                    case GUIFacadeControl.Layout.List:
                    case GUIFacadeControl.Layout.Playlist:
                        return APIListLayout.Vertical;
                }
            }
            return APIListLayout.Vertical;
        }

        public APIImage GetItemImageBytes(GUIListItem item, APIListLayout layout)
        {
            var filename = string.Empty;
            if (item == null) return ImageHelper.CreateImage(filename);

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
            return ImageHelper.CreateImage(filename);
        }

        #endregion

        #region Send Messages



        public void SendList(APIListType listType, APIListLayout layout, List<APIListItem> items)
        {
            var count = items != null ? items.Count() : 0;
            _currentBatchId++;
            if (listType == APIListType.List && count >= _settings.ListBatchThreshold)
            {
               
                var batchNo = 1;
                var batchCount = count < _settings.ListBatchSize ? 1 : ((count + _settings.ListBatchSize - 1) / _settings.ListBatchSize);
                for (var i = 0; i < count; i += _settings.ListBatchSize)
                {
                    if (items != null)
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
                if (items != null)
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
            _log.Message(LogLevel.Debug, "[SendList] - ListType: {0}, ItemCount: {1}, Layout: {2}", listType, count, layout);

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

            if (action.IsEqual(_lastSelectedAction)) return;

            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.Action,
                Action = action
            });
            _lastSelectedAction = action;
            _log.Message(LogLevel.Debug, "[SendSelectedItem] - ListType: {0}, Index: {1}, Text: {2}", listType, index, text);
        }

        private static void SendLayout(APIListLayout layout)
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
            _log.Message(LogLevel.Debug, "[SendLayout] - Layout: {0}", layout);
        }

        #endregion


    }
}
