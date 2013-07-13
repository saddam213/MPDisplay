using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPDisplay.Common.Settings
{
    [XmlInclude(typeof(ConnectionSettings))] 
    public class GUISettings : SettingsBase
    {
        private ConnectionSettings _connectionSettings = new ConnectionSettings();
        private string _skinName = "default";
        private string _display = @"\\.\DISPLAY1";
        private bool _customResolution = false;
        private int _screenWidth = 1280;
        private int _screenHeight = 720;
        private int _screenOffSetX = 0;
        private int _screenOffSetY = 0;
        private bool _desktopMode = false;
        private string _priority = "BelowNormal";
        private bool _restartOnError = true;
        private string _cursorStyle = "Arrow";
        private int _resumeConnectDelay = 5000;

        public string SkinName
        {
            get { return _skinName; }
            set { _skinName = value; NotifyPropertyChanged("SkinName"); }
        }

        public string Display
        {
            get { return _display; }
            set { _display = value; NotifyPropertyChanged("Display"); }
        }

        public bool CustomResolution
        {
            get { return _customResolution; }
            set { _customResolution = value; NotifyPropertyChanged("CustomResolution"); }
        }

        public int ScreenWidth
        {
            get { return _screenWidth; }
            set { _screenWidth = value; NotifyPropertyChanged("ScreenWidth"); }
        }

        public int ScreenHeight
        {
            get { return _screenHeight; }
            set { _screenHeight = value; NotifyPropertyChanged("ScreenHeight"); }
        }

        public int ScreenOffSetX
        {
            get { return _screenOffSetX; }
            set { _screenOffSetX = value; NotifyPropertyChanged("ScreenOffSetX"); }
        }

        public int ScreenOffSetY
        {
            get { return _screenOffSetY; }
            set { _screenOffSetY = value; NotifyPropertyChanged("ScreenOffSetY"); }
        }

        public bool DesktopMode
        {
            get { return _desktopMode; }
            set { _desktopMode = value; NotifyPropertyChanged("DesktopMode"); }
        }

        public string CursorStyle
        {
            get { return _cursorStyle; }
            set { _cursorStyle = value; NotifyPropertyChanged("CursorStyle"); }
        }

        public string Priority
        {
            get { return _priority; }
            set { _priority = value; NotifyPropertyChanged("Priority"); }
        }


        public ConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { _connectionSettings = value; NotifyPropertyChanged("ConnectionSettings"); }
        }

        public bool RestartOnError
        {
            get { return _restartOnError; }
            set { _restartOnError = value; NotifyPropertyChanged("RestartOnError"); }
        }

        public int ResumeConnectDelay
        {
            get { return _resumeConnectDelay; }
            set { _resumeConnectDelay = value; NotifyPropertyChanged("ResumeConnectDelay"); }
        }

        [XmlIgnore]
        public string SkinInfoXml { get; set; }

      
    }
}
