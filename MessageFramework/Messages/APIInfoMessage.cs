using System.Collections.Generic;
using System.Runtime.Serialization;
using MessageFramework.DataObjects;

namespace MessageFramework.Messages
{
    [DataContract]
    public class APIInfoMessage
    {
        [DataMember]
        public APIInfoMessageType MessageType { get; set; }

        [DataMember]
        public APIWindowMessage WindowMessage { get; set; }

        [DataMember]
        public APIDialogMessage DialogMessage { get; set; }

        [DataMember]
        public APIPlayerMessage PlayerMessage { get; set; }

        [DataMember]
        public APIVisibleMessage VisibleMessage { get; set; }
    }

    public enum APIInfoMessageType
    {
        WindowMessage,
        DialogMessage,
        PlayerMessage,
        VisibleMessage
    }


    public class APIWindowMessage
    {
        public APIWindowMessageType MessageType { get; set; }
        public int WindowId { get; set; }
        public int FocusedControlId { get; set; }
        public List<string> EnabledPlugins { get; set; }
        public int ChannelId { get; set; }
        public int ProgramId { get; set; }
    }

    public enum APIWindowMessageType
    {
        WindowId,
        FocusedControlId,
    }

    public class APIPlayerMessage
    {
        public APIPlaybackType PlayerPluginType { get; set; }
        public APIPlaybackType PlaybackType { get; set; }
        public APIPlaybackState PlaybackState { get; set; }
        public bool PlayerFullScreen { get; set; }

        public bool IsEquals(APIPlayerMessage msg)
        {
            if (msg == null)
            {
                return false;
            }

            if (PlayerPluginType == msg.PlayerPluginType && PlaybackType == msg.PlaybackType 
                && PlaybackState == msg.PlaybackState && PlayerFullScreen == msg.PlayerFullScreen)
            {
                return true;
            }
            return false;

        }
    }

    public class APIDialogMessage
    {
        public APIDialogMessageType MessageType { get; set; }
        public int DialogId { get; set; }
        public int FocusedControlId { get; set; }
    }

    public enum APIDialogMessageType
    {
        DialogId,
        FocusedControlId
    }

    public class APIVisibleMessage
    {
        public List<string> EnabledPlugins { get; set; }
        public bool IsTvRecording { get; set; }
    }

    public enum APIVisibleType
    {
        PluginList,
        IsTvRecording
    }
  
}
