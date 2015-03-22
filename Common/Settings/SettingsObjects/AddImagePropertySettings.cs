using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Common.Settings
{
    public class AddImagePropertySettings : SettingsBase
    {
        private string _imageName;
        private string _mpdSkinTag;
        private string _propertyString;
        private string _path;

        private string _fullPath;
        private bool _pathExists;
        private List<String> _extensions = new List<string> { ".png", ".jpg", ".bmp", ".tif" };
        private List<String> _mpProperties;

        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; NotifyPropertyChanged("ImageName"); }
        }

        public string MPDSkinTag
        {
            get { return _mpdSkinTag; }
            set { _mpdSkinTag = value; NotifyPropertyChanged("MPDSkinTag"); }
        }

        public string PropertyString
        {
            get { return _propertyString; }
            set { _propertyString = value; NotifyPropertyChanged("PropertyString"); }
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; NotifyPropertyChanged("Path"); }
        }

        [XmlIgnore]
        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        [XmlIgnore]
        public bool PathExists
        {
            get { return _pathExists; }
            set
            {
                _pathExists = false;
                if (!String.IsNullOrEmpty(_fullPath))
                {
                    _pathExists = Directory.Exists(_fullPath);
                }
            }
        }

        [XmlIgnore]
        public List<string> Extensions
        {
            get { return _extensions; }
        }

        [XmlIgnore]
        public List<string> MPProperties
        {
            get { return _mpProperties; }
            set
            {
                _mpProperties = new List<string>();

                if (!string.IsNullOrEmpty(_propertyString))
                {
                    var parts = _propertyString.Contains("+") ? _propertyString.Split('+').ToList() : new List<string> { _propertyString };
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("#"))
                        {
                               _mpProperties.Add(part);
                        }
                    }
                }
            }
        }
    }
}
