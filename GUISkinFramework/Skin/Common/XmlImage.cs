﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISkinFramework.Common
{
    public class XmlImageFile : INotifyPropertyChanged
    {
        private string _xmlName = string.Empty;
        private string _fileName = string.Empty;
        private string _subFolder = string.Empty;

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; NotifyPropertyChanged("DisplayName");  }
        }
        
        public string XmlName
        {
            get { return _xmlName; }
            set { _xmlName = value; NotifyPropertyChanged("XmlName"); }
        }

        public string SubFolder
        {
            get { return _subFolder; }
            set { _subFolder = value; NotifyPropertyChanged("SubFolder"); }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; NotifyPropertyChanged("FileName"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
