using System.Xml.Serialization;

namespace Common.Settings
{
    [XmlInclude(typeof(ConnectionSettings))] 
    public class GUISettings : SettingsBase
    {
        private ConnectionSettings _connectionSettings = new ConnectionSettings();
        private string _skinName = "default";
        private string _display = @"\\.\DISPLAY1";
        private bool _customResolution;
        private int _screenWidth = 1280;
        private int _screenHeight = 720;
        private int _screenOffSetX;
        private int _screenOffSetY;
        private bool _desktopMode;
        private string _priority = "BelowNormal";
        private bool _restartOnError = true;
        private string _cursorStyle = "Arrow";
        private bool _isSystemInfoEnabled;
        private int _userInteractionDelay = 10;
        private string _googleApiKey = "";

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

        public int UserInteractionDelay
        {
            get { return _userInteractionDelay; }
            set { _userInteractionDelay = value; NotifyPropertyChanged("UserInteractionDelay"); }
        }

        public string GoogleApiKey
        {
            get { return _googleApiKey;}
            set { _googleApiKey = value; NotifyPropertyChanged("GoogleApiKey"); }
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

        public bool IsSystemInfoEnabled
        {
            get { return _isSystemInfoEnabled; }
            set { _isSystemInfoEnabled = value; NotifyPropertyChanged("IsSystemInfoEnabled"); }
        }


        [XmlIgnore]
        public string SkinInfoXml { get; set; }


      
    }
}
