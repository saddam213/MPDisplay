﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Common.Helpers;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;

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
    

        public GUIWindow CurrentWindow
        {
            get { return _currentWindow; }
        }

        public int CurrentWindowFocusedControlId
        {
            get { return _currentWindow != null ? _currentWindow.GetFocusControlId() : -1; }
        }

        public void Initialize()
        {
            ListManager.Instance.Initialize();
            PropertyManager.Instance.Initialize();

            GUIWindowManager.OnActivateWindow += GUIWindowManager_OnActivateWindow;
            g_Player.PlayBackStarted += Player_PlayBackStarted;
            g_Player.PlayBackStopped += Player_PlayBackStopped;
            g_Player.PlayBackEnded += Player_PlayBackEnded;
            g_Player.PlayBackChanged += Player_PlayBackChanged;

            GUIWindowManager.Receivers += GUIGraphicsContext_Receivers;

        }


        void GUIGraphicsContext_Receivers(GUIMessage message)
        {
         

          
            
        }

        void GUIGraphicsContext_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
         
        }

        void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
          
        }

        void GUIWindowManager_Receivers(GUIMessage message)
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

      

        public void Shutdown()
        {
            GUIWindowManager.OnActivateWindow -= GUIWindowManager_OnActivateWindow;
            g_Player.PlayBackStarted -= Player_PlayBackStarted;
            g_Player.PlayBackStopped -= Player_PlayBackStopped;
            g_Player.PlayBackEnded -= Player_PlayBackEnded;

            ListManager.Instance.Shutdown();
            PropertyManager.Instance.Shutdown();
        }


        private void GUIWindowManager_OnActivateWindow(int windowId)
        {
         
            ThreadPool.QueueUserWorkItem((o) => SetCurrentWindow(windowId));
        }

        public void OnWindowMessageReceived(APIWindowInfoMessage message)
        {
            PropertyManager.Instance.RegisterWindowProperties(message.Properties);
            ListManager.Instance.RegisterWindowListTypes(message.Lists);
        }

        public void OnDialogMessageReceived(APIWindowInfoMessage message)
        {
           
        }

        public void OnActionMessageReceived(APIActionMessage action)
        {
           
        }


        private void SetCurrentWindow(int windowId)
        {
            ListManager.Instance.ClearWindowListControls();
            while (GUIWindowManager.IsSwitchingToNewWindow)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(250);
            _isFullscreenVideo = (windowId == 2005 || windowId == 602);
            if (!_isFullscreenVideo)
            {
                _currentWindow = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow);
                SendWindowMessage();
                ListManager.Instance.SetWindowListControls();
            }

          
        }

        private void SendWindowMessage()
        {
            if (_currentWindow != null)
            {
                //Log.Message(LogLevel.Debug, "[SendWindowMessage] - WindowId: {0}, FocusedControlId: {1}"
                //   , _currentWindow.GetID, _currentWindow.GetFocusControlId());
                MessageService.Instance.SendInfoMessage(new APIInfoMessage
                {
                    MessageType = APIInfoMessageType.WindowMessage,
                    WindowMessage = new APIWindowMessage
                    {
                        WindowId = _currentWindow.GetID,
                        FocusedControlId = _currentWindow.GetFocusControlId(),
                    }
                });
            }
        }

        private void SendPlayerMessage()
        {
            if (_currentWindow != null)
            {
                //Log.Message(LogLevel.Debug, "[SendPlayerMessage] - PlaybackState: {0}, PlaybackType: {1}, PlayerPluginType: {2}"
                //    , _currentPlaybackState, _currentPlaybackType, _currentPlayerPlugin);
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

        private APIPlaybackState _currentPlaybackState = APIPlaybackState.None;
        private APIPlaybackType _currentPlaybackType = APIPlaybackType.None;
        private APIPlaybackType _currentPlayerPlugin = APIPlaybackType.None;
        private Dictionary<int, APIPlaybackType> _playerWindows = new Dictionary<int, APIPlaybackType>();
  

        private void Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackEnded] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Stopped;
          
        }

        private void Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackStopped] - PlayType: {0}", type);
            _currentPlaybackState = APIPlaybackState.Stopped;
        }

        private int _playerId = -1;

        private void Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            var player = g_Player.Player;
           // Log.Message(LogLevel.Debug, "[Player_PlayBackStarted] - PlayType: {0}", type);
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
                    _playerId = ListManager.Instance.LastSelectedItemWindowId;
                   Log.Message(LogLevel.Debug, "[Player_PlayBackStarted] - PlayerPlugin: {0}", _playerId);
                }
            }
        }


     
    


        void Player_PlayBackChanged(g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Message(LogLevel.Debug, "[Player_PlayBackChanged] - PlayType: {0}", type);
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

        private APIPlaybackType GetPluginPlayerType(g_Player.MediaType type)
        {
            if (_currentWindow != null)
            {
                if (_playerWindows.ContainsKey(_currentWindow.GetID))
                {
                    return _playerWindows[_currentWindow.GetID];
                }

                return GetPlaybackType(type);
            }
            return APIPlaybackType.None;
        }

     
    
    }
}
