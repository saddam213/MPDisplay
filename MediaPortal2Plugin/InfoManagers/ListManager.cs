using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.Common.General;
using MediaPortal.UiComponents.SkinBase.Models;
using MediaPortal.UiComponents.Weather.Models;
using MediaPortal.UiComponents.News.Models;
using MediaPortal.UiComponents.Media.Models;
using MediaPortal.UiComponents.Weather;
using MediaPortal.UI.Presentation.DataObjects;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using Log = Common.Log.Log;

namespace MediaPortal2Plugin.InfoManagers
{
    public class ListManager
    {
       #region Singleton Implementation

        private static ListManager _instance;

        private ListManager()
        {
            _log = LoggingManager.GetLog(typeof(ListManager));
        }

        public static ListManager Instance => _instance ?? (_instance = new ListManager());

        #endregion

        private static Log _log;
        private static bool _groupFocused;
        private static bool _listFocused;
        private static bool _isRecheckingListItems;
        private static bool _isRecheckingListType;
        private static bool _checkItemsNeedsRefresh;
        private static int _focusedControlId = -1;
        private List<APIListType> _registeredListTypes = new List<APIListType>();
        private PluginSettings _settings;
        private static int _currentBatchId;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
        }

        public void Shutdown()
        {
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
            //RegisterWindowListTypes();
        }

   

        public void SetWindowListControls()
        {
            if (WindowManager.Instance.CurrentWindow == null) return;
            //RegisterWindowListTypes();
        }

        public void ClearWindowListControls()
        {
            if (_registeredListTypes.Any())
            {
                _registeredListTypes.ForEach(t => SendList(t, APIListLayout.Vertical, new List<APIListItem>()));
            }
            _focusedControlId = -1;
            _groupFocused = false;
            _listFocused = false;
            _isRecheckingListItems = false;
            _isRecheckingListType = false;
            _currentLayout = APIListLayout.Vertical;
        }


        //private void RegisterWindowListTypes()
        //{
        //    if (!_registeredListTypes.Any()) return;
        //    if (_registeredListTypes.Contains(APIListType.List))
        //    {
        //        SendFacadeList();
        //        SendListControlList();
        //        SendFacadeSelectedItem();
        //        ThreadPool.QueueUserWorkItem(o =>
        //        {
        //            Thread.Sleep(2000);
        //            SendFacadeLayout();
        //         });

        //    }

        //    if (_registeredListTypes.Contains(APIListType.GroupMenu))
        //    {
        //        SendGroupList();
        //        SendGroupSelectedItem();
        //    }

        //    if (_registeredListTypes.Contains(APIListType.Menu))
        //    {
        //        SendMenuControlList();
        //    }
        //}


        public void SendSkinEditorData(ListItem item)
        {
            if (item == null || !MessageService.Instance.IsSkinEditorConnected) return;

            try
            {

                var data = item.Labels.Select(kvp => new[] {kvp.Key, kvp.Value.ToString()}).ToList();
                _log.Message(LogLevel.Debug, "[SendSkinEditorData] - ListItem <{0}> #Labels {1}", item.Label("Name","----"), item.Labels.Count);

            MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = APISkinEditorDataType.ListItem,
                    ListItemData = data
                });
            }
            catch (Exception ex)
            {
                _log.Message(LogLevel.Error, "[SendSkinEditorData] - An exception occured sendind data for ListItem <{0}>. Exception: {1}", item, ex.Message);
            }
        }



        #region ListControl

        //private void SendListControlList()
        //{
        //    var currentList = _listControls.FirstOrDefault(f => f.Focus) ?? _listControls.FirstOrDefault(l => l.Count > 0);
        //    if (currentList != null)
        //    {
        //        SendList(APIListType.List, APIListLayout.Vertical, GetApiListItems(currentList, APIListLayout.Vertical));
        //    }
        //}


        //private void SendListSelectedItem()
        //{
        //    var currentList = _listControls.FirstOrDefault(f => f.Focus);
        //    if (currentList?.SelectedListItem == null) return;

        //    SendSelectedItem(APIListType.List, currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);

        //    SendSkinEditorData(currentList.SelectedListItem);
        //}

        //private void SetListSelectedItem(APIListAction item, bool isSelect)
        //{
        //    var currentList = _listControls.FirstOrDefault(f => f.Focus);
        //    if (currentList == null) return;

        //    if (item.ItemIndex > currentList.Count) return;

        //    currentList.SelectedListItemIndex = item.ItemIndex;
        //    if (isSelect)
        //    {
        //        SupportedPluginManager.GuiSafeInvoke(() => 
        //        {
        //            currentList.OnAction(new Action((Action.ActionType)7, 0f, 0f));
        //            CheckForListItemChanges();
        //        });
        //    }
        //}

        #endregion



        #region Helpers

        //public List<APIListItem> GetApiListItems(GUIGroup actionMenu)
        //{
        //    var returnValue = new List<APIListItem>();
        //    if (actionMenu == null) return returnValue;
        //    var index = 0;
        //    foreach (var item in actionMenu.Children.GetControls())
        //    {
        //        var label = string.Empty;
        //        if (item is GUIButtonControl)
        //        {
        //            label = (item as GUIButtonControl).Label;
        //        }
        //        if (item is GUIMenuButton)
        //        {
        //            label = (item as GUIMenuButton).SelectedItemLabel;
        //        }
        //        if (item is GUICheckButton)
        //        {
        //            label = (item as GUICheckButton).Label;
        //        }

        //        if (label == null) continue;
        //        returnValue.Add(new APIListItem { Index = index, Label = label });
        //        index++;
        //    }
        //    return returnValue;
        //}





        //public List<APIListItem> GetApiListItems(GUIListControl listcontrol, APIListLayout layout)
        //{
        //    var returnValue = new List<APIListItem>();
        //    if (listcontrol == null) return returnValue;

        //    var items = new List<GUIListItem>();
        //    for (var i = 0; i < listcontrol.Count; i++)
        //    {
        //        items.Add(listcontrol[i]);
        //    }

        //    if (WindowManager.Instance.CurrentPlugin != null)
        //    {
        //        returnValue = WindowManager.Instance.CurrentPlugin.GetListItems(items, layout);
        //    }

        //    if (returnValue.Any() || !items.Any()) return returnValue;

        //    var index = 0;
        //    foreach (var item in items)
        //    {
        //        returnValue.Add(new APIListItem
        //        {
        //            Index = index,
        //            Label = item.Label,
        //            Label2 = item.Label2,
        //            Label3 = item.Label3,
        //            Image = GetItemImageBytes(item, layout)
        //        });
        //        index++;
        //    }
        //    return returnValue;
        //}

        //public List<APIListItem> GetApiListItems(GUIMenuControl menuControl)
        //{
        //    var returnValue = new List<APIListItem>();
        //    if (menuControl == null) return returnValue;

        //    var index = 0;
        //    foreach (var button in menuControl.ButtonInfos)
        //    {
        //        returnValue.Add(new APIListItem
        //        {
        //            Index = index,
        //            Label = button.Text
        //        });
        //        index++;
        //    }
        //    return returnValue;
        //}

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

        //public APIListLayout GetApiListLayout(GUIFacadeControl facade)
        //{
        //    if (facade == null) return APIListLayout.Vertical;

        //    // get layout for plugins from plugin settings
        //    if (WindowManager.Instance.CurrentPlugin != null)
        //    {
        //        switch (facade.CurrentLayout)
        //        {
        //            case GUIFacadeControl.Layout.AlbumView:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutAlbumview);
        //            case GUIFacadeControl.Layout.CoverFlow:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutCoverflow);
        //            case GUIFacadeControl.Layout.Filmstrip:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutFilmstrip);
        //            case GUIFacadeControl.Layout.SmallIcons:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutSmallIcons);
        //            case GUIFacadeControl.Layout.List:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutList);
        //            case GUIFacadeControl.Layout.Playlist:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutPlaylist);
        //            case GUIFacadeControl.Layout.LargeIcons:
        //                return GetApiListLayout(WindowManager.Instance.CurrentPlugin.Settings.ListLayoutLargeIcons);
        //        }
        //    }
        //    else
        //    {
        //        switch (facade.CurrentLayout)
        //        {
        //            case GUIFacadeControl.Layout.AlbumView:
        //            case GUIFacadeControl.Layout.CoverFlow:
        //                return APIListLayout.CoverFlow;
        //            case GUIFacadeControl.Layout.Filmstrip:
        //            case GUIFacadeControl.Layout.SmallIcons:
        //            case GUIFacadeControl.Layout.LargeIcons:
        //                return APIListLayout.Horizontal;
        //            case GUIFacadeControl.Layout.List:
        //            case GUIFacadeControl.Layout.Playlist:
        //                return APIListLayout.Vertical;
        //        }
        //    }
        //    return APIListLayout.Vertical;
        //}

        //public APIImage GetItemImageBytes(GUIListItem item, APIListLayout layout)
        //{
        //    var filename = string.Empty;
        //    if (item == null) return ImageHelper.CreateImage(filename);

        //    if (item.HasThumbnail)
        //    {
        //        filename = item.ThumbnailImage;
        //    }
        //    else if (item.HasIconBig)
        //    {
        //        filename = item.IconImageBig;
        //    }
        //    else if (item.HasIcon)
        //    {
        //        filename = item.IconImage;
        //    }
        //    else if (item.HasPinIcon)
        //    {
        //        filename = item.PinImage;
        //    }
        //    return ImageHelper.CreateImage(filename);
        //}

        #endregion

        #region Send Messages

        public void SendList(APIListType listType, APIListLayout layout, List<APIListItem> items)
        {
            var count = items?.Count ?? 0;
            _currentBatchId++;
            if (listType == APIListType.List && count >= _settings.ListBatchThreshold)
            {
               
                var batchNo = 1;
                var batchCount = count < _settings.ListBatchSize ? 1 : (count + _settings.ListBatchSize - 1) / _settings.ListBatchSize;
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
