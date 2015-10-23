using SkinEditor.Themes;

namespace SkinEditor.Views
{
    public class InfoEditorViewSettings : EditorViewModelSettings
    {
        public InfoEditorViewSettings()
        {
            IpAddress = "localhost";
            Port = 44444;
        }


        private string _ipAddress = "localhost";

        private int _port = 44444;

        public string IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; NotifyPropertyChanged("IpAddress"); }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }
    }

 
}
