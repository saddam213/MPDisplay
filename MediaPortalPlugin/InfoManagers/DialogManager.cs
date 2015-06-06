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
    public class DialogManager 
    {
        #region Singleton Implementation

        private static DialogManager _instance;

        private DialogManager()
        {
            _log = LoggingManager.GetLog(typeof(DialogManager));
        }

        public static DialogManager Instance
        {
            get { return _instance ?? (_instance = new DialogManager()); }
        }

        #endregion

        #region Vars

        private Log _log;
        private bool _isWorking;
        private bool _isDialogVisible;
        private GUIWindow _currentDialog;
        private int _currentDialogId = -1;
        private Timer _dialogTimer;
        private enum DialogType { None, Button, List, Notifiy }
        private DialogType _currentDialogType = DialogType.None;
        private List<GUIListControl> _listControls;
        private List<GUIButtonControl> _buttonControls;
        private int _lastFocusedControlId;
        private APIListAction _lastSelectedAction;
        private int _lastListCount;

        #endregion


        public void Initialize(PluginSettings settings)
        {
            _dialogTimer = new Timer(o => ProcessDialogThread(), null, 1000, 150);
            _log = LoggingManager.GetLog(typeof(DialogManager));
        }

        public void Shutdown()
        {
            if (_dialogTimer == null) return;
            _dialogTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _dialogTimer = null;
        }

        public void RegisterDialogInfo(APIWindowInfoMessage message)
        {
            
        }

         #region Process

        public void OnActionMessageReceived(APIActionMessage action)
        {
            try
            {
                if (action == null || action.ListAction == null) return;
                if (_currentDialog == null) return;
                switch (_currentDialogType)
                {
                    case DialogType.List:
                        var currentList = _listControls.FirstOrDefault(f => f.Focus);
                        if (currentList != null)
                        {
                            if (action.ListAction.ItemIndex <= currentList.Count)
                            {
                                currentList.SelectedListItemIndex = action.ListAction.ItemIndex;
                                if (action.ListAction.ActionType == APIListActionType.SelectedItem)
                                {
                                    GUIGraphicsContext.OnAction(new Action((Action.ActionType)7, 0f, 0f));
                                }
                            }
                        }
                        break;
                    case DialogType.Button:
                        var buttons = _currentDialog.GetControls<GUIButtonControl>().ToList();
                        if (buttons.Any())
                        {
                            var currentFocus = buttons.FirstOrDefault(c => c.Focus);
                            if (currentFocus != null)
                            {
                                currentFocus.Selected = false;
                                currentFocus.Focus = false;
                            }

                            var newFocus = buttons.FirstOrDefault(c => c.GetID == action.ListAction.ItemIndex || c.Label == action.ListAction.ItemText);
                            if (newFocus != null)
                            {
                                newFocus.Selected = true;
                                newFocus.Focus = true;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[OnActionMessageReceived] - An exception occured processing dialog thread", ex);
            }
        }

        /// <summary>
        /// Processes the dialog thread.
        /// </summary>
        private void ProcessDialogThread()
        {
            try
            {
                if (_isWorking) return;

                _isWorking = true;
                if ((GUIWindowManager.IsRouted && !_isDialogVisible) || (GUIWindowManager.IsRouted && _isDialogVisible && _currentDialogId != GUIWindowManager.RoutedWindow))
                {
                    var dialog = GUIWindowManager.GetWindow(GUIWindowManager.RoutedWindow);
                    if (dialog != null)
                    {
                        Thread.Sleep(200);
                        _isDialogVisible = true;
                        _currentDialogId = GUIWindowManager.RoutedWindow;
                        _currentDialog = dialog;
                        SendDialogOpenMessage();
                        DialogOpen();
                    }
                }
                else if (!GUIWindowManager.IsRouted && _isDialogVisible)
                {
                    DialogClose();
                    SendDialogCloseMessage();
                    _isDialogVisible = false;
                }

                if (_isDialogVisible)
                {
                    DialogUpdate();
                }
                _isWorking = false;
            }
            catch (Exception ex)
            {
                _log.Exception("[DialogManager]-[ProcessDialogThread] - An exception occured processing dialog thread",  ex);
            }
        }

        private void DialogOpen()
        {
            try
            {
                if (_currentDialog == null) return;

                _listControls = new List<GUIListControl>(_currentDialog.GetControls<GUIListControl>());
                _buttonControls = new List<GUIButtonControl>(_currentDialog.GetControls<GUIButtonControl>());
                _currentDialogType = _listControls.Any() ? DialogType.List : _buttonControls.Any() ? DialogType.Button : DialogType.Notifiy;

                switch (_currentDialogType)
                {
                    case DialogType.List:
                        var currentList = _listControls.FirstOrDefault(f => f.Focus);
                        if (currentList != null)
                        {
                            SendList(ListManager.Instance.GetApiListItems(currentList, APIListLayout.Vertical));
                        }
                        break;
                    case DialogType.Button:
                        var items = _currentDialog.GetControls<GUIButtonControl>()
                            .Select(item => new APIListItem
                            {
                                Label = item.Label,
                                Index = item.GetID
                            });
                        SendList(items);
                        break;
                }


                foreach (var control in _currentDialog.GetControls())
                {
                    if (!_buttonControls.Contains(control))
                    {
                        var label = ReflectionHelper.GetPropertyValue<string>(control, "Label", null);
                        if (!string.IsNullOrEmpty(label))
                        {
                            var tag = string.Format("#Dialog.Label{0}", control.GetID);
                            PropertyManager.Instance.SendLabelProperty(tag, label);
                            SendEditorData(APISkinEditorDataType.Property, tag, label);
                        }
                    }
                    if (!(control is GUIImage)) continue;

                    var imagepath = ReflectionHelper.GetPropertyValue<string>(control, "FileName", null);
                    if (string.IsNullOrEmpty(imagepath)) continue;

                    var tag1 = string.Format("#Dialog.Image{0}", control.GetID);
                    PropertyManager.Instance.SendImageProperty(tag1, imagepath);
                    SendEditorData(APISkinEditorDataType.Property, tag1, imagepath);
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[DialogOpen] - An exception occured processing dialog open", ex);
            }
        }

        private void DialogClose()
        {
            try
            {
                if (_currentDialog == null) return;

                _listControls.Clear();
                _buttonControls.Clear();
                _currentDialogId = -1;
                _lastFocusedControlId = -1;
                _currentDialogType = DialogType.None;
                _currentDialog = null;
                ListManager.Instance.CheckForListItemChanges();
            }
            catch (Exception ex)
            {
                _log.Exception("[DialogClose] - An exception occured processing dialog close", ex);
            }
        }

        /// <summary>
        /// On new dialog action
        /// </summary>
         private void DialogUpdate()
        {
            try
            {
                if (_currentDialog == null) return;

                switch (_currentDialogType)
                {
                    case DialogType.List:
                        var currentList = _listControls.FirstOrDefault(f => f.Focus);
                        if (currentList != null && currentList.SelectedListItem != null)
                        {
                            SendSelectedItem(currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);
                            SendSkinEditorData(currentList.SelectedListItem);

                        }
                        if (currentList != null && currentList.ListItems.Count != _lastListCount)
                        {
                            _lastListCount = currentList.ListItems.Count;
                            SendList(ListManager.Instance.GetApiListItems(currentList, APIListLayout.Vertical));
                        }
                        break;
                    case DialogType.Button:
                        var focusCtrl = _currentDialog.GetControls<GUIButtonControl>().FirstOrDefault(b => b.Focus);
                        if (focusCtrl != null)
                        {
                            SendSelectedItem(focusCtrl.Label, focusCtrl.GetID);
                            var item = new APIListItem
                            {
                                Label = focusCtrl.Label,
                                Index = focusCtrl.GetID
                            };
                            SendSkinEditorData(item);
                        }
                        break;
                }
                SendDialogFocusMessage();
            }
            catch (Exception ex)
            {
                _log.Exception("[DialogUpdate] - An exception occured processing dialog update", ex);
            }
        }

        #endregion


        private static void SendList(IEnumerable<APIListItem> items)
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.List,
                List = new APIList
                {
                    ListType = APIListType.DialogList,
                    ListItems = new List<APIListItem>(items),
                    ListLayout =  APIListLayout.Vertical
                }
            });
        }

        private void SendSelectedItem(string text, int index)
        {
            var action = new APIListAction
            {
                ActionType = APIListActionType.SelectedItem,
                ItemListType = APIListType.DialogList,
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
        }

        private void SendDialogOpenMessage()
        {
            if (_currentDialog == null) return;

            MessageService.Instance.SendInfoMessage(new APIInfoMessage
            {
                MessageType = APIInfoMessageType.DialogMessage,
                DialogMessage = new APIDialogMessage
                {
                    MessageType = APIDialogMessageType.DialogId,
                    DialogId = _currentDialogId,
                    FocusedControlId = _currentDialog.GetFocusControlId()
                }
            });

            SendEditorData(APISkinEditorDataType.DialogId, _currentDialogId);
        }

        private static void SendDialogCloseMessage()
        {
            MessageService.Instance.SendInfoMessage(new APIInfoMessage
            {
                MessageType = APIInfoMessageType.DialogMessage,
                DialogMessage = new APIDialogMessage
                {
                    MessageType = APIDialogMessageType.DialogId,
                    DialogId = -1,
                    FocusedControlId = -1
                }
            });

            SendEditorData(APISkinEditorDataType.DialogId, -1);
        }

        private void SendDialogFocusMessage()
        {
            if (_currentDialog == null) return;

            var focusedControlId = _currentDialog.GetFocusControlId();
            if (_lastFocusedControlId == focusedControlId) return;

            _lastFocusedControlId = focusedControlId;
            MessageService.Instance.SendInfoMessage(new APIInfoMessage
            {
                MessageType = APIInfoMessageType.DialogMessage,
                DialogMessage = new APIDialogMessage
                {
                    MessageType = APIDialogMessageType.FocusedControlId,
                    DialogId = _currentDialogId,
                    FocusedControlId = _lastFocusedControlId
                }
            });

            SendEditorData(APISkinEditorDataType.FocusedControlId, focusedControlId);
        }

        private static void SendEditorData(APISkinEditorDataType type, int value)
        {
            if (MessageService.Instance.IsSkinEditorConnected)
            {
                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = type,
                    IntValue = value
                });
            }
        }

        private static void SendEditorData(APISkinEditorDataType type, string tag, string tagValue)
        {
            if (MessageService.Instance.IsSkinEditorConnected)
            {
                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = type,
                    PropertyData = new[] { tag, tagValue }
                });
            }
        }

        private static void SendSkinEditorData(GUIListItem item)
        {
            if (item == null || !MessageService.Instance.IsSkinEditorConnected) return;

            try
            {
                var data = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)).Select(property => new[] {property.Name,
                    (string) property.GetValue(item, null)}).ToList();

                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = APISkinEditorDataType.ListItem,
                    ListItemData = data
                });
            }
            catch
            {
                // ignored
            }
        }

        private static void SendSkinEditorData(APIListItem item)
        {
            if (item == null || !MessageService.Instance.IsSkinEditorConnected) return;

            try
            {
                var data = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(string)).Select(property => new[] {property.Name,
                    (string) property.GetValue(item, null)}).ToList();

                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = APISkinEditorDataType.ListItem,
                    ListItemData = data
                });
            }
            catch
            {
                // ignored
            }
        }
    }
}
