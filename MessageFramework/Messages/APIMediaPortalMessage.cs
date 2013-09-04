using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MessageFramework.DataObjects
{
    [DataContract]
    public class APIMediaPortalMessage
    {
        [DataMember]
        public APIMediaPortalMessageType MessageType { get; set; }

        [DataMember]
        public APIWindowInfoMessage WindowMessage { get; set; }

        [DataMember]
        public APIActionMessage ActionMessage { get; set; }
    }

    public enum APIMediaPortalMessageType
    {
        ActionMessage,
        WindowInfoMessage,
        DialogInfoMessage,
    }

    public class APIWindowInfoMessage
    {
        public List<APIPropertyMessage> Properties { get; set; }
        public List<APIListType> Lists { get; set; }
        public int EQData { get; set; }
    }

    public class APIActionMessage
    {
        public APIActionMessageType ActionType { get; set; }
        public APIListAction ListAction { get; set; }
        public APIMediaPortalAction MediaPortalAction { get; set; }
    }

    public enum APIActionMessageType
    {
        MediaPortalAction,
        ListAction,
        MediaPortalWindow
    }


    public class APIListAction
    {
        public APIListActionType ActionType { get; set; }
        public APIListType ItemListType { get; set; }
        public int ItemIndex { get; set; }
        public string ItemText { get; set; }
        public APIListLayout ItemLayout { get; set; }

        public override bool Equals(object obj)
        {
             var lObj = obj as APIListAction;
            if (lObj == null || this == null)
            {
                return this == null && lObj == null;
            }



            if (ActionType == lObj.ActionType
                && ItemListType == lObj.ItemListType
                && ItemText == lObj.ItemText
                && ItemIndex == lObj.ItemIndex)
            {
                return true;
            }

            return base.Equals(obj);
        }
    }

    public enum APIListActionType
    {
        SelectedItem,
        FocusedItem,
        Layout
    }


    public class APIMediaPortalAction
    {
        public int ActionId { get; set; }
    }

   
}
