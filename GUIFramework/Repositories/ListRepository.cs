﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common.Log;
using Common.MessengerService;
using Common.Settings;
using GUIFramework.GUI;
using GUIFramework.Managers;
using GUIFramework.Utils;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIFramework.Repositories
{
    public class ListRepository : IRepository
    {
        #region Singleton Implementation

        private ListRepository()
        {
            _log = LoggingManager.GetLog(typeof(ListRepository));
        }

        private static ListRepository _instance;

        public static ListRepository Instance => _instance ?? (_instance = new ListRepository());

        public static void RegisterMessage<T>(ListServiceMessage message, Action<T> callback)
        {
            Instance._listService.Register(message, callback);
        }

        public static void DeregisterMessage(ListServiceMessage message,object owner)
        {
            Instance._listService.Deregister(message, owner);
        }

        public static void RegisterListMessage(GUIList listcontrol, XmlListType listType)
        {
            Instance.RegisterList(listcontrol, listType);
        }

        public static void DeregisterListMessage(GUIList listcontrol, XmlListType listType)
        {
            Instance.DeregisterList(listcontrol, listType);
        }

        public static Task<List<APIListItem>> GetCurrentListItems(XmlListType xmlListType)
        {
            return Instance.GetListItems(xmlListType);
        }

        public static XmlListLayout GetCurrentMediaPortalListLayout(XmlListType xmlListType = XmlListType.MediaPortalListControl)
        {
            return Instance.GetMediaPortalListLayout(xmlListType);
        }

        public static Task<APIListAction> GetCurrentSelectedListItem(XmlListType xmlListType)
        {
            return Instance.GetSelectedListItem(xmlListType);
        }

        #endregion

        private readonly MessengerService<ListServiceMessage> _listService = new MessengerService<ListServiceMessage>();
        private DataRepository<XmlListType, List<APIListItem>> _listRepository;
        private DataRepository<XmlListType, APIListAction> _listSelectionRepository;
        private APIListLayout _mediaPortalListLayout;
        private readonly Log _log;

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _listRepository = new DataRepository<XmlListType, List<APIListItem>>((v1, v2) => v1.IsEqual(v2));
            _listSelectionRepository = new DataRepository<XmlListType, APIListAction>((v1, v2) => v1.IsEqual(v2));
        }

        public void ClearRepository()
        {
            _listRepository.ClearRepository();
            _listSelectionRepository.ClearRepository();
        }

        public void ResetRepository()
        {
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }

        public void AddListData(APIListMessage message)
        {
            if (message == null) return;

            if (message.MessageType == APIListMessageType.List && message.List != null)
            {
                switch (message.List.ListType)
                {
                    case APIListType.List:

                        ProcessBatch(message.List);
                        break;
                    case APIListType.Menu:
                        AddList(XmlListType.MediaPortalMenuControl, message.List.ListItems);
                        break;
                    case APIListType.GroupMenu:
                        AddList(XmlListType.MediaPortalButtonGroup, message.List.ListItems);
                        break;
                    case APIListType.DialogList:
                        AddList(XmlListType.MediaPortalDialogList, message.List.ListItems);
                        break;
                }
            }
            else if (message.MessageType == APIListMessageType.Action && message.Action != null)
            {
                switch (message.Action.ActionType)
                {
                    case APIListActionType.SelectedItem:
                        switch (message.Action.ItemListType)
                        {
                            case APIListType.List:
                                AddListSelection(XmlListType.MediaPortalListControl, message.Action);
                                break;
                            case APIListType.Menu:
                                AddListSelection(XmlListType.MediaPortalMenuControl, message.Action);
                                break;
                            case APIListType.GroupMenu:
                                AddListSelection(XmlListType.MediaPortalButtonGroup, message.Action);
                                break;
                            case APIListType.DialogList:
                                AddListSelection(XmlListType.MediaPortalDialogList, message.Action);
                                break;
                        }
                        break;

                    case APIListActionType.Layout:
                        _mediaPortalListLayout = message.Action.ItemLayout;
                        NotifyListLayoutChanged();
                        break;
                }
            }
        }

        private readonly SortedDictionary<int, List<APIListItem>> _data = new SortedDictionary<int, List<APIListItem>>();
        private int _currentBatchId = -1;
        private readonly object _syncObject = new object();

        private void ProcessBatch(APIList message)
        {
            try
            {
                if (message.BatchCount == -1)
                {
                    AddList(XmlListType.MediaPortalListControl, message.ListItems);
                    _mediaPortalListLayout = message.ListLayout;
                    NotifyListLayoutChanged();
                    return;
                }

                lock (_syncObject)
                {
                    if (message.BatchId > _currentBatchId)
                    {
                        _data.Clear();
                        _currentBatchId = message.BatchId;
                    }

                    if (message.BatchId != _currentBatchId) return;

                    _data.Add(message.BatchNumber, message.ListItems);
                    if (_data.Count != message.BatchCount) return;

                    AddList(XmlListType.MediaPortalListControl, _data.Values.SelectMany(k => k).ToList());
                    _mediaPortalListLayout = message.ListLayout;
                    NotifyListLayoutChanged();
                }
            }
            catch( Exception ex)
            {
                _log.Message(LogLevel.Error,
                    $"Error sending list, BatchId: {message.BatchId}, Count: {message.BatchCount}, Exception: {ex}");
            }
        }


        /// <summary>
        /// Adds an APIProperty property.
        /// </summary>
        /// <param name="listType">Type of the list.</param>
        /// <param name="list">The list.</param>
        public void AddList(XmlListType listType, List<APIListItem> list)
        {
            if (!_listRepository.AddOrUpdate(listType, list)) return;

            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Count", list?.Count.ToString() ?? "0");
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem", string.Empty);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem2", string.Empty);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem3", string.Empty);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selectedindex", string.Empty);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selectedthumb", default(byte[]));
            NotifyListChanged(listType);
        }

        public void AddListSelection(XmlListType listType, APIListAction action)
        {
            if (_listSelectionRepository.AddOrUpdate(listType, action))
            {
                NotifyListSelectionChanged(listType);
            }
        }

        public Task<List<APIListItem>> GetListItems(XmlListType listType)
        {
            return Task.Factory.StartNew(() => _listRepository.GetValue(listType));
        }

        public Task<APIListAction> GetSelectedListItem(XmlListType listType)
        {
            return Task.Factory.StartNew(() => _listSelectionRepository.GetValue(listType));
        }

        public XmlListLayout GetMediaPortalListLayout(XmlListType listType)
        {
            if (listType != XmlListType.MediaPortalListControl) return XmlListLayout.Vertical;

            switch (_mediaPortalListLayout)
            {
                case APIListLayout.Vertical:
                    return XmlListLayout.Vertical;
                case APIListLayout.VerticalIcon:
                    return XmlListLayout.VerticalIcon;
                case APIListLayout.Horizontal:
                    return XmlListLayout.Horizontal;
                case APIListLayout.CoverFlow:
                    return XmlListLayout.CoverFlow;
            }
            return XmlListLayout.Vertical;
        }

        public void RegisterList(GUIList listcontrol, XmlListType listType)
        {
            switch (listType)
            {
                case XmlListType.MediaPortalListControl:
                    _listService.Register(ListServiceMessage.ListItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.ListItemSelect, listcontrol.OnSelectedItemReceived);
                    _listService.Register(ListServiceMessage.ListItemLayout,  listcontrol.OnListLayoutReceived);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    _listService.Register(ListServiceMessage.GroupItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.GroupItemSelect, listcontrol.OnSelectedItemReceived);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    _listService.Register(ListServiceMessage.MenuItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.MenuItemSelect, listcontrol.OnSelectedItemReceived);
                    break;
                case XmlListType.MediaPortalDialogList:
                    _listService.Register(ListServiceMessage.DialogItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.DialogSelect, listcontrol.OnSelectedItemReceived);
                    break;
                case XmlListType.MPDisplaySkins:
                    _listService.Register(ListServiceMessage.SkinItems, listcontrol.OnPropertyChanging);
                    _listService.Register(ListServiceMessage.SkinItemSelect, listcontrol.OnSelectedItemReceived);
                    break;
                case XmlListType.MPDisplayStyles:
                    _listService.Register(ListServiceMessage.StyleItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.StyleItemSelect, listcontrol.OnSelectedItemReceived);
                    break;
                case XmlListType.MPDisplayLanguages:
                    _listService.Register(ListServiceMessage.LanguageItems, listcontrol.OnListItemsReceived);
                    _listService.Register(ListServiceMessage.LanguageItemSelect, listcontrol.OnSelectedItemReceived);
                    break;
            }
        }

        public void DeregisterList(GUIList control, XmlListType listType)
        {
            switch (listType)
            {
                case XmlListType.MediaPortalListControl:
                    _listService.Deregister(ListServiceMessage.ListItems, control);
                    _listService.Deregister(ListServiceMessage.ListItemSelect, control);
                    _listService.Deregister(ListServiceMessage.ListItemLayout, control);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    _listService.Deregister(ListServiceMessage.GroupItems, control);
                    _listService.Deregister(ListServiceMessage.GroupItemSelect, control);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    _listService.Deregister(ListServiceMessage.MenuItems, control);
                    _listService.Deregister(ListServiceMessage.MenuItemSelect, control);
                    break;
                case XmlListType.MediaPortalDialogList:
                    _listService.Deregister(ListServiceMessage.DialogItems, control);
                    _listService.Deregister(ListServiceMessage.DialogSelect, control);
                    break;
                case XmlListType.MPDisplaySkins:
                    _listService.Deregister(ListServiceMessage.SkinItems, control);
                    _listService.Deregister(ListServiceMessage.SkinItemSelect, control);
                    break;
                case XmlListType.MPDisplayStyles:
                    _listService.Deregister(ListServiceMessage.StyleItems, control);
                    _listService.Deregister(ListServiceMessage.StyleItemSelect, control);
                    break;
                case XmlListType.MPDisplayLanguages:
                    _listService.Deregister(ListServiceMessage.LanguageItems, control);
                    _listService.Deregister(ListServiceMessage.LanguageItemSelect, control);
                    break;
            }
        }


        public void FocusListControlItem(GUIList listcontrol, APIListItem item)
        {
            if (listcontrol == null || item == null) return;

            switch (listcontrol.ListType)
            {
                case XmlListType.MediaPortalListControl:
                    SendListAction(APIListActionType.FocusedItem, APIListType.List, item);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    SendListAction(APIListActionType.FocusedItem, APIListType.GroupMenu, item);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    SendListAction(APIListActionType.FocusedItem, APIListType.Menu, item);
                    break;
                case XmlListType.MediaPortalDialogList:
                    SendListAction(APIListActionType.FocusedItem, APIListType.DialogList, item);
                    break;
            }

            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem", item.Label);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem2", item.Label2);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selecteditem3", item.Label3);
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selectedindex", item.Index.ToString());
            PropertyRepository.Instance.AddProperty("#MPD.ListControl.Selectedthumb", item.Image.ToImageBytes());
        }

        public void SelectListControlItem(GUIList listcontrol, APIListItem item)
        {
            if (listcontrol == null || item == null) return;

            switch (listcontrol.ListType)
            {
                case XmlListType.None:
                    break;
                case XmlListType.MediaPortalListControl:
                    SendListAction(APIListActionType.SelectedItem, APIListType.List, item);
                    break;
                case XmlListType.MediaPortalButtonGroup:
                    SendListAction(APIListActionType.SelectedItem, APIListType.GroupMenu, item);
                    break;
                case XmlListType.MediaPortalMenuControl:
                    SendListAction(APIListActionType.SelectedItem, APIListType.Menu, item);
                    break;
                case XmlListType.MediaPortalDialogList:
                    SendListAction(APIListActionType.SelectedItem, APIListType.DialogList, item);
                    break;
                case XmlListType.MPDisplaySkins:
                    break;
                case XmlListType.MPDisplayStyles:
                    SkinInfo.SetStyle(item.Label);
                    break;
                case XmlListType.MPDisplayLanguages:
                    SkinInfo.SetLanguage(item.Label);
                    break;
            }
        }

        private void SendListAction(APIListActionType actionType, APIListType listType, APIListItem item)
        {
            var action = new APIListAction
                     {
                         ActionType = actionType,
                         ItemListType = listType,
                         ItemText = item.Label,
                         ItemIndex = item.Index
                     };


            _listService.NotifyListeners(ListServiceMessage.SendItem, action);
        }



        private void NotifyListChanged(XmlListType listType)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                switch (listType)
                {
                    case XmlListType.None:
                        break;
                    case XmlListType.MediaPortalListControl:
                        _listService.NotifyListeners(ListServiceMessage.ListItems);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        _listService.NotifyListeners(ListServiceMessage.GroupItems);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        _listService.NotifyListeners(ListServiceMessage.MenuItems);
                        break;
                    case XmlListType.MediaPortalDialogList:
                        _listService.NotifyListeners(ListServiceMessage.DialogItems);
                        break;
                    case XmlListType.MPDisplaySkins:
                        _listService.NotifyListeners(ListServiceMessage.SkinItems);
                        break;
                    case XmlListType.MPDisplayStyles:
                        _listService.NotifyListeners(ListServiceMessage.StyleItems);
                        break;
                    case XmlListType.MPDisplayLanguages:
                        _listService.NotifyListeners(ListServiceMessage.LanguageItems);
                        break;
                }
            });
        }

        private void NotifyListSelectionChanged(XmlListType listType)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                switch (listType)
                {
                    case XmlListType.None:
                        break;
                    case XmlListType.MediaPortalListControl:
                        _listService.NotifyListeners(ListServiceMessage.ListItemSelect);
                        break;
                    case XmlListType.MediaPortalButtonGroup:
                        _listService.NotifyListeners(ListServiceMessage.GroupItemSelect);
                        break;
                    case XmlListType.MediaPortalMenuControl:
                        _listService.NotifyListeners(ListServiceMessage.MenuItemSelect);
                        break;
                    case XmlListType.MediaPortalDialogList:
                        _listService.NotifyListeners(ListServiceMessage.DialogSelect);
                        break;
                    case XmlListType.MPDisplaySkins:
                        _listService.NotifyListeners(ListServiceMessage.SkinItemSelect);
                        break;
                    case XmlListType.MPDisplayStyles:
                        _listService.NotifyListeners(ListServiceMessage.StyleItemSelect);
                        break;
                    case XmlListType.MPDisplayLanguages:
                        _listService.NotifyListeners(ListServiceMessage.LanguageItemSelect);
                        break;
                }

            });
        }

        private void NotifyListLayoutChanged()
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _listService.NotifyListeners(ListServiceMessage.ListItemLayout);
            });
            GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
        }

 
        public static List<APIListType> GetRegisteredLists(IControlHost window)
        {
            return window?.Controls.GetControls().OfType<GUIList>().Select(l => l.ListType.ToAPIType()).ToList() ?? new List<APIListType>();
        }
    }

    public enum ListServiceMessage
    {
        ListItems,
        ListItemSelect,
        ListItemLayout,
        MenuItems,
        MenuItemSelect,
        GroupItems,
        GroupItemSelect,
        SkinItems,
        SkinItemSelect,
        StyleItems,
        StyleItemSelect,
        LanguageItems,
        LanguageItemSelect,
        SendItem,
        DialogItems,
        DialogSelect
    }
}
