using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageFramework.DataObjects;
using Common.Logging;
using Common.Settings;
using MediaPortal.GUI.Library;
using Common.Helpers;
using System.Threading;

namespace MediaPortalPlugin.InfoManagers
{
    public class DialogManager
    {
        #region Singleton Implementation

        private static DialogManager instance;

        private DialogManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(DialogManager));
        }

        public static DialogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DialogManager();
                }
                return instance;
            }
        }

        #endregion

        #region Vars

        private Common.Logging.Log Log = LoggingManager.GetLog(typeof(DialogManager));
        private PluginSettings _settings;
        private bool _isWorking = false;
        private bool _isDialogVisible;
        private GUIWindow _currentDialog;
        private int _currentDialogId = -1;
        private System.Threading.Timer _dialogTimer;
        private enum DialogType { None, Button, List, Notifiy }
        private DialogType _currentDialogType = DialogType.None;
        private List<GUIListControl> _listControls;
        private List<GUIButtonControl> _buttonControls;
        private int _lastFocusedControlId;
        private APIListAction _lastSelectedAction;

        #endregion


        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            _dialogTimer = new System.Threading.Timer((o) => ProcessDialogThread(), null, 1000, 150);
        }

        public void Shutdown()
        {
            if (_dialogTimer != null)
            {
                _dialogTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _dialogTimer = null;
            }
        }

        public void RegisterDialogInfo(APIWindowInfoMessage message)
        {
            
        }

    


        #region Process

        public void OnActionMessageReceived(APIActionMessage action)
        {
            try
            {
                if (action != null && action.ListAction != null)
                {
                    if (_currentDialog != null)
                    {
                        if (_currentDialogType == DialogType.List)
                        {
                            var currentList = _listControls.FirstOrDefault(f => f.Focus);
                            if (currentList != null)
                            {
                                if (action.ListAction.ItemIndex <= currentList.Count)
                                {
                                    currentList.SelectedListItemIndex = action.ListAction.ItemIndex;
                                    if (action.ListAction.ActionType == APIListActionType.SelectedItem)
                                    {
                                        GUIGraphicsContext.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)7, 0f, 0f));
                                    }
                                }
                            }
                        }
                        else if (_currentDialogType == DialogType.Button)
                        {
                            var buttons = _currentDialog.GetControls<GUIButtonControl>();
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[OnActionMessageReceived] - An exception occured processing dialog thread", ex);
            }
        }

        /// <summary>
        /// Processes the dialog thread.
        /// </summary>
        /// <param name="state">The state.</param>
        private void ProcessDialogThread()
        {
            try
            {
                if (!_isWorking)
                {
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
            }
            catch (Exception ex)
            {
                Log.Exception("[DialogManager]-[ProcessDialogThread] - An exception occured processing dialog thread",  ex);
            }
        }

        private void DialogOpen()
        {
            try
            {
                if (_currentDialog != null)
                {
                    _listControls = new List<GUIListControl>(_currentDialog.GetControls<GUIListControl>());
                    _buttonControls = new List<GUIButtonControl>(_currentDialog.GetControls<GUIButtonControl>());
                    _currentDialogType = _listControls.Any() ? DialogType.List : _buttonControls.Any() ? DialogType.Button : DialogType.Notifiy;

                    if (_currentDialogType == DialogType.List)
                    {
                        var currentList = _listControls.FirstOrDefault(f => f.Focus);
                        if (currentList != null)
                        {
                            SendList(ListManager.Instance.GetAPIListItems(currentList, APIListLayout.Vertical));
                        }
                    }
                    else if (_currentDialogType == DialogType.Button)
                    {
                        var items = _currentDialog.GetControls<GUIButtonControl>()
                            .Select(item => new APIListItem
                            {
                                Label = item.Label,
                                Index = item.GetID
                            });
                        SendList(items);
                    }


                    foreach (var control in _currentDialog.GetControls())
                    {
                        var label = ReflectionHelper.GetPropertyValue<string>(control, "Label", null);
                        if (!string.IsNullOrEmpty(label))
                        {
                            string tag = string.Format("#Dialog.Label{0}", control.GetID);
                            PropertyManager.Instance.SendLabelProperty(tag, label);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[DialogOpen] - An exception occured processing dialog open", ex);
            }
        }

        private void DialogClose()
        {
            try
            {
                if (_currentDialog != null)
                {
                    _listControls.Clear();
                    _buttonControls.Clear();
                    _currentDialogId = -1;
                    _lastFocusedControlId = -1;
                    _currentDialogType = DialogType.None;
                    _currentDialog = null;
                    ListManager.Instance.CheckForListItemChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[DialogClose] - An exception occured processing dialog close", ex);
            }
        }

        /// <summary>
        /// On new dialog action
        /// </summary>
        private void DialogUpdate()
        {
            try
            {
                if (_currentDialog != null)
                {
                    if (_currentDialogType == DialogType.List)
                    {
                        var currentList = _listControls.FirstOrDefault(f => f.Focus);
                        if (currentList != null && currentList.SelectedListItem != null)
                        {
                           SendSelectedItem(currentList.SelectedListItem.Label, currentList.SelectedListItemIndex);
                        }
                    }
                    else if (_currentDialogType == DialogType.Button)
                    {
                        var focusCtrl = _currentDialog.GetControls<GUIButtonControl>().FirstOrDefault(b => b.Focus);
                        if (focusCtrl != null)
                        {
                            SendSelectedItem(focusCtrl.Label, focusCtrl.GetID);
                        }
                    }
                    SendDialogFocusMessage();
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[DialogUpdate] - An exception occured processing dialog update", ex);
            }
        }

        #endregion


        private void SendList(IEnumerable<APIListItem> items)
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

            if (!action.IsEqual(_lastSelectedAction))
            {
                MessageService.Instance.SendListMessage(new APIListMessage
                {
                    MessageType = APIListMessageType.Action,
                    Action = action
                });
                _lastSelectedAction = action;
            }
        }

        private void SendDialogOpenMessage()
        {
            if (_currentDialog != null)
            {
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
            }
        }

        private void SendDialogCloseMessage()
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
        }

        private void SendDialogFocusMessage()
        {
            if (_currentDialog != null)
            {
                int focusedControlId = _currentDialog.GetFocusControlId();
                if (_lastFocusedControlId != focusedControlId)
                {
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
                }
            }
        }
     
    }
}
