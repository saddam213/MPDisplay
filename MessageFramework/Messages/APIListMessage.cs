using System.Runtime.Serialization;
using MessageFramework.DataObjects;

namespace MessageFramework.Messages
{
    [DataContract]
    public class APIListMessage
    {
        [DataMember]
        public APIListMessageType MessageType { get; set; }

        [DataMember]
        public APIList List { get; set; }

        [DataMember]
        public APIListAction Action { get; set; }

        [DataMember]
        public APITVGuide TvGuide { get; set; }
    }

    public enum APIListMessageType
    {
        Action,
        List,
        TVGuide
    }
}
