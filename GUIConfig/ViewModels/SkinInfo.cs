using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GUIConfig.ViewModels
{
    [Serializable]
    [XmlType("XmlSkinInfo")]
    public class SkinInfo : INotifyPropertyChanged
    {
        private int _skinWidth = 1280;
        private int _skinHeight = 720;
        private string _currentStyle = "Default";
        private string _currentLanguage = "English";
        private string _textVerticalScrollSeperator = "---------------------------------";
        private bool _textVerticalScrollWrap = true;
        private int _textVerticalScrollSpeed = 20;
        private int _textVerticalScrollDelay = 6;
        private string _textHorizontalScrollSeperator = " | ";
        private bool _textHorizontalScrollWrap = true;
        private int _textHorizontalScrollSpeed = 20;
        private int _textHorizontalScrollDelay = 3;
        private bool _textEnableScrolling = true;
        private ObservableCollection<SkinOption> _skinOptions = new ObservableCollection<SkinOption>();

       

        public int SkinWidth
        {
            get { return _skinWidth; }
            set { _skinWidth = value; NotifyPropertyChanged("SkinWidth"); }
        }
      
        public int SkinHeight
        {
            get { return _skinHeight; }
            set { _skinHeight = value; NotifyPropertyChanged("SkinHeight"); }
        }

        public string CurrentStyle
        {
            get { return _currentStyle; }
            set { _currentStyle = value; NotifyPropertyChanged("CurrentStyle"); NotifyPropertyChanged("PreviewImage"); }
        }

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; NotifyPropertyChanged("CurrentLanguage");  }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text scrolling is enabled.
        /// </summary>
        public bool TextEnableScrolling
        {
            get { return _textEnableScrolling; }
            set { _textEnableScrolling = value; NotifyPropertyChanged("TextEnableScrolling"); }
        }



        /// <summary>
        /// Gets or sets the horizontal scroll text delay.
        /// </summary>
        public int TextHorizontalScrollDelay
        {
            get { return _textHorizontalScrollDelay; }
            set { _textHorizontalScrollDelay = value; NotifyPropertyChanged("TextHorizontalScrollDelay"); }
        }

        /// <summary>
        /// Gets or sets the horizontal scrolling text speed.
        /// </summary>
        public int TextHorizontalScrollSpeed
        {
            get { return _textHorizontalScrollSpeed; }
            set { _textHorizontalScrollSpeed = value; NotifyPropertyChanged("TextHorizontalScrollSpeed"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether horizontal scrolling text wraps.
        /// </summary>
        public bool TextHorizontalScrollWrap
        {
            get { return _textHorizontalScrollWrap; }
            set { _textHorizontalScrollWrap = value; NotifyPropertyChanged("TextHorizontalScrollWrap"); }
        }

        /// <summary>
        /// Gets or sets the horizontal scrolling text seperator.
        /// </summary>
        public string TextHorizontalScrollSeperator
        {
            get { return _textHorizontalScrollSeperator; }
            set { _textHorizontalScrollSeperator = value; NotifyPropertyChanged("TextHorizontalScrollSeperator"); }
        }

        /// <summary>
        /// Gets or sets the vertical scrolling text delay.
        /// </summary>
        public int TextVerticalScrollDelay
        {
            get { return _textVerticalScrollDelay; }
            set { _textVerticalScrollDelay = value; NotifyPropertyChanged("TextVerticalScrollDelay"); }
        }

        /// <summary>
        /// Gets or sets the vertical scrolling text speed.
        /// </summary>
        public int TextVerticalScrollSpeed
        {
            get { return _textVerticalScrollSpeed; }
            set { _textVerticalScrollSpeed = value; NotifyPropertyChanged("TextVerticalScrollSpeed"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vertical scrolling text wraps.
        /// </summary>
        public bool TextVerticalScrollWrap
        {
            get { return _textVerticalScrollWrap; }
            set { _textVerticalScrollWrap = value; NotifyPropertyChanged("TextVerticalScrollWrap"); }
        }

        /// <summary>
        /// Gets or sets the text vertical scroll seperator.
        /// </summary>
        public string TextVerticalScrollSeperator
        {
            get { return _textVerticalScrollSeperator; }
            set { _textVerticalScrollSeperator = value; NotifyPropertyChanged("TextVerticalScrollSeperator"); }
        }



        public ObservableCollection<SkinOption> SkinOptions
        {
            get { return _skinOptions; }
            set { _skinOptions = value; NotifyPropertyChanged("SkinOptions"); }
        }

        private string _skinName;

     
        public string SkinName
        {
            get { return _skinName; }
            set { _skinName = value; NotifyPropertyChanged("SkinName"); }
        }

        private string _author = "Team MPDisplay++";

        public string Author
        {
            get { return _author; }
            set { _author = value; NotifyPropertyChanged("Author"); }
        }


















        private string _skinFolderPath;
        [XmlIgnore]
        [Browsable(false)]
        public string SkinFolderPath
        {
            get { return _skinFolderPath; }
            set { _skinFolderPath = value; NotifyPropertyChanged("SkinFolderPath"); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinInfoPath
        {
            get { return SkinFolderPath + "\\SkinInfo.xml"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinLanguagePath
        {
            get { return SkinFolderPath + "\\Language.xml"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinImageFolder
        {
            get { return SkinFolderPath + "\\Images"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinStyleFolder
        {
            get { return SkinFolderPath + "\\Styles\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string PreviewImage
        {
            get
            { 
                string filename = string.Format("{0}\\Preview_{1}.png",SkinImageFolder, CurrentStyle);
                if (System.IO.File.Exists(filename))
                {
                    return filename;
                }
                return SkinImageFolder + "\\Preview.png";
            }
        }

        public IEnumerable<string> Styles
        {
            get
            {
                if (Directory.Exists(SkinStyleFolder))
                {
                    return Directory.GetFiles(SkinStyleFolder, "*.xml").Select(x => System.IO.Path.GetFileNameWithoutExtension(x));
                }
                return null;
            }
        }

        public IEnumerable<string> Languages
        {
            get
            {
                if (File.Exists(SkinLanguagePath))
                {
                    try
                    {
                        return XElement.Load(SkinLanguagePath).Descendants("LanguageValue").Select(x => x.FirstAttribute.Value).Distinct();
                    }
                    catch  { }
                }
                return null;
            }
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

    [Serializable]
    [XmlType(TypeName = "SkinOption")]
    public class SkinOption : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private bool _isEnabled;
        private string _description = null;
        private string _previewImage = string.Empty;
        private bool _isPreviewImageEnabled;

        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }

        private string _group;
        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Group")]
        public string Group
        {
            get { return _group; }
            set { _group = value; NotifyPropertyChanged("Group"); }
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
            get { return _previewImage; }
            set { _previewImage = value; NotifyPropertyChanged("PreviewImage"); }
        }

   
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
