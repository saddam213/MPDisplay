using System;
using System.Collections.Generic;
using System.Linq;
using Common.MessengerService;
using Common.Settings;
using GUIFramework.Managers;
using GUIFramework.Utils;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIFramework.Repositories
{
    public class InfoRepository : IRepository
    {
        #region Singleton Implementation

        private InfoRepository() { }
        private static InfoRepository _instance;
        public static InfoRepository Instance
        {
            get { return _instance ?? (_instance = new InfoRepository()); }
        }

        public static void RegisterMessage<T>(InfoMessageType message, Action<T> callback)
        {
            Instance.InfoService.Register(message, callback);
        }

        public static void RegisterMessage<T, TU>(InfoMessageType message, Action<T, TU> callback)
        {
            Instance.InfoService.Register(message, callback);
        }

        public static void DeregisterMessage(InfoMessageType message, object owner)
        {
            Instance.InfoService.Deregister(message, owner);
        }

        public static void NotifyListeners<T>(InfoMessageType message, T value)
        {
            Instance.NotifiyValueChanged(message, value);
        }

        #endregion

        #region Fields

        private MessengerService<InfoMessageType> _infoService = new MessengerService<InfoMessageType>();
        private List<string> _currentEnabledPluginMap = new List<string>();
        private APIPlaybackType _playerType = APIPlaybackType.None;
        private APIPlaybackType _playbackType = APIPlaybackType.None;
        private APIPlaybackState _playbackState = APIPlaybackState.None;
        // private int _mediaPortalPlayerId = -1;
        private bool _isTvRecording;
        private bool _isFullscreenVideo;
        private int _mediaPortalWindowId = -1;
        private int _mediaPortalDialogId = -1;
        private int _previousWindowId = -1;
        private int _focusedDialogControlId = -1;
        private int _focusedWindowControlId = -1;
        private int _focusedProgramId = -1;
        private int _focusedChannelId = -1;
        private bool _isMediaPortalConnected;
        private bool _isMPDisplayConnected;
        private bool _isFullscreenMusic;

        #endregion

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }


        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
        }

        public void ClearRepository()
        {
            _currentEnabledPluginMap = new List<string>();
            _playerType = APIPlaybackType.None;
            _playbackType = APIPlaybackType.None;
            _playbackState = APIPlaybackState.None;
            _isTvRecording = false;
            _isFullscreenVideo = false;
            _mediaPortalWindowId = -1;
            _mediaPortalDialogId = -1;
            _previousWindowId = -1;
            _focusedDialogControlId = -1;
            _focusedWindowControlId = -1;
           
        }

        public void ResetRepository()
        {
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }


        #region Properties

        public MessengerService<InfoMessageType> InfoService
        {
            get { return _infoService; }
        }

        /// <summary>
        /// Gets the current enabled plugin map.
        /// </summary>
        public List<string> EnabledPluginMap
        {
            get { return _currentEnabledPluginMap; }
            set
            {
                if (value == null) return;
                if (_currentEnabledPluginMap != null && _currentEnabledPluginMap.SequenceEqual(value)) return;
                _currentEnabledPluginMap = value;
                NotifiyValueChanged(InfoMessageType.EnabledPluginMap, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            }
        }

        /// <summary>
        /// Gets the type of the current media portal player.
        /// </summary>
        public APIPlaybackType PlayerType
        {
            get { return _playerType; }
            set
            {
                if (_playerType == value) return;
                _playerType = value;
                NotifiyValueChanged(InfoMessageType.PlayerType, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
            }
        }

        public APIPlaybackType PlaybackType
        {
            get { return _playbackType; }
            set
            {
                if (_playbackType == value) return;
                _playbackType = value;
                NotifiyValueChanged(InfoMessageType.PlaybackType, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
            }
        }

        public APIPlaybackState PlaybackState
        {
            get { return _playbackState; }
            set
            {
                if (_playbackState == value) return;
                _playbackState = value;
                NotifiyValueChanged(InfoMessageType.PlaybackState, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is media portal tv recording.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is media portal tv recording; otherwise, <c>false</c>.
        /// </value>
        public bool IsTvRecording
        {
            get { return _isTvRecording; }
            set
            {
                if (_isTvRecording == value) return;
                _isTvRecording = value;
                NotifiyValueChanged(InfoMessageType.IsTvRecording, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is media portal fullscreen video.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is media portal fullscreen video; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullscreenVideo
        {
            get { return _isFullscreenVideo; }
            set
            {
                if (_isFullscreenVideo == value) return;
                _isFullscreenVideo = value;
                NotifiyValueChanged(InfoMessageType.IsFullscreenMedia, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            }
        }

        public bool IsFullscreenMusic
        {
            get { return _isFullscreenMusic; }
            set
            {
                if (_isFullscreenMusic == value) return;
                _isFullscreenMusic = value;
                NotifiyValueChanged(InfoMessageType.IsFullscreenMedia, value);
            }
        }

        /// <summary>
        /// Gets the current media portal window id.
        /// </summary>
        public int WindowId
        {
            get { return _mediaPortalWindowId; }
            set
            {
                if (_mediaPortalWindowId == value) return;
                _mediaPortalWindowId = value;
                NotifiyValueChanged(InfoMessageType.WindowId, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
            }
        }

        /// <summary>
        /// Gets the current media portal dialog id.
        /// </summary>
        public int DialogId
        {
            get { return _mediaPortalDialogId; }
            set
            {
                if (_mediaPortalDialogId == value) return;
                _mediaPortalDialogId = value;
                NotifiyValueChanged(InfoMessageType.DialogId, value);
            }
        }

        /// <summary>
        /// Gets the previous media portal window id.
        /// </summary>
        public int PreviousWindowId
        {
            get { return _previousWindowId; }
            set
            {
                if (_previousWindowId == value) return;
                _previousWindowId = value;
                NotifiyValueChanged(InfoMessageType.PreviousWindowId, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
            }
        }

        /// <summary>
        /// Gets the current focused media portal control id.
        /// </summary>
        public int FocusedWindowControlId
        {
            get { return _focusedWindowControlId; }
            set
            {
                if (_focusedWindowControlId == value) return;
                _focusedWindowControlId = value;
                NotifiyValueChanged(InfoMessageType.FocusedWindowControlId, value);
                GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
            }
        }


        /// <summary>
        /// Gets the current focused media portal program id in TVGuide.
        /// </summary>
        public int FocusedProgramId
        {
            get { return _focusedProgramId; }
            set
            {
                if (_focusedProgramId == value) return;
                _focusedProgramId = value;
                if( _focusedProgramId > 0 && _focusedChannelId > 0 ) NotifiyValueChanged(InfoMessageType.FocusedTVGuideId, _focusedProgramId, _focusedChannelId);
            }
        }
        /// <summary>
        /// Gets the current focused media portal channel id in TVGuide.
        /// </summary>
        public int FocusedChannelId
        {
            get { return _focusedChannelId; }
            set
            {
                if (_focusedChannelId == value) return;
                _focusedChannelId = value;
                if (_focusedProgramId > 0 && _focusedChannelId > 0) NotifiyValueChanged(InfoMessageType.FocusedTVGuideId, _focusedProgramId, _focusedChannelId);
            }
        }


        /// <summary>
        /// Gets the current focused media portal dialog control id.
        /// </summary>
        public int FocusedDialogControlId
        {
            get { return _focusedDialogControlId; }
            set
            {
                if (_focusedDialogControlId == value) return;
                _focusedDialogControlId = value;
                NotifiyValueChanged(InfoMessageType.FocusedDialogControlId, value);
            }
        }


        public bool IsMPDisplayConnected
        {
            get { return _isMPDisplayConnected; }
            set
            {
                if (_isMPDisplayConnected == value) return;
                _isMPDisplayConnected = value;
                NotifiyValueChanged(InfoMessageType.IsMPDisplayConnected, value);
            }
        }

        public bool IsMediaPortalConnected
        {
            get { return _isMediaPortalConnected; }
            set
            {
                if (_isMediaPortalConnected == value) return;
                _isMediaPortalConnected = value;
                NotifiyValueChanged(InfoMessageType.IsMediaPortalConnected, value);
            }
        }

        #endregion


        public bool IsSkinOptionEnabled(string option)
        {
            var skinOption = SkinInfo.SkinOptions.FirstOrDefault(o => o.Name == option);
            return skinOption != null && skinOption.IsEnabled;
        }


        public void NotifiyValueChanged<T>(InfoMessageType type, T value)
        {
            InfoService.NotifyListeners(type, value);
        }

        public void NotifiyValueChanged<T, TU>(InfoMessageType type, T value1, TU value2)
        {
            InfoService.NotifyListeners(type, value1, value2);
        }


        public void AddInfo(APIInfoMessage message)
        {
            if (message == null) return;

            if (message.MessageType == APIInfoMessageType.WindowMessage)
            {
                AddWindowMessage(message.WindowMessage);
            }

            if (message.MessageType == APIInfoMessageType.DialogMessage)
            {
                AddDialogMessage(message.DialogMessage);
            }

            if (message.MessageType == APIInfoMessageType.PlayerMessage)
            {
                AddPlayerMessage(message.PlayerMessage);
            }
        }

        private void AddPlayerMessage(APIPlayerMessage message)
        {
            if (message == null) return;
            PlayerType = message.PlayerPluginType;
            PlaybackType = message.PlaybackType;
            PlaybackState = message.PlaybackState;
            IsFullscreenVideo = message.PlaybackType.IsVideo() && message.PlayerFullScreen;
            IsFullscreenMusic = message.PlaybackType.IsMusic() && message.PlayerFullScreen;
        }

        private void AddWindowMessage(APIWindowMessage message)
        {
            if (message == null) return;

            switch (message.MessageType)
            {
                case APIWindowMessageType.WindowId:
                    WindowId = message.WindowId;
               
                    FocusedWindowControlId = -1;
                    FocusedWindowControlId = message.FocusedControlId;

                    if (message.EnabledPlugins != null)
                    {
                        EnabledPluginMap = message.EnabledPlugins;
                    }
                    break;

                case APIWindowMessageType.FocusedControlId:
                    FocusedWindowControlId = message.FocusedControlId;

                    FocusedChannelId = -1;
                    FocusedProgramId = -1;

                    // message contains EPG update
                    if (message.ProgramId > 0 && message.ChannelId > 0)
                    {
                        FocusedProgramId = message.ProgramId;
                        FocusedChannelId = message.ChannelId;
                    }
                    break;
            }
        }

        private void AddDialogMessage(APIDialogMessage message)
        {
            if (message == null) return;

            switch (message.MessageType)
            {
                case APIDialogMessageType.DialogId:
                    DialogId = message.DialogId;
                    FocusedDialogControlId = -1;
                    FocusedDialogControlId = message.FocusedControlId;
                    break;

                case APIDialogMessageType.FocusedControlId:
                    FocusedDialogControlId = message.FocusedControlId;
                    break;
            }
        }
    }

    public enum InfoMessageType
    {
        EnabledPluginMap,
        PlayerType,
        PlaybackType,
        PlaybackState,
        IsTvRecording,
        WindowId,
        DialogId,
        PreviousWindowId,
        FocusedWindowControlId,
        FocusedDialogControlId,
        FocusedTVGuideId,
        IsMPDisplayConnected,
        IsMediaPortalConnected,
        IsTVServerConnected,
        SendListItem,
        IsFullscreenMedia
    }
}
