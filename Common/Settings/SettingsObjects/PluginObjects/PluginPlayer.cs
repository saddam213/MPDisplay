using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Common.Settings.SettingsObjects
{
    public class PlayerPlugin
    {
        private string _pluginName;
        private ObservableCollection<int> _windowIds = new ObservableCollection<int>();

        public string PluginName
        {
            get { return _pluginName; }
            set { _pluginName = value; }
        }
     
        public ObservableCollection<int> WindowIds
        {
            get { return _windowIds; }
            set { _windowIds = value; }
        }
    }
}
