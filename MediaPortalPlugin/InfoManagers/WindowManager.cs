using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MessageFramework.DataObjects;
using Common.Logging;
using Common.Settings;
using System.Xml.Linq;
using MediaPortal.Profile;
using MediaPortalPlugin.PluginHelpers;
using MediaPortal.Configuration;

namespace MediaPortalPlugin.InfoManagers
{
    

    public class WindowManager
    {
        #region Singleton Implementation

        private static WindowManager instance;

        private WindowManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(WindowManager));
        }

        public static WindowManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowManager();
                }
                return instance;
            }
        }

        #endregion

        private Common.Logging.Log Log;
        private bool _isFullscreenVideo;
        private GUIWindow _currentWindow;
        private PluginSettings _settings;
        private AdvancedPluginSettings _advancedSettings;
        private int _previousFocusedControlId = -1;
        private APIPlaybackState _currentPlaybackState = APIPlaybackState.None;
        private APIPlaybackType _currentPlaybackType = APIPlaybackType.None;
        private APIPlaybackType _currentPlayerPlugin = APIPlaybackType.None;
        private Timer _secondTimer;
        private DateTime _lastIteraction = DateTime.Now.AddYears(1);
        private DateTime _lastPlayBackChanged;
        private bool _isUserInteracting = false;
        private List<string> _enabledlugins;
        private bool _isFullScreenMusic;
        private APIPlayerMessage _lastPlayerMessage;
        private PluginHelper _currentPlugin;
     
        public GUIWindow CurrentWindow
        {
            get { return _currentWindow; }
        }

        public PluginHelper CurrentPlugin
        {
            get { return _currentPlugin; }
        }

        public int CurrentWindowFocusedControlId
        {
            get { return _currentWindow != null ? _currentWindow.GetFocusControlId() : -1; }
        }

        public void Initialize(PluginSettings settings, AdvancedPluginSettings advancedSettings)
        {
            _settings = settings;
            _advancedSettings = advancedSettings;

            if (_enabledlugins == null)
            {
                _enabledlugins = MPSettings.Instance.GetSection<string>("plugins").Where(kv => kv.Value == "yes").Select(kv => kv.Key).ToList();
            }
            SupportedPluginManager.LoadPlugins(_advancedSettings);

            ListManager.Instance.Initialize(_settings);
            PropertyManager.Instance.Initialize(_settings);
            EqualizerManager.Instance.Initialize(_settings);
            DialogManager.Instance.Initialize(_settings);

            GUIWindowManager.OnActivateWindow += GUIWindowManager_OnActivateWindow;
            g_Player.PlayBackStarted += Player_PlayBackStarted;
            g_Player.PlayBackStopped += Player_PlayBackStopped;
            g_Player.PlayBackEnded += Player_PlayBackEnded;
            g_Player.PlayBackChanged += Player_PlayBackChanged;
            GUIWindowManager.OnNewAction += GUIWindowManager_OnNewAction;

          //  GUIWindowManager.Receivers += GUIGraphicsContext_Receivers;
            _secondTimer = new Timer( SecondTimerTick, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            _lastPlayBackChanged = DateTime.Now;
        }

        public void Shutdown()
        {
            _secondTimer.Change(Timeout.Infinite, Timeout.Infinite);
            GUIWindowManager.OnActivateWindow -= GUIWindowManager_OnActivateWindow;
            g_Player.PlayBackStarted -= Player_PlayBackStarted;
            g_Player.PlayBackStopped -= Player_PlayBackStopped;
            g_Player.PlayBackEnded -= Player_PlayBackEnded;
            g_Player.PlayBackChanged -= Player_PlayBackChanged;
            GUIWindowManager.OnNewAction -= GUIWindowManager_OnNewAction;

            ListManager.Instance.Shutdown();
            PropertyManager.Instance.Shutdown();
            EqualizerManager.Instance.Shutdown();
            DialogManager.Instance.Shutdown();
        }

       

        private void SecondTimerTick(object state)
        {
            CheckUserInteraction();
        }

        private void CheckUserInteraction()
        {
            if (_isUserInteracting)
            {
                if (DateTime.Now > _lastIteraction.AddSeconds(_settings.UserInteractionDelay))
                {
                    _isUserInteracting = false;
                    _lastIteraction = DateTime.Now.AddYears(1);
                    OnUserInteractionEnded();
                }
            }
        }

        private void ResetUserInteraction()
        {
            OnUserInteractionStarted();
            _isUserInteracting = true;
            _lastIteraction = DateTime.Now;
        }

        private void OnUserInteractionEnded()
        {
            if (_currentPlaybackType.IsMusic() && !_isFullScreenMusic)
            {
                _isFullScreenMusic = true;
                SendPlayerMessage();
                EqualizerManager.Instance.StartEqualizer();
            }
        }

        private void OnUserInteractionStarted()
        {
            if (_currentPlaybackType.IsMusic() && _isFullScreenMusic)
            {
                _isFullScreenMusic = false;
                EqualizerManager.Instance.StopEqualizer();
                SendPlayerMessage();
            }
        }
     
        private void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
         
                switch (action.wID)
                {
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_DOWN:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_LEFT:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_RIGHT:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOVE_UP:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_SHOW_ACTIONMENU:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_PREVIOUS_MENU:
                    case MediaPortal.GUI.Library.Action.ActionType.ACTION_MOUSE_CLICK:
                        ResetUserInteraction();
                        break;
                    default:
                        break;
                }


            // workaround for MovingPictures (WindowID 96742): When the detailed screen for a movie is selected focus is still on the movie selection (ControlID 50), but not on the first item of the
            // detail screen (control 6). Therefore, instead of sending the actual focussed control 50 send control ID 6. If selecting a movie directly plays the movie (instead of the detail screen)
            // the detail screen will shortly display on MPD, then the player screen will be activated.
            if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM || action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_KEY_PRESSED) &&
                    _currentWindow.GetID == 96742 && CurrentWindowFocusedControlId == 50)
            {
                SendFocusedControlMessage(6);
            }
            else
            {
                SendFocusedControlMessage(-1);
            }

            if (action.wID != MediaPortal.GUI.Library.Action.ActionType.ACTION_MOUSE_MOVE)
            {
                SendActionIdMessage((int)action.wID);
            }
        }

        private void GUIWindowManager_OnActivateWindow(int windowId)
        {
            ThreadPool.QueueUserWorkItem((o) => SetCurrentWindow(windowId));
        }


        private void GUIWindowManager_Receivers(GUIMessage message)
        {
       //  GUIWaitCursor
            //Log.Message(LogLevel.Debug, "[GUIWindowManager_Receivers] - Message: {0}, SenderControlId: {1}, SendToTargetWindow: {2}, TargetControlId: {3}, TargetWindowId: {4}"
            //    + ", TargetWindowId: {5}, TargetWindowId: {6}, TargetWindowId: {7}, TargetWindowId: {8}"
            //    +", TargetWindowId: {9}, TargetWindowId: {10}"
            //    +", TargetWindowId: {11}, TargetWindowId: {12}, TargetWindowId: {13}, TargetWindowId: {14}"
            //  , message.Message, message.SenderControlId, message.SendToTargetWindow, message.TargetControlId, message.TargetWindowId
            //  ,message.Label, message.Label2, message.Label3, message.Label4
            //  , message.Object, message.Object2
            //  , message.Param1, message.Param2, message.Param3, message.Param4);
        }

        public void OnMediaPortalMessageReceived(APIMediaPortalMessage message)
        {
            if (message != null)
            {
                Log.Message(LogLevel.Debug, "[ReceiveMediaPortalMessage] - MediaPortalMessage received from MPDisplay, MessageType: {0}", message.MessageType);
                if (message.MessageType == APIMediaPortalMessageType.WindowInfoMessage)
                {
                    if (message.WindowMessage != null)
                    {
                        PropertyManager.Instance.RegisterWindowProperties(message.WindowMessage.Properties);
                        ListManager.Instance.RegisterWindowListTypes(message.WindowMessage.Lists);
                        EqualizerManager.Instance.RegisterEqualizer(message.WindowMessage.EQData);
                    }
                }
                else if (message.MessageType == APIMediaPortalMessageType.DialogInfoMessage)
                {
                    if (message.WindowMessage != null)
                    {
                        DialogManager.Instance.RegisterDialogInfo(message.WindowMessage);
                    }
                }

                if (message.MessageType == APIMediaPortalMessageType.ActionMessage)
                {
                    if (message.ActionMessage != null)
                    {
                        if (message.ActionMessage.ActionType == APIActionMessageType.WindowListAction)
                        {
                            ListManager.Instance.OnActionMessageReceived(message.ActionMessage);
                        }
                        else if (message.ActionMessage.ActionType == APIActionMessageType.DialogListAction)
                        {
                            DialogManager.Instance.OnActionMessageReceived(message.ActionMessage);
                        }
                        else if (message.ActionMessage.ActionType == APIActionMessageType.GuideAction)
                        {
                            TVServerManager.Instance.OnActionMessageReceived(message.ActionMessage);
                        }
                        else
                        {
                            if (message.ActionMessage.MediaPortalAction != null)
                            {
                                SupportedPluginManager.GUISafeInvoke(() =>
                                {
                                    if (message.ActionMessage.ActionType == APIActionMessageType.MediaPortalAction)
                                    {
                                        GUIGraphicsContext.OnAction(new MediaPortal.GUI.Library.Action((MediaPortal.GUI.Library.Action.ActionType)(int)message.ActionMessage.MediaPortalAction.ActionId, 0f, 0f));
                                    }

                                    if (message.ActionMessage.ActionType == APIActionMessageType.MediaPortalWindow)
                                    {
                                        GUIWindowManager.ActivateWindow(message.ActionMessage.MediaPortalAction.ActionId);
                                    }
                                });
                            }
                        }
                    }
                }
            }
        }

    
        public void SendFullUpdate()
        {
            Log.Message(LogLevel.Debug, "[SendFullUpdate] - Sending full information update");
            SetCurrentWindow(GUIWindowManager.ActiveWindow);
            SendPlayerMessage();
            TVServerManager.Instance.SendTvGuide();
            TVServerManager.Instance.SendRecordings();
        }

        private void SetCurrentWindow(int windowId)
        {
            if (!GUIWindowManager.IsRouted)
            {
                ListManager.Instance.ClearWindowListControls();
                PropertyManager.Instance.Suspend(true);

                int retry = 0;
                while (GUIWindowManager.IsSwitchingToNewWindow || !GUIWindowManager.Initalized)
                {
                    Log.Message(LogLevel.Debug, "[SetCurrentWindow] - Waiting for window to initalize, WindowId: {0}", windowId);
                    Thread.Sleep(200);
                    if (retry > 100)
                    {
                        Log.Message(LogLevel.Error, "[SetCurrentWindow] - I've been waiting for ages... So I am giving up.., WindowId: {0}", windowId);
                        break;
                    }
                    retry++;
                }

                _currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
                bool fullscreen = GUIGraphicsContext.IsFullScreenVideo || _currentWindow.GetID == 2005 || _currentWindow.GetID == 602;

                if (!fullscreen)
                {
                    _currentPlugin = SupportedPluginManager.GetPluginHelper(WindowManager.Instance.CurrentWindow.GetID);
                    SendWindowMessage();
                    ListManager.Instance.SetWindowListControls();
                    if (WindowManager.Instance.CurrentWindow.GetID == 600 || WindowManager.Instance.CurrentWindow.GetID == 604)
                    {
                        var firstEPG = WindowManager.Instance.CurrentWindow.GetControls().FirstOrDefault(c => c.GetID > 50000);
                        if (firstEPG != null)
                        {
                            SendFocusedControlMessage(firstEPG.GetID);
                        }
                    }
                }

                // if fullscreen state has changed send player update
                if (fullscreen != _isFullscreenVideo)
                {
                    _isFullscreenVideo = fullscreen;
                    SendPlayerMessage();
                }
                // SendFocusedControlMessage();
                PropertyManager.Instance.Suspend(false);

            }
         
        }

        private void SendWindowMessage()
        {
            if (_currentWindow != null && MessageService.Instance.IsMPDisplayConnected)
            {
                Log.Message(LogLevel.Debug, "[SendWindowMessage] - WindowId: {0}, FocusedControlId: {1}"
                   , _currentWindow.GetID, _currentWindow.GetFocusControlId());
                MessageService.Instance.SendInfoMessage(new APIInfoMessage
                {
                    MessageType = APIInfoMessageType.WindowMessage,
                    WindowMessage = new APIWindowMessage
                    {
                        WindowId = _currentWindow.GetID,
                        FocusedControlId = _currentWindow.GetFocusControlId(),
                        EnabledPlugins = _enabledlugins
                    }
                });

                SendEditorData(APISkinEditorDataType.WindowId, _currentWindow.GetID);
            }
        }

        private void SendFocusedControlMessage(int controlID)
        {
            if (_currentWindow != null && MessageService.Instance.IsMPDisplayConnected)
            {
                int channelId = -1;
                int programId = -1;

                int focusId = CurrentWindowFocusedControlId;
                if (controlID >= 0) focusId = controlID;

                // If control is a TVGuide item send also programId and channelId
                if ((_currentWindow.GetID == 600 || _currentWindow.GetID == 604) && focusId >= 50000)
                {
                        var item = _currentWindow.GetControl(focusId);
                        var program = item.Data;
                        channelId = ReflectionHelper.GetPropertyValue<int>(program, "IdChannel", -1);
                        programId = ReflectionHelper.GetPropertyValue<int>(program, "IdProgram", -1);
                 }
 
                if (focusId != _previousFocusedControlId || programId > 0)
                {
                _previousFocusedControlId = focusId;
                Log.Message(LogLevel.Debug, "[SendFocusedControlId] - FocusedControlId: {0}", focusId);
                    MessageService.Instance.SendInfoMessage(new APIInfoMessage
                    {
                        MessageType = APIInfoMessageType.WindowMessage,
                        WindowMessage = new APIWindowMessage
                        {
                            MessageType = APIWindowMessageType.FocusedControlId,
                            FocusedControlId = focusId,
                            ProgramId = programId,
                            ChannelId = channelId
                        }
                    });

                    SendEditorData(APISkinEditorDataType.FocusedControlId, focusId);
                }
            }
        }

       

        private void SendPlayerMessage()
        {
            if (_currentWindow != null && MessageService.Instance.IsMPDisplayConnected)
            {
                var message = new APIPlayerMessage
                    {
                        PlaybackState = _currentPlaybackState,
                        PlaybackType = _currentPlaybackType,
                        PlayerPluginType = _currentPlayerPlugin,
                        PlayerFullScreen = _isFullscreenVideo || _isFullScreenMusic
                    };

                if (!message.IsEquals(_lastPlayerMessage))
                {
                    Log.Message(LogLevel.Debug, "[SendPlayerMessage] - PlaybackState: {0}, PlaybackType: {1}, PlayerPluginType: {2}, FullScreen: {3}"
                                                                , _currentPlaybackState, _currentPlaybackType, _currentPlayerPlugin, _isFullScreenMusic || _isFullscreenVideo);
                    _lastPlayerMessage = message;
                    MessageService.Instance.SendInfoMessage(new APIInfoMessage
                    {
                        MessageType = APIInfoMessageType.PlayerMessage,
                        PlayerMessage = message
                    });
                }
            }
        }


        private void SendActionIdMessage(int actionId)
        {
            if (MessageService.Instance.IsMPDisplayConnected)
            {
                MessageService.Instance.SendDataMessage(new APIDataMessage
                {
                    DataType = APIDataMessageType.MPActionId,
                    IntValue = actionId
                });
            }
        }


        #region Player

        private void Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            if (DateTime.Now > _lastPlayBackChanged.AddSeconds(3))                                  // Ignore Player Ended event due to MovingPictures Bug when playing multiple files
            {

                Log.Message(LogLevel.Debug, "[Player_PlayBackEnded] - PlayType: {0}", type);
                EqualizerManager.Instance.StopEqualizer();
                _currentPlaybackState = APIPlaybackState.Stopped;
                _currentPlaybackType = APIPlaybackType.None;
                _currentPlayerPlugin = APIPlaybackType.None;
                _isFullScreenMusic = false;
                _isFullscreenVideo = false;
                SendPlayerMessage();
            }
        }

        private void Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackStopped] - PlayType: {0}", type);
            EqualizerManager.Instance.StopEqualizer();
            _currentPlaybackState = APIPlaybackState.Stopped;
            _currentPlaybackType = APIPlaybackType.None;
            _currentPlayerPlugin = APIPlaybackType.None;
            _isFullScreenMusic = false;
            _isFullscreenVideo = false;
            SendPlayerMessage();
        }

        void Player_PlayBackChanged(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackChanged] - PlayType: {0}", type);
            if (type == g_Player.MediaType.Video)
            {
                _lastPlayBackChanged = DateTime.Now;                                                // remember time of this event for workaround MovingPictures bug when chanhing files
            }
        }
        
        private void Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackStarted] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Playing;
            _currentPlaybackType = GetPlaybackType(type);
            _currentPlayerPlugin = SupportedPluginManager.GetPluginPlayerType(_currentPlaybackType, filename);
            _isFullscreenVideo = _currentPlaybackType.IsVideo();
            _isFullScreenMusic = _currentPlaybackType.IsMusic();
            SendPlayerMessage();
            if (_isFullScreenMusic)
            {
                EqualizerManager.Instance.StartEqualizer();
            }
        }

        private APIPlaybackType GetPlaybackType(g_Player.MediaType type)
        {
            switch (type)
            {
                case g_Player.MediaType.Music:
                    return APIPlaybackType.IsMusic;
                case g_Player.MediaType.Radio:
                    return APIPlaybackType.IsRadio;
                case g_Player.MediaType.RadioRecording:
                    return APIPlaybackType.IsTVRecording;
                case g_Player.MediaType.Recording:
                    return APIPlaybackType.IsTVRecording;
                case g_Player.MediaType.TV:
                    return APIPlaybackType.IsTV;
                case g_Player.MediaType.Unknown:
                    return APIPlaybackType.None;
                case g_Player.MediaType.Video:
                    return APIPlaybackType.IsVideo;
                default:
                    break;
            }
            return APIPlaybackType.None;
        }

        #endregion


        private void SendEditorData(APISkinEditorDataType type, int value)
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

    }


    
}
