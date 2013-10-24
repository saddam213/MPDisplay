using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUISkinFramework;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{
    public class InfoRepository : IRepository
    {
        #region Singleton Implementation

        private InfoRepository() { }
        private static InfoRepository _instance;
        public static InfoRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InfoRepository();
                }
                return _instance;
            }
        }

        public static void RegisterMessage<T>(InfoMessageType message, Action<T> callback)
        {
            Instance.InfoService.Register<T>(message, callback);
        }

        public static void DeregisterMessage(InfoMessageType message, object owner)
        {
            Instance.InfoService.Deregister(message, owner);
        }

        public static void NotifyListeners<T>(InfoMessageType message, T value)
        {
            Instance.NotifiyValueChanged<T>(message, value);
        }

        #endregion

        #region Fields

        private MessengerService<InfoMessageType> _infoService = new MessengerService<InfoMessageType>();
        private List<string> _currentEnabledPluginMap = new List<string>();
        private APIPlaybackType _playerType = APIPlaybackType.None;
        private APIPlaybackType _playbackType = APIPlaybackType.None;
        private APIPlaybackState _playbackState = APIPlaybackState.None;
        // private int _mediaPortalPlayerId = -1;
        private bool _isTvRecording = false;
        private bool _isFullscreenVideo = false;
        private int _mediaPortalWindowId = -1;
        private int _mediaPortalDialogId = -1;
        private int _previousWindowId = -1;
        private int _focusedDialogControlId = -1;
        private int _focusedWindowControlId = -1;
        private bool _isTVServerConnected;
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
                if (_currentEnabledPluginMap != null && value != null)
                {
                    if (!_currentEnabledPluginMap.SequenceEqual(value))
                    {
                        _currentEnabledPluginMap = value;
                        NotifiyValueChanged<List<string>>(InfoMessageType.EnabledPluginMap, value);
                        GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                    }
                }
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
                if (_playerType != value)
                {
                    _playerType = value;
                    NotifiyValueChanged<APIPlaybackType>(InfoMessageType.PlayerType, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public APIPlaybackType PlaybackType
        {
            get { return _playbackType; }
            set
            {
                if (_playbackType != value)
                {
                    _playbackType = value;
                    NotifiyValueChanged<APIPlaybackType>(InfoMessageType.PlaybackType, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
            }
        }

        public APIPlaybackState PlaybackState
        {
            get { return _playbackState; }
            set
            {
                if (_playbackState != value)
                {
                    _playbackState = value;
                    NotifiyValueChanged<APIPlaybackState>(InfoMessageType.PlaybackState, value);
                }
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
                if (_isTvRecording != value)
                {
                    _isTvRecording = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsTvRecording, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
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
                if (_isFullscreenVideo != value)
                {
                    _isFullscreenVideo = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsFullscreenMedia, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
            }
        }

        public bool IsFullscreenMusic
        {
            get { return _isFullscreenMusic; }
            set
            {
                if (_isFullscreenMusic != value)
                {
                    _isFullscreenMusic = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsFullscreenMedia, value);
                }
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
                if (_mediaPortalWindowId != value)
                {
                    _mediaPortalWindowId = value;
                    NotifiyValueChanged<int>(InfoMessageType.WindowId, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
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
                if (_mediaPortalDialogId != value)
                {
                    _mediaPortalDialogId = value;
                    NotifiyValueChanged<int>(InfoMessageType.DialogId, value);

                }
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
                if (_previousWindowId != value)
                {
                    _previousWindowId = value;
                    NotifiyValueChanged<int>(InfoMessageType.PreviousWindowId, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.GlobalVisibilityChanged);
                }
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
                if (_focusedWindowControlId != value)
                {
                    _focusedWindowControlId = value;
                    NotifiyValueChanged<int>(InfoMessageType.FocusedWindowControlId, value);
                    GUIVisibilityManager.NotifyVisibilityChanged(VisibleMessageType.ControlVisibilityChanged);
                }
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
                if (_focusedDialogControlId != value)
                {
                    _focusedDialogControlId = value;
                    NotifiyValueChanged<int>(InfoMessageType.FocusedDialogControlId, value);
                }
            }
        }


        public bool IsMPDisplayConnected
        {
            get { return _isMPDisplayConnected; }
            set
            {
                if (_isMPDisplayConnected != value)
                {
                    _isMPDisplayConnected = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsMPDisplayConnected, value);
                }
            }
        }

        public bool IsMediaPortalConnected
        {
            get { return _isMediaPortalConnected; }
            set
            {
                if (_isMediaPortalConnected != value)
                {
                    _isMediaPortalConnected = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsMediaPortalConnected, value);
                }
            }
        }

        public bool IsTVServerConnected
        {
            get { return _isTVServerConnected; }
            set
            {
                if (_isTVServerConnected != value)
                {
                    _isTVServerConnected = value;
                    NotifiyValueChanged<bool>(InfoMessageType.IsTVServerConnected, value);
                }
            }
        }

        #endregion


        public bool IsSkinOptionEnabled(string option)
        {
            var skinOption = SkinInfo.SkinOptions.FirstOrDefault(o => o.Name == option);
            if (skinOption != null)
            {
                return skinOption.IsEnabled;
            }
            return false;
        }


        public void NotifiyValueChanged<T>(InfoMessageType type, T value)
        {
            InfoService.NotifyListeners(type, value);
        }


        public void AddInfo(APIInfoMessage message)
        {
            if (message != null)
            {
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
        }






        private void AddPlayerMessage(APIPlayerMessage message)
        {
            if (message != null)
            {
                PlayerType = message.PlayerPluginType;
                PlaybackType = message.PlaybackType;
                PlaybackState = message.PlaybackState;
                IsFullscreenVideo = message.PlaybackType.IsVideo() && message.PlayerFullScreen;
                IsFullscreenMusic = message.PlaybackType.IsMusic() && message.PlayerFullScreen;
            }
        }

        private void AddWindowMessage(APIWindowMessage message)
        {
            if (message != null)
            {
                if (message.MessageType == APIWindowMessageType.WindowId)
                {
                    WindowId = message.WindowId;
               
                    FocusedWindowControlId = -1;
                    FocusedWindowControlId = message.FocusedControlId;

                    if (message.EnabledPlugins != null)
                    {
                        EnabledPluginMap = message.EnabledPlugins;
                    }

                }
                else if (message.MessageType == APIWindowMessageType.FocusedControlId)
                {
                    FocusedWindowControlId = message.FocusedControlId;
                }
            }
        }

        private void AddDialogMessage(APIDialogMessage message)
        {
            if (message != null)
            {
                if (message.MessageType == APIDialogMessageType.DialogId)
                {
                    DialogId = message.DialogId;
                    FocusedDialogControlId = -1;
                    FocusedDialogControlId = message.FocusedControlId;
                }
                else if (message.MessageType == APIDialogMessageType.FocusedControlId)
                {
                    FocusedDialogControlId = message.FocusedControlId;
                }
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
        IsMPDisplayConnected,
        IsMediaPortalConnected,
        IsTVServerConnected,
        SendListItem,
        IsFullscreenMedia,
    }
}
