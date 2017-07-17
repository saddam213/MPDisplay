using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Common.Settings
{
    public class AddImagePropertySettings : SettingsBase
    {
        private string _imageName;
        private string _mpdSkinTag;
        private string _propertyString;
        private string _path;

        private bool _pathExists;
        private List<string> _mpProperties;

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
        public string FullPath { get; set; }

        [XmlIgnore]
        public bool PathExists
        {
            get { return _pathExists; }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                _pathExists = false;
                if (!String.IsNullOrEmpty(FullPath))
                {
                    _pathExists = Directory.Exists(FullPath);
                }
            }
        }

        [XmlIgnore]
        public List<string> Extensions { get; } = new List<string> { ".png", ".jpg", ".bmp", ".tif" };

        [XmlIgnore]
        public List<string> MPProperties
        {
            get { return _mpProperties; }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                _mpProperties = new List<string>();

                if (string.IsNullOrEmpty(_propertyString)) return;
                var parts = _propertyString.Contains("+") ? _propertyString.Split('+').ToList() : new List<string> { _propertyString };
                foreach (var part in parts.Where(part => part.StartsWith("#")))
                {
                    _mpProperties.Add(part);
                }
            }
        }
    }
}
