using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class ListManager
    {
       #region Singleton Implementation

        private static ListManager instance;

        private ListManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(ListManager));
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

        private MPDisplay.Common.Log.Log Log;
        private bool _groupFocused;
        private bool _listFocused;
        private bool _isRecheckingListItems;
        private bool _isRecheckingListType;
        private int _focusedControlId = -1;
        private List<APIListType> _registeredListTypes = new List<APIListType>();
        private List<GUIFacadeControl> _facadeControls = new List<GUIFacadeControl>();
        private List<GUIListControl> _listControls = new List<GUIListControl>();
        private List<GUIGroup> _groupControls = new List<GUIGroup>();
        private GUIMenuControl _menuControl;
        private PluginSettings _settings;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            GUIWindowManager.OnNewAction += GUIWindowManager_OnNewAction;
            GUIWindowManager.Receivers += GUIWindowManager_Receivers;
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;
        }

        public void Shutdown()
        {
            GUIWindowManager.OnNewAction -= GUIWindowManager_OnNewAction;
            GUIWindowManager.Receivers -= GUIWindowManager_Receivers;
            GUIPropertyManager.OnPropertyChanged -= GUIPropertyManager_OnPropertyChanged;
        }

        public void RegisterWindowListTypes(List<APIListType> list)
        {
            Log.Message(LogLevel.Verbose, "[RegisterWindowListTypes] - Registering MPDisplay skin list types...");
            _registeredListTypes = new List<APIListType>(list);
            foreach (var listType in _registeredListTypes)
            {
                Log.Message(LogLevel.Verbose, "[RegisterWindowListTypes] - Registering MPDisplay skin list type: {0}", listType);
            }
            Log.Message(LogLevel.Verbose, "[RegisterWindowListTypes] - Registering MPDisplay skin list types complete.");
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
                }

                if (_registeredListTypes.Contains(APIListType.GroupMenu))
                {
                    SendGroupList();
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

        public GUIListItem LastSelectedItem { get; set; }
        public int LastSelectedItemWindowId { get; set; }

        private void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
            var currentFacde = _facadeControls.FirstOrDefault(f => f.Focus);
             if (currentFacde != null && currentFacde.SelectedListItem != null)
             {
                 LastSelectedItem = currentFacde.SelectedListItem;
                 LastSelectedItemWindowId = currentFacde.WindowId;
             }

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
                    if (_registeredListTypes.Contains(APIListType.List))
                    {
                        var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
                        if (currentFacade != null)
                        {
                            SendLayout(GetAPIListLayout(currentFacade));
                        }
                    }
                }


                if (tag.Equals("#highlightedbutton"))
                {
                    if (_registeredListTypes.Contains(APIListType.Menu))
                    {
                        SendMenuControlSelectedItem(tagValue);
                    }
                }
            }
        }

        private void CheckForListItemChanges()
        {
            if (_registeredListTypes.Contains(APIListType.List))
            {
                if (!_isRecheckingListItems)
                {
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        _isRecheckingListItems = true;
                        Thread.Sleep(200);
                        CheckFacadeForChanges();
                        CheckListsForChanges();
                        _isRecheckingListItems = false;
                    });
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

                            if (_registeredListTypes.Contains(APIListType.List))
                            {
                                _listFocused = _facadeControls.Any(f => f.Focus) || _listControls.Any(f => f.Focus);

                                if (_listFocused)
                                {
                                    _groupFocused = false;
                                    SendFacdeSelectedItem();
                                    SendListSelectedItem();
                                    _isRecheckingListType = false;
                                    return;
                                }
                            }

                            if (_registeredListTypes.Contains(APIListType.GroupMenu))
                            {
                                if (!_groupFocused && _groupControls.Any(g => g.Children.GetControls().Any(c => c.Focus)))
                                {
                                    _groupFocused = true;
                                    SendGroupList();
                                }

                                if (_groupFocused && !_groupControls.Any(g => g.Children.GetControls().Any(c => c.Focus)))
                                {
                                    _groupFocused = false;
                                    SendList(APIListType.GroupMenu, APIListLayout.Vertical, new List<APIListItem>());
                                }

                                if (_groupFocused)
                                {
                                    SendGroupSelectedItem();
                                }
                            }
                        }
                    }
                    _isRecheckingListType = false;
                });
            }
        }

        #region Facade

        private void CheckFacadeForChanges()
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade != null)
            {
                DateTime timeout = DateTime.Now.AddSeconds(5);
                while (currentFacade.SelectedListItem == null && DateTime.Now < timeout)
                {
                    Log.Message(LogLevel.Verbose, "Facade not ready, Waiting 250ms");
                    Thread.Sleep(250);
                }
                SendFacadeList();
            }
        }

        private void SendFacadeList()
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade != null)
            {
                SendList(APIListType.List, GetAPIListLayout(currentFacade), GetAPIListItems(currentFacade));
            }
        }

        private void SendFacdeSelectedItem()
        {
            var currentFacde = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacde != null && currentFacde.SelectedListItem != null)
            {
                SendSelectedItem(APIListType.List, currentFacde.SelectedListItem.Label, currentFacde.SelectedListItemIndex);
            }
        }

        private void SetFacdeSelectedItem(APIListAction item, bool isSelect)
        {
            var currentFacade = _facadeControls.FirstOrDefault(f => f.Focus);
            if (currentFacade != null)
            {
                if (item.ItemIndex <= currentFacade.Count)
                {
                    currentFacade.SelectedListItemIndex = item.ItemIndex;
                    if (isSelect)
                    {
                        GUIGraphicsContext.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f));
                    }
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
                DateTime timeout = DateTime.Now.AddSeconds(5);
                while (currentListControl.SelectedListItem == null && DateTime.Now < timeout)
                {
                    Thread.Sleep(100);
                    Log.Message(LogLevel.Verbose, "ListControl not ready, Waiting 100ms");
                }
                SendListControlList();
            }
        }


        private void SendListControlList()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList != null)
            {
                SendList(APIListType.List, APIListLayout.Vertical, GetAPIListItems(currentList));
            }
        }


        private void SendListSelectedItem()
        {
            var currentList = _listControls.FirstOrDefault(f => f.Focus);
            if (currentList != null && currentList.SelectedListItem != null)
            {
                SendSelectedItem(APIListType.List, currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);
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
                        GUIGraphicsContext.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f));
                    }
                }
            }
        }

        #endregion

        #region Group

        private void SendGroupList()
        {
            var currentGroup = _groupControls.FirstOrDefault(g => g.Children.GetControls().Any(c => c.Focus));
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
                SendList(APIListType.Menu, APIListLayout.Vertical, GetAPIListItems(_menuControl));
            }
        }

        private void SetMenuControlSelectedItem(APIListAction item, bool isSelect)
        {
            if (_menuControl != null)
            {

            }
        }

        private void SendMenuControlSelectedItem(string text)
        {
            if (_menuControl != null)
            {
                SendSelectedItem(APIListType.Menu, text, -1);
            }
        }

        #endregion

        #region Heplers

        public IEnumerable<APIListItem> GetAPIListItems(GUIGroup actionMenu)
        {
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
                        yield return new APIListItem { Index = index, Label = label, };
                        index++;
                    }
                }
            }
        }



        public IEnumerable<APIListItem> GetAPIListItems(GUIFacadeControl facade)
        {
            if (facade != null)
            {
                for (int i = 0; i < facade.Count; i++)
                {
                    var facadeItem = facade[i];
                    yield return new APIListItem
                    {
                        Index = i,
                        Label = facadeItem.Label,
                        Label2 = facadeItem.Label2,
                        Label3 = facadeItem.Label3,
                        Image = GetItemImageBytes(facadeItem)
                    };
                }
            }
        }

        public IEnumerable<APIListItem> GetAPIListItems(GUIListControl listcontrol)
        {
            if (listcontrol != null)
            {
                for (int i = 0; i < listcontrol.Count; i++)
                {
                    var listItem = listcontrol[i];
                    yield return new APIListItem
                    {
                        Index = i,
                        Label = listItem.Label,
                        Label2 = listItem.Label2,
                        Label3 = listItem.Label3,
                        Image = GetItemImageBytes(listItem)
                    };
                }
            }
        }

        public IEnumerable<APIListItem> GetAPIListItems(GUIMenuControl menuControl)
        {
            if (menuControl != null)
            {
                int index = 0;
                foreach (var button in menuControl.ButtonInfos)
                {
                    yield return new APIListItem
                    {
                        Index = index,
                        Label = button.Text
                    };
                    index++;
                }
            }
        }

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

        public byte[] GetItemImageBytes(GUIListItem item)
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
            if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
            {
                try
                {
                    return File.ReadAllBytes(filename);
                }
                catch { }
            }
            return null;
        }

        #endregion

        #region Send Messages

        private void SendList(APIListType listType, APIListLayout layout, IEnumerable<APIListItem> items)
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.List,
                List = new APIList
                {
                    ListType = listType,
                    ListItems = new List<APIListItem>(items),
                    ListLayout = layout
                }
            });
            Log.Message(LogLevel.Debug, "[SendList] - ListType: {0}, ItemCount: {1}", listType, items != null ? items.Count() : 0);
        }

        private APIListAction _lastSelectedAction;

        private void SendSelectedItem(APIListType listType, string text, int index)
        {
            var action = new APIListAction
                {
                    ActionType = APIListActionType.SelectedItem,
                    ItemListType = listType,
                    ItemIndex = index,
                    ItemText = text
                };

            if (!action.Equals(_lastSelectedAction))
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
