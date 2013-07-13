using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIConnection
    {
        public APIConnection(string connectionName)
        {
            this.ConnectionName = connectionName;
        }
        [DataMember]
        public string ConnectionName { get; set; }
    }
}
