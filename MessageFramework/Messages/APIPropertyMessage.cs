using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIPropertyMessage
    {
        [DataMember]
        public string SkinTag { get; set; }

        [DataMember]
        public List<string> Tags { get; set; }

        [DataMember]
        public APIPropertyType PropertyType { get; set; }

        [DataMember]
        public APIImage Image { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string DefaultValue { get; set; }
    }

    public enum APIPropertyType
    {
        Label,
        Image,
        Number
    }
}
