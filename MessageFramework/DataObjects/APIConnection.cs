using System;
using System.Runtime.Serialization;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIConnection
    {
        public APIConnection(ConnectionType type)
        {
            ConnectionName = type.ToString("g") +"_" + Guid.NewGuid().ToString("n").Substring(0, 10);
            ConnectionType = type;
        }
        [DataMember]
        public string ConnectionName { get; set; }

        [DataMember]
        public ConnectionType ConnectionType { get; set; }
    }
    
    public enum ConnectionType
    {
        Unknown,
        MediaPortalPlugin,
        MPDisplay,
        SkinEditor
    }
} 

