using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Log;
using Common.Settings;
using MediaPortal.Common;
using MediaPortal.Common.Messaging;
using MediaPortal.UI.Control.InputManager;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.Workflow;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using MediaPortal.UI.Services.Players;
using MediaPortal.UI.SkinEngine.ScreenManagement;
using MediaPortal.UI.SkinEngine.InputManagement;
using Log = Common.Log.Log;
using Timer = System.Threading.Timer;

namespace MediaPortal2Plugin.InfoManagers
{
    
    public class WindowManager
    {
        #region Singleton Implementation

        private static WindowManager _instance;

        private WindowManager()
        {
            _log = LoggingManager.GetLog(typeof(WindowManager));
        }

        public static WindowManager Instance => _instance ?? (_instance = new WindowManager());

        #endregion

        protected AsynchronousMessageQueue MessageQueue;
        protected object SyncObj = new object();

        public PlayerContext CurrentPlayer { get; private set; }
        public PlayerContext AudioPlayer { get; private set; }

        private readonly Log _log;
        private bool _isFullscreenVideo;
        private PluginSettings _settings;
        private MP2PluginSettings _mp2PluginSettings;
        private int _previousFocusedControlId = -1;
        private APIPlaybackState _currentPlaybackState = APIPlaybackState.None;
        private APIPlaybackType _currentPlaybackType = APIPlaybackType.None;
        private APIPlaybackType _currentPlayerPlugin = APIPlaybackType.None;
        private Timer _secondTimer;
        private DateTime _lastIteraction = DateTime.Now.AddYears(1);
        private DateTime _lastPlayBackChanged;
        private bool _isUserInteracting;
        private bool _isFullScreenMusic;
        private APIPlayerMessage _lastPlayerMessage;


        public MP2IdMapping CurrentWindow { get; private set; } = null;

        //public int CurrentWindowFocusedControlId => CurrentWindow?.GetFocusControlId() ?? -1;

        public void Initialize(PluginSettings settings, AdvancedPluginSettings advancedSettings, MP2PluginSettings mp2PluginSettings)
        {
            _log.Message(LogLevel.Debug, "[Initialize] - Initializing WindowsManager...");
            _settings = settings;
            _mp2PluginSettings = mp2PluginSettings;

           SubscribeToMessages();

            InputManager.Instance.KeyPreview += OnMP2KeyPressed;

            ListManager.Instance.Initialize(_settings);
            PropertyManager.Instance.Initialize(_settings);
            EqualizerManager.Instance.Initialize(_settings);
            //DialogManager.Instance.Initialize(_settings);


            //GUIWindowManager.OnActivateWindow += GUIWindowManager_OnActivateWindow;
            //GUIWindowManager.OnNewAction += GUIWindowManager_OnNewAction;

          //  GUIWindowManager.Receivers += GUIGraphicsContext_Receivers;

            _secondTimer = new Timer( SecondTimerTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            _lastPlayBackChanged = DateTime.MinValue;
            _log.Message(LogLevel.Debug, "[Initialize] - Initialize complete");
        }

        public void Shutdown()
        {
            _secondTimer.Change(Timeout.Infinite, Timeout.Infinite);

            UnsubscribeFromMessages();
            InputManager.Instance.KeyPreview -= OnMP2KeyPressed;

            //GUIWindowManager.OnActivateWindow -= GUIWindowManager_OnActivateWindow;
             //GUIWindowManager.OnNewAction -= GUIWindowManager_OnNewAction;

            //ListManager.Instance.Shutdown();
            PropertyManager.Instance.Shutdown();
            EqualizerManager.Instance.Shutdown();
            //DialogManager.Instance.Shutdown();

        }

        private void SecondTimerTick(object state)
        {
            CheckUserInteraction();
        }

        private void CheckUserInteraction()
        {
            _isUserInteracting = InputManager.Instance.LastInputTime <= _lastIteraction || InputManager.Instance.LastMouseUsageTime <= _lastIteraction;

            if (!_isUserInteracting) return;
            if (DateTime.Now <= _lastIteraction.AddSeconds(_settings.UserInteractionDelay)) return;
            _isUserInteracting = false;
            _lastIteraction = DateTime.Now.AddYears(1);
            OnUserInteractionEnded();
        }

        private void ResetUserInteraction()
        {
            OnUserInteractionStarted();
            _isUserInteracting = true;
            _lastIteraction = DateTime.Now;
        }

        private void OnUserInteractionEnded()
        {
            if (!_currentPlaybackType.IsMusic() || _isFullScreenMusic) return;
            _isFullScreenMusic = true;
            SendPlayerMessage();
            EqualizerManager.Instance.StartEqualizer();
        }

        private void OnUserInteractionStarted()
        {
            if (!_currentPlaybackType.IsMusic() || !_isFullScreenMusic) return;
            _isFullScreenMusic = false;
            EqualizerManager.Instance.StopEqualizer();
            SendPlayerMessage();
        }

        #region MP2 Messaging

        private void SubscribeToMessages()
        {
            _log.Message(LogLevel.Debug, "[SubscribeToMessages] - Initialize subscription to MP2 messages");
            MessageQueue = new AsynchronousMessageQueue(this, new[]
              {
                    ScreenManagerMessaging.CHANNEL,
                    PlayerManagerMessaging.CHANNEL,
                    PlayerContextManagerMessaging.CHANNEL,
                    WorkflowManagerMessaging.CHANNEL
              });
            MessageQueue.MessageReceived += OnMP2MessageReceived;
            MessageQueue.Start();
            _log.Message(LogLevel.Debug, "[SubscribeToMessages] - Subscription to MP2 messages done");
        }

        private void UnsubscribeFromMessages()
        {
            if (MessageQueue == null)
                return;
            _log.Message(LogLevel.Debug, "[UnsubscribeFromMessages] - Unsubscription from MP2 messages done");
            MessageQueue.Shutdown();
            MessageQueue = null;
        }

        private void OnMP2MessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
        {
            if (message.ChannelName == WorkflowManagerMessaging.CHANNEL)
            {
                WorkflowManagerMessaging.MessageType messageType = (WorkflowManagerMessaging.MessageType) message.MessageType;
                _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - WorkflowManagerMessaging Type <{0}>", messageType);
                switch (messageType)
                {
                    case WorkflowManagerMessaging.MessageType.StatePushed:
                        var context = (NavigationContext)message.MessageData[WorkflowManagerMessaging.CONTEXT];
                        if (context != null)
                        {
                            _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - Context Item Label <{0}> WorkflowStateId <{1}>", context.DisplayLabel, context.WorkflowState.StateId);
                            foreach (var modelitem in context.Models)
                            {
                                var model = modelitem.Value;
                                PropertyManager.Instance.RegisterModel(model);
                            }
                        }
                        break;
                    case WorkflowManagerMessaging.MessageType.StatesPopped:
                        var contexts = (IDictionary<Guid, NavigationContext>)message.MessageData[WorkflowManagerMessaging.CONTEXTS];
                        if (contexts != null)
                        {
                            foreach (var contextitem in contexts)
                            {
                                _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - Context Item <{0}> Label <{1}> WorkflowStateId <{2}>", contextitem.Key, contextitem.Value.DisplayLabel, contextitem.Value.WorkflowState.StateId);

                                foreach (var model in contextitem.Value.Models.Select(modelitem => modelitem.Value))
                                {
                                    PropertyManager.Instance.RegisterModel(model);
                                }
                            }
                        }

                        break;
                    case WorkflowManagerMessaging.MessageType.NavigationComplete:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (message.ChannelName == PlayerManagerMessaging.CHANNEL)
            {
                  IPlayerManager playerManager = ServiceRegistration.Get<IPlayerManager>();
                  IPlayerContextManager playerContextManager = ServiceRegistration.Get<IPlayerContextManager>();
                  IPlayerContext playerContext = playerContextManager.GetPlayerContext(playerContextManager.CurrentPlayerIndex);
                  AVType type = AVType.None;
                  if (playerContext != null)
                  {
                    type = playerContext.AVType;
                  } 
          
                // React to player changes
                PlayerManagerMessaging.MessageType messageType = (PlayerManagerMessaging.MessageType) message.MessageType;
                _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - PlayerManagerMessaging Type <{0}>", messageType);
                switch (messageType)
                {
                    case PlayerManagerMessaging.MessageType.PlayerResumeState:
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerError:
                    case PlayerManagerMessaging.MessageType.PlayerEnded:
                        Player_PlayBackEnded(type);
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerStopped:
                        Player_PlayBackStopped(type);
                        break;
                    case PlayerManagerMessaging.MessageType.RequestNextItem:
                        Player_PlayBackChanged(type);
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerStarted:
                        Player_PlayBackStarted(type);
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerStateReady:
                        break;
                    case PlayerManagerMessaging.MessageType.PlaybackStateChanged:
                        break;
                    case PlayerManagerMessaging.MessageType.AudioSlotChanged:
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerSlotStarted:
                        break;
                    case PlayerManagerMessaging.MessageType.PlayerSlotClosed:
                        break;
                    case PlayerManagerMessaging.MessageType.PlayersMuted:
                        break;
                    case PlayerManagerMessaging.MessageType.PlayersResetMute:
                        break;
                    case PlayerManagerMessaging.MessageType.VolumeChanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (message.ChannelName == PlayerContextManagerMessaging.CHANNEL)
            {
                // React to internal player context manager changes
                PlayerContextManagerMessaging.MessageType messageType = (PlayerContextManagerMessaging.MessageType) message.MessageType;
                _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - PlayerContextMessaging Type <{0}>", messageType);
                switch (messageType)
                {
                    case PlayerContextManagerMessaging.MessageType.UpdatePlayerRolesInternal:
                        CurrentPlayer = (PlayerContext) message.MessageData[PlayerContextManagerMessaging.NEW_CURRENT_PLAYER_CONTEXT];
                        AudioPlayer = (PlayerContext) message.MessageData[PlayerContextManagerMessaging.NEW_AUDIO_PLAYER_CONTEXT];
                        break;
                    case PlayerContextManagerMessaging.MessageType.PlayerSlotsChanged:
                        break;
                    case PlayerContextManagerMessaging.MessageType.CurrentPlayerChanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (message.ChannelName == ScreenManagerMessaging.CHANNEL)
            {
                ScreenManagerMessaging.MessageType messageType = (ScreenManagerMessaging.MessageType) message.MessageType;
                var context = ServiceRegistration.Get<IWorkflowManager>().CurrentNavigationContext;
                _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - ScreenManagerMessaging Type <{0}>", messageType);

                switch (messageType)
                {
                    case ScreenManagerMessaging.MessageType.ShowScreen:
                        var screen = (Screen) message.MessageData[ScreenManagerMessaging.SCREEN];
                        var closeDialogs = (bool) message.MessageData[ScreenManagerMessaging.CLOSE_DIALOGS];
                        _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - ScreenManagerMessaging ShowScreen <{0}>, instance ID <{1}> CloseDialogs is {2}", screen.Name, screen.ScreenInstanceId, closeDialogs);

                        if (context != null)
                        {
                            _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - Context Item Label <{0}>", context.DisplayLabel);
                            SetCurrentWindow(context.WorkflowState.StateId.ToString());
                            foreach (var modelitem in context.Models)
                            {
                                var model = modelitem.Value;
                                PropertyManager.Instance.RegisterModel(model);
                            }
                        }
                        break;
                    case ScreenManagerMessaging.MessageType.SetSuperLayer:
                        break;
                    case ScreenManagerMessaging.MessageType.ShowDialog:
                        var dialogData = (DialogData) message.MessageData[ScreenManagerMessaging.DIALOG_DATA];
                         _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - ScreenManagerMessaging ShowDialog <{0}>, name is {1}", dialogData.DialogInstanceId, dialogData.DialogName);                           
                        if (context != null)
                        {
                            var dialogItem = _mp2PluginSettings.GetDialogMapping(context.WorkflowState.StateId.ToString());
                            _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - ScreenManagerMessaging DialogID is <{0}>", dialogItem.MPDid);                           
                        }
                        break;
                    case ScreenManagerMessaging.MessageType.CloseDialogs:
                        var dialogInstanceId = (Guid) message.MessageData[ScreenManagerMessaging.DIALOG_INSTANCE_ID];
                        var mode = (CloseDialogsMode) message.MessageData[ScreenManagerMessaging.CLOSE_DIALOGS_MODE];
                        _log.Message(LogLevel.Debug, "[OnMP2MessageReceived] - ScreenManagerMessaging CloseDialog <{0}>, mode is {1}", dialogInstanceId, mode);
                        break;
                    case ScreenManagerMessaging.MessageType.ReloadScreens:
                        break;
                    case ScreenManagerMessaging.MessageType.SwitchSkinAndTheme:
                        break;
                    case ScreenManagerMessaging.MessageType.SetBackgroundDisabled:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion

        private void OnMP2KeyPressed(ref Key key)
        {
            _log.Message(LogLevel.Debug, "[OnMP2KeyPressed] - Key <{0}> was pressed", key.Name);

            switch (key.Name)
            {
                case "Down":
                case "Left":
                case "Right":
                case "Up":
                case "Info":
                case "Ok":
                case "Back":
                //case Action.ActionType.ACTION_MOUSE_CLICK:
                    ResetUserInteraction();
                    break;
            }


            //    if ((action.wID == Action.ActionType.ACTION_SELECT_ITEM || action.wID == Action.ActionType.ACTION_KEY_PRESSED) && CurrentWindow.GetID == 96742 && CurrentWindowFocusedControlId == 50)
            //    {
            //        SendFocusedControlMessage(6);
            //    }
            //    else
            //    {
            //        SendFocusedControlMessage(-1);
            //    }

            //    if (action.wID != Action.ActionType.ACTION_MOUSE_MOVE)
            //    {
            //        SendActionIdMessage((int) action.wID);
            //    }
        }

        public void OnMediaPortalMessageReceived(APIMediaPortalMessage message)
        {
            if (message == null) return;

            _log.Message(LogLevel.Debug, "[ReceiveMediaPortalMessage] - MediaPortalMessage received from MPDisplay, MessageType: {0}", message.MessageType);
            switch (message.MessageType)
            {
                case APIMediaPortalMessageType.WindowInfoMessage:
                    if (message.WindowMessage != null)
                    {
                        PropertyManager.Instance.RegisterWindowProperties(message.WindowMessage.Properties);
                        ListManager.Instance.RegisterWindowListTypes(message.WindowMessage.Lists);
                        EqualizerManager.Instance.RegisterEqualizer(message.WindowMessage.EQData);
                    }
                    break;
                case APIMediaPortalMessageType.DialogInfoMessage:
                    if (message.WindowMessage != null)
                    {
                        //DialogManager.Instance.RegisterDialogInfo(message.WindowMessage);
                    }
                    break;
            }

            if (message.MessageType != APIMediaPortalMessageType.ActionMessage) return;
            if (message.ActionMessage == null) return;

            switch (message.ActionMessage.ActionType)
            {
                case APIActionMessageType.WindowListAction:
                    //ListManager.Instance.OnActionMessageReceived(message.ActionMessage);
                    break;
                case APIActionMessageType.DialogListAction:
                    //DialogManager.Instance.OnActionMessageReceived(message.ActionMessage);
                    break;
                case APIActionMessageType.GuideAction:
                    TvServerManager.Instance.OnActionMessageReceived(message.ActionMessage);
                    break;
                default:
                    if (message.ActionMessage.MediaPortalAction != null)
                    {
                        //SupportedPluginManager.GuiSafeInvoke(() =>
                        //{
                        //    if (message.ActionMessage.ActionType == APIActionMessageType.MediaPortalAction)
                        //    {
                        //        GUIGraphicsContext.OnAction(new Action((Action.ActionType) message.ActionMessage.MediaPortalAction.ActionId, 0f, 0f));
                        //    }

                        //    if (message.ActionMessage.ActionType == APIActionMessageType.MediaPortalWindow)
                        //    {
                        //        GUIWindowManager.ActivateWindow(message.ActionMessage.MediaPortalAction.ActionId);
                        //    }
                        //});
                    }
                    break;
            }
        }


        public void SendFullUpdate()
        {
            _log.Message(LogLevel.Debug, "[SendFullUpdate] - Sending full information update");
            //SetCurrentWindow(GUIWindowManager.ActiveWindow);
            SendPlayerMessage();
            TvServerManager.Instance.SendTvGuide();
            TvServerManager.Instance.SendRecordings();
        }

        private void SetCurrentWindow(string guid)
        {
            CurrentWindow = _mp2PluginSettings.GetWindowMapping(guid);
           _log.Message(LogLevel.Debug, "[SetCurrentWindow] - WindowId: {0}", CurrentWindow.MPDid);

            //ListManager.Instance.ClearWindowListControls();
            //PropertyManager.Instance.Suspend(true);

            //var retry = 0;
            //while (GUIWindowManager.IsSwitchingToNewWindow || !GUIWindowManager.Initalized)
            //{
            //    _log.Message(LogLevel.Debug, "[SetCurrentWindow] - Waiting for window to initalize, WindowId: {0}", windowId);
            //    Thread.Sleep(200);
            //    if (retry > 100)
            //    {
            //        _log.Message(LogLevel.Error, "[SetCurrentWindow] - I've been waiting for ages... So I am giving up.., WindowId: {0}", windowId);
            //        break;
            //    }
            //    retry++;
            //}

            //CurrentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
            //var fullscreen = GUIGraphicsContext.IsFullScreenVideo || CurrentWindow.GetID == 2005 || CurrentWindow.GetID == 602;

            //if (!fullscreen)
            //{
            //    CurrentPlugin = SupportedPluginManager.GetPluginHelper(Instance.CurrentWindow.GetID);
                  SendWindowMessage();
            //    ListManager.Instance.SetWindowListControls();
            //    if (Instance.CurrentWindow.GetID == 600 || Instance.CurrentWindow.GetID == 604)
            //    {
            //        var firstEpg = Instance.CurrentWindow.GetControls().FirstOrDefault(c => c.GetID > 50000);
            //        if (firstEpg != null)
            //        {
            //            SendFocusedControlMessage(firstEpg.GetID);
            //        }
            //    }
            //}

            // if fullscreen state has changed send player update
            //if (fullscreen != _isFullscreenVideo)
            //{
            //    _isFullscreenVideo = fullscreen;
            //    SendPlayerMessage();
            //}
            //// SendFocusedControlMessage();
            //PropertyManager.Instance.Suspend(false);
        }

        private void SendWindowMessage()
        {
            if (CurrentWindow == null) return;

            if (MessageService.Instance.IsMpDisplayConnected)
            {
            //      _log.Message(LogLevel.Debug, "[SendWindowMessage] - WindowId: {0}, FocusedControlId: {1}", CurrentWindow.GetID, CurrentWindow.GetFocusControlId());
            //    MessageService.Instance.SendInfoMessage(new APIInfoMessage
            //    {
            //        MessageType = APIInfoMessageType.WindowMessage, WindowMessage = new APIWindowMessage
            //        {
            //            WindowId = CurrentWindow.GetID, FocusedControlId = CurrentWindow.GetFocusControlId(), EnabledPlugins = _enabledlugins
            //        }
            //    });
            }

            if (MessageService.Instance.IsSkinEditorConnected) SendEditorData(APISkinEditorDataType.WindowId, CurrentWindow.MPDid);
        }

        private void SendFocusedControlMessage(int controlId)
        {
            //if (CurrentWindow == null) return;

            //var focusId = CurrentWindowFocusedControlId;

            //if (MessageService.Instance.IsMpDisplayConnected)
            //{
            //    var channelId = -1;
            //    var programId = -1;

            //    if (controlId >= 0) focusId = controlId;

            //    // If control is a TVGuide item send also programId and channelId
            //    if ((CurrentWindow.GetID == 600 || CurrentWindow.GetID == 604) && focusId >= 50000)
            //    {
            //        var item = CurrentWindow.GetControl(focusId);
            //        var program = item.Data;
            //        channelId = ReflectionHelper.GetPropertyValue(program, "IdChannel", -1);
            //        programId = ReflectionHelper.GetPropertyValue(program, "IdProgram", -1);
            //    }

            //    if (focusId == _previousFocusedControlId && programId <= 0) return;

            //    _previousFocusedControlId = focusId;
            //    _log.Message(LogLevel.Debug, "[SendFocusedControlId] - FocusedControlId: {0}", focusId);
            //    MessageService.Instance.SendInfoMessage(new APIInfoMessage
            //    {
            //        MessageType = APIInfoMessageType.WindowMessage, WindowMessage = new APIWindowMessage
            //        {
            //            MessageType = APIWindowMessageType.FocusedControlId, FocusedControlId = focusId, ProgramId = programId, ChannelId = channelId
            //        }
            //    });
            //}

            //if (MessageService.Instance.IsSkinEditorConnected) SendEditorData(APISkinEditorDataType.FocusedControlId, focusId);
        }


        private void SendPlayerMessage()
        {
            //if (CurrentWindow == null ) return;
            if (!MessageService.Instance.IsMpDisplayConnected) return;

            var message = new APIPlayerMessage
            {
                PlaybackState = _currentPlaybackState, PlaybackType = _currentPlaybackType, PlayerPluginType = _currentPlayerPlugin, PlayerFullScreen = _isFullscreenVideo || _isFullScreenMusic
            };

            if (message.IsEquals(_lastPlayerMessage)) return;

            _log.Message(LogLevel.Debug, "[SendPlayerMessage] - PlaybackState: {0}, PlaybackType: {1}, PlayerPluginType: {2}, FullScreen: {3}", _currentPlaybackState, _currentPlaybackType, _currentPlayerPlugin, _isFullScreenMusic || _isFullscreenVideo);
            _lastPlayerMessage = message;
            MessageService.Instance.SendInfoMessage(new APIInfoMessage
            {
                MessageType = APIInfoMessageType.PlayerMessage, PlayerMessage = message
            });
        }


        private static void SendActionIdMessage(int actionId)
        {
            if (MessageService.Instance.IsMpDisplayConnected)
            {
                MessageService.Instance.SendDataMessage(new APIDataMessage
                {
                    DataType = APIDataMessageType.MPActionId, IntValue = actionId
                });
            }
        }

        #region Player

        private void Player_PlayBackEnded(AVType type)
        {
            if (DateTime.Now > _lastPlayBackChanged.AddSeconds(10)) // Ignore Player Ended event due to MovingPictures issue when playing multiple files
            {
                _log.Message(LogLevel.Debug, "[Player_PlayBackEnded] - PlayType: {0}", type);
                EqualizerManager.Instance.StopEqualizer();
                _currentPlaybackState = APIPlaybackState.Stopped;
                _currentPlaybackType = APIPlaybackType.None;
                _currentPlayerPlugin = APIPlaybackType.None;
                _isFullScreenMusic = false;
                _isFullscreenVideo = false;
                SendPlayerMessage();
            }
            else
            {
                _lastPlayBackChanged = DateTime.MinValue; // reset changed flag after first stop event
            }
        }

        private void Player_PlayBackStopped(AVType type)
        {
            _log.Message(LogLevel.Debug, "[Player_PlayBackStopped] - PlayType: {0}", type);
            EqualizerManager.Instance.StopEqualizer();
            _currentPlaybackState = APIPlaybackState.Stopped;
            _currentPlaybackType = APIPlaybackType.None;
            _currentPlayerPlugin = APIPlaybackType.None;
            _isFullScreenMusic = false;
            _isFullscreenVideo = false;
            SendPlayerMessage();
        }

        void Player_PlayBackChanged(AVType type)
        {
            _log.Message(LogLevel.Debug, "[Player_PlayBackChanged] - PlayType: {0}", type);
        }

        private void Player_PlayBackStarted(AVType type)
        {
            _log.Message(LogLevel.Debug, "[Player_PlayBackStarted] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Playing;
            _currentPlaybackType = GetPlaybackType(type);
            _isFullscreenVideo = _currentPlaybackType.IsVideo();
            _isFullScreenMusic = _currentPlaybackType.IsMusic();
            SendPlayerMessage();
            if (_isFullScreenMusic)
            {
                EqualizerManager.Instance.StartEqualizer();
            }
        }

        private static APIPlaybackType GetPlaybackType(AVType type)
        {
            switch (type)
            {
                case AVType.Audio:
                    return APIPlaybackType.IsMusic;
                case AVType.Video:
                    return APIPlaybackType.IsVideo;
                case AVType.None:
                    return APIPlaybackType.None;
            }
            return APIPlaybackType.None;
        }

        #endregion

        private static void SendEditorData(APISkinEditorDataType type, int value)
        {
            if (MessageService.Instance.IsSkinEditorConnected)
            {
                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = type, IntValue = value
                });
            }
        }
    }
}
