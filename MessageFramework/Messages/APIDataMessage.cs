using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MessageFramework.Messages
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

        [DataMember]
        public int IntValue { get; set; }

        [DataMember]
        public APISkinEditorData SkinEditorData { get; set; }
    }

 

    public enum APIDataMessageType
    {
        KeepAlive,
        EQData,
        MPActionId,
        SkinEditorInfo
    }




    public class APISkinEditorData
    {
        public APISkinEditorDataType DataType { get; set; }
        public string[] PropertyData { get; set; }
        public List<string[]> ListItemData { get; set; }
        public int IntValue { get; set; }
    }

    public enum APISkinEditorDataType
    {
        Property,
        ListItem,
        ActionId,
        WindowId,
        DialogId,
        FocusedControlId
    }

}
