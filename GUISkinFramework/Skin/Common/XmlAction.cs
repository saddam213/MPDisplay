using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using MPDisplay.Common.Controls.PropertyGrid.Attributes;
using GUISkinFramework.Styles;

namespace GUISkinFramework.Common
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

        public string DisplayName
        {
            get
            {
                return string.IsNullOrEmpty(Param1)
                    ? string.Format("Action({0})", ActionType)
                    : string.Format("{0}({1})", ActionType, Param1);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                PropertyChanged(this, new PropertyChangedEventArgs("DisplayName"));
            }
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
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class XmlActionTypeDetailsAttribute : Attribute
    {
        protected string _displayName;
        protected string _paramName;

        public XmlActionTypeDetailsAttribute(string displayName, string paramName)
        {
            this._displayName = displayName;
            this._paramName = paramName;
        }

        public string DisplayName
        {
            get { return this._displayName; }
        }

        public string ParamName
        {
            get { return this._paramName; }
        }
    }
}
