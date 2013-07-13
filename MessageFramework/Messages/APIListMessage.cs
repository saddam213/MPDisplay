using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessageFramework.DataObjects
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
    }

    public enum APIListMessageType
    {
        Action,
        List
    }
}
