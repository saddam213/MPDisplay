using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIDataMessage
    {
        [DataMember]
        public APIDataMessageType DataType { get; set; }

        [DataMember]
        public int[] IntArray { get; set; }

        [DataMember]
        public byte[] ByteArray { get; set; }
    }

    public enum APIDataMessageType
    {
        EQData,
    }

}
