using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MessageFramework.DataObjects;

namespace MessageFramework.Messages
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
        DialogInfoMessage
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
        public APIGuideAction GuideAction { get; set; }
        public APIMediaPortalAction MediaPortalAction { get; set; }
    }

    public enum APIActionMessageType
    {
        MediaPortalAction,
        MediaPortalWindow,
        WindowListAction,
        DialogListAction,
        GuideAction
    }

    public class APIGuideAction
    {
        public APIGuideActionType ActionType { get; set; }
        public int ChannelId { get; set; }
        public int ProgramId { get; set; }
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Cancel { get; set; }
    }

    public class APIListAction
    {
        public APIListActionType ActionType { get; set; }
        public APIListType ItemListType { get; set; }
        public int ItemIndex { get; set; }
        public string ItemText { get; set; }
        public APIListLayout ItemLayout { get; set; }

        public bool IsEqual(APIListAction action)
        {
            if (action == null)
            {
                return false;
            }

            return ActionType == action.ActionType
                   && ItemListType == action.ItemListType
                   && ItemText == action.ItemText
                   && ItemIndex == action.ItemIndex;
        }

    }

    public enum APIListActionType
    {
        SelectedItem,
        FocusedItem,
        Layout
    }

    public enum APIGuideActionType
    {
        UpdateData,
        UpdateRecordings,
        EPGAction
    }


    public class APIMediaPortalAction
    {
        public int ActionId { get; set; }
    }

   
}
