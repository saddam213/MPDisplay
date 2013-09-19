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
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;
using System.Xml.Linq;
using MediaPortal.Profile;

namespace MediaPortalPlugin.InfoManagers
{
    

    public class WindowManager
    {
        #region Singleton Implementation

        private static WindowManager instance;

        private WindowManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(WindowManager));
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

        private MPDisplay.Common.Log.Log Log;
        private bool _isFullscreenVideo;
        private GUIWindow _currentWindow;
        private PluginSettings _settings;
        private int _previousFocusedControlId = -1;
        private Dictionary<int, APIPlaybackType> _playerWindows = new Dictionary<int, APIPlaybackType>();
        private APIPlaybackState _currentPlaybackState = APIPlaybackState.None;
        private APIPlaybackType _currentPlaybackType = APIPlaybackType.None;
        private APIPlaybackType _currentPlayerPlugin = APIPlaybackType.None;
    
        public GUIWindow CurrentWindow
        {
            get { return _currentWindow; }
        }

        public int CurrentWindowFocusedControlId
        {
            get { return _currentWindow != null ? _currentWindow.GetFocusControlId() : -1; }
        }

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;

            if (_enabledlugins == null)
            {
                _enabledlugins = MPSettings.Instance.GetSection<string>("plugins").Where(kv => kv.Value == "yes").Select(kv => kv.Key).ToList();
            }

            LoadPlayerPluginIds(_settings.PlayerPlugins);

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

        }

      

      

        public void Shutdown()
        {
            GUIWindowManager.OnActivateWindow -= GUIWindowManager_OnActivateWindow;
            g_Player.PlayBackStarted -= Player_PlayBackStarted;
            g_Player.PlayBackStopped -= Player_PlayBackStopped;
            g_Player.PlayBackEnded -= Player_PlayBackEnded;

            ListManager.Instance.Shutdown();
            PropertyManager.Instance.Shutdown();
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
                    SendInteractionMessage();
                    break;
                default:
                    break;
            }

            SendFocusedControlMessage();

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
                if (message.MessageType == APIMediaPortalMessageType.KeepAlive)
                {
                    return;
                }

                Log.Message(LogLevel.Verbose, "[ReceiveMediaPortalMessage] - MediaPortalMessage received from MPDisplay, MessageType: {0}", message.MessageType);
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
                        else
                        {
                            if (message.ActionMessage.MediaPortalAction != null)
                            {
                                PluginHelpers.GUISafeInvoke(() =>
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

        private List<string> _enabledlugins;

        public void SendFullUpdate()
        {
            Log.Message(LogLevel.Info, "[SendFullUpdate] - Sending full information update");
            SetCurrentWindow(GUIWindowManager.ActiveWindow); 
            SendPlayerMessage();

        
        }
    
        private void SetCurrentWindow(int windowId)
        {
            ListManager.Instance.ClearWindowListControls();
            PropertyManager.Instance.Suspend = true;
            int retry = 0;
            while (GUIWindowManager.IsSwitchingToNewWindow || !GUIWindowManager.Initalized)
            {
                Log.Message(LogLevel.Verbose, "[SetCurrentWindow] - Waiting 100ms for window to initalize, WindowId: {0}", windowId);
                Thread.Sleep(200);
                if (retry > 100)
                {
                    Log.Message(LogLevel.Error, "[SetCurrentWindow] - I've been waiting for ages... So I am giving up.., WindowId: {0}", windowId);
                    break;
                }
                retry++;
            }
            _isFullscreenVideo = (windowId == 2005 || windowId == 602 || GUIGraphicsContext.IsFullScreenVideo);
            if (!_isFullscreenVideo)
            {
                _currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
                SendWindowMessage();
                ListManager.Instance.SetWindowListControls();
            }
            SendFocusedControlMessage();
        }




        private void SendWindowMessage()
        {
            if (_currentWindow != null)
            {
                Log.Message(LogLevel.Info, "[SendWindowMessage] - WindowId: {0}, FocusedControlId: {1}"
                   , _currentWindow.GetID, _currentWindow.GetFocusControlId());
                MessageService.Instance.SendInfoMessage(new APIInfoMessage
                {
                    MessageType = APIInfoMessageType.WindowMessage,
                    WindowMessage = new APIWindowMessage
                    {
                        
                        WindowId = _currentWindow.GetID,
                        FocusedControlId = _currentWindow.GetFocusControlId(),
                        IsFullscreenVideo = _isFullscreenVideo,
                        EnabledPlugins = _enabledlugins
                    }
                });
            }
        }

        private void SendFocusedControlMessage()
        {
            if (_currentWindow != null)
            {
                int focusId = CurrentWindowFocusedControlId;
                if (focusId != _previousFocusedControlId)
                {
                    _previousFocusedControlId = focusId;
                    Log.Message(LogLevel.Info, "[SendFocusedControlId] - FocusedControlId: {0}", focusId);
                    MessageService.Instance.SendInfoMessage(new APIInfoMessage
                    {
                        MessageType = APIInfoMessageType.WindowMessage,
                        WindowMessage = new APIWindowMessage
                        {
                            MessageType = APIWindowMessageType.FocusedControlId,
                            FocusedControlId = focusId
                        }
                    });
                }
            }
        }

        private void SendPlayerMessage()
        {
            if (_currentWindow != null)
            {
                Log.Message(LogLevel.Info, "[SendPlayerMessage] - PlaybackState: {0}, PlaybackType: {1}, PlayerPluginType: {2}"
                    , _currentPlaybackState, _currentPlaybackType, _currentPlayerPlugin);
                MessageService.Instance.SendInfoMessage(new APIInfoMessage
                {
                    MessageType = APIInfoMessageType.PlayerMessage,
                    PlayerMessage = new APIPlayerMessage
                    {
                        PlaybackState = _currentPlaybackState,
                        PlaybackType = _currentPlaybackType,
                        PlayerPluginType = _currentPlayerPlugin
                    }
                });
            }
        }

        private void SendInteractionMessage()
        {
            MessageService.Instance.SendDataMessage(new APIDataMessage
            {
                DataType = APIDataMessageType.ResetIteraction
            });
        }

        private void SendActionIdMessage(int actionId)
        {
            MessageService.Instance.SendDataMessage(new APIDataMessage
            {
                DataType = APIDataMessageType.MPActionId,
                IntValue = actionId
            });
        }


        #region Player

        private void LoadPlayerPluginIds(IEnumerable<PlayerPlugin> pluginInfo)
        {
            if (pluginInfo != null && pluginInfo.Any())
            {
                _playerWindows.Clear();
                foreach (var item in pluginInfo)
                {
                    APIPlaybackType pluginType = (APIPlaybackType)Enum.Parse(typeof(APIPlaybackType), item.PluginName);
                    foreach (var id in item.WindowIds)
                    {
                        if (!_playerWindows.ContainsKey(id))
                        {
                            _playerWindows.Add(id, pluginType);
                        }
                    }
                }
            }
        }

        private void Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            Log.Message(LogLevel.Info, "[Player_PlayBackEnded] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Stopped;
            _currentPlaybackType = APIPlaybackType.None;
            _currentPlayerPlugin = APIPlaybackType.None;
            SendPlayerMessage();
        }

        private void Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Info, "[Player_PlayBackStopped] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Stopped;
            _currentPlaybackType = APIPlaybackType.None;
            _currentPlayerPlugin = APIPlaybackType.None;
            SendPlayerMessage();
        }

        void Player_PlayBackChanged(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Info, "[Player_PlayBackChanged] - PlayType: {0}", type);
        }

        private void Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            Log.Message(LogLevel.Info, "[Player_PlayBackStarted] - PlayType: {0}", type);

            _currentPlaybackState = APIPlaybackState.Started;
            _currentPlaybackType = GetPlaybackType(type);
            _currentPlayerPlugin = _currentPlaybackType;

            if (ListManager.Instance.LastSelectedItem != null)
            {
                bool isLastSelectedPlaying = false;
                if (ListManager.Instance.LastSelectedItem.Path == filename)
                {
                    isLastSelectedPlaying = true;
                }
                else if (ListManager.Instance.LastSelectedItem.TVTag != null)
                {
                    if (ReflectionHelper.FindStringValue(ListManager.Instance.LastSelectedItem.TVTag, filename))
                    {
                        isLastSelectedPlaying = true;
                    }
                    else
                    {
                        // TVSeries
                        if (ListManager.Instance.LastSelectedItem.GetMPTVSeriesItemFilename() == filename)
                        {
                            isLastSelectedPlaying = true;
                        }
                    }
                }

                if (isLastSelectedPlaying)
                {
                    _currentPlayerPlugin = GetPluginPlayerType(ListManager.Instance.LastSelectedItemWindowId, type);
                }
            }

            SendPlayerMessage();
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

        private APIPlaybackType GetPluginPlayerType(int windowId, g_Player.MediaType defaultType)
        {
            if (windowId != -1 && _playerWindows.ContainsKey(windowId))
            {
                return _playerWindows[windowId];
            }
            return GetPlaybackType(defaultType);
        }

        #endregion
   
    }


    
}
