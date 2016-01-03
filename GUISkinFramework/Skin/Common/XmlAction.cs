using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace GUISkinFramework.Skin
{
    [Serializable]
    [XmlType(TypeName = "Action")]
    public class XmlAction : INotifyPropertyChanged
    {
        private XmlActionType _actionType;
        private string _param1;
        private string _param2;

        public XmlAction()
        {
            this.SetDefaultValues();
        }

        [DefaultValue(XmlActionType.Empty)]
        [XmlAttribute(AttributeName="Action")]
        public XmlActionType ActionType
        {
            get { return _actionType; }
            set { _actionType = value; NotifyPropertyChanged("ActionType"); }
        }

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Param")]
        public string Param1
        {
            get { return _param1; }
            set { _param1 = value; NotifyPropertyChanged("Param1"); }
        }

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Param2")]
        public string Param2
        {
            get { return _param2; }
            set { _param2 = value; NotifyPropertyChanged("Param2"); }
        }

        public string DisplayName => string.IsNullOrEmpty(Param1)
            ? $"Action({ActionType})"
            : $"{ActionType}({Param1})";

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(property));
            PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
        }
    }

    public enum XmlActionType
    {
        [XmlActionTypeDetails("Empty", "")]
        Empty = 0,

        [XmlActionTypeDetails("Exit", "Exits MPDisplay")]
        Exit,

        [XmlActionTypeDetails("OpenWindow", "Id of the MPDisplay Window to Open")]
        OpenWindow,

        [XmlActionTypeDetails("OpenDialog", "Id of the MPDisplay Dialog to Open/Close")]
        OpenDialog,

        [XmlActionTypeDetails("PreviousWindow", "Returns to the previous window")]
        PreviousWindow,

        [XmlActionTypeDetails("LockWindow", "Locks the current MPDisplay window")]
        LockWindow,

        [XmlActionTypeDetails("CloseWindow", "Closes the current window")]
        CloseWindow,

        [XmlActionTypeDetails("NowPlaying", "Switches to the now playing screen (if playing)")]
        NowPlaying,

        [XmlActionTypeDetails("ControlVisible", "Id of the Control to make Visible/Invisible")]
        ControlVisible,

        [XmlActionTypeDetails("ChangeListView", "Change the current ListControls item layout")]
        ChangeListView,

        [XmlActionTypeDetails("RunProgram", "Path of program to run")]
        RunProgram,

        [XmlActionTypeDetails("KillProgram", "Path of program to kill")]
        KillProgram,

        [XmlActionTypeDetails("MediaPortalWindow", "Id of the MediaPortal window to open")]
        MediaPortalWindow,

        [XmlActionTypeDetails("MediaPortalAction", "Action to send to MediaPortal")]
        MediaPortalAction,

        [XmlActionTypeDetails("SwitchTheme", "Changes the current skins theme")]
        SwitchTheme,

        [XmlActionTypeDetails("Connect", "Connects to the MPDisplay Server if connection is lost.")]
        Connect,

        [XmlActionTypeDetails("ScheduleEPGAction", "Creates or cancels the currently selected EPG schedule.")]
        ScheduleEPGAction
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class XmlActionTypeDetailsAttribute : Attribute
    {
        protected string DdisplayName;
        protected string PparamName;

        public XmlActionTypeDetailsAttribute(string displayName, string paramName)
        {
            DdisplayName = displayName;
            PparamName = paramName;
        }

        public string DisplayName => DdisplayName;

        public string ParamName => PparamName;
    }
}
