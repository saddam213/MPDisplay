using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinEditor.Views
{
    public class InfoEditorViewSettings : EditorViewModelSettings
    {
        public InfoEditorViewSettings()
            : base()
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
