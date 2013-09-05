﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GUISkinFramework.Common
{
    [Serializable]
    [XmlType(TypeName = "SkinOption")]
    public class XmlSkinOption : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isEnabled;
        private string _description = string.Empty;
        private string _previewImage = string.Empty;
        private bool _isPreviewImageEnabled;

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        [DefaultValue(false)]
        [XmlAttribute(AttributeName = "IsEnabled")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; NotifyPropertyChanged("IsEnabled"); }
        }

        [DefaultValue(false)]
        [XmlAttribute(AttributeName = "IsPreviewImageEnabled")]
        public bool IsPreviewImageEnabled
        {
            get { return _isPreviewImageEnabled; }
            set { _isPreviewImageEnabled = value; NotifyPropertyChanged("IsPreviewImageEnabled"); }
        }

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "PreviewImage")]
        public string PreviewImage
        {
            get { return _isPreviewImageEnabled ? _previewImage : string.Empty; }
            set { _previewImage = _isPreviewImageEnabled ? value : string.Empty; NotifyPropertyChanged("PreviewImage"); }
        }

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
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