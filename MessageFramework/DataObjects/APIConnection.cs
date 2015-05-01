using System.Runtime.Serialization;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIConnection
    {
        public APIConnection(string connectionName)
        {
            ConnectionName = connectionName;
        }
        [DataMember]
        public string ConnectionName { get; set; }
    }
}
