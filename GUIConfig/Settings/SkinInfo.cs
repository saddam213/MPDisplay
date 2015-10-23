using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GUIConfig.Settings
{
    /// <summary>
    /// Slimline version class for XmlSkinInfo
    /// </summary>
    [Serializable]
    [XmlType("XmlSkinInfo")]
    public class SkinInfo : INotifyPropertyChanged
    {
        #region Fields

        private string _author = "Team MPDisplay++";
        private string _skinName;
        private string _skinFolderPath;
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the skin.
        /// </summary>
        /// <value>
        /// The width of the skin.
        /// </value>
        public int SkinWidth
        {
            get { return _skinWidth; }
            set { _skinWidth = value; NotifyPropertyChanged("SkinWidth"); }
        }

        /// <summary>
        /// Gets or sets the height of the skin.
        /// </summary>
        /// <value>
        /// The height of the skin.
        /// </value>
        public int SkinHeight
        {
            get { return _skinHeight; }
            set { _skinHeight = value; NotifyPropertyChanged("SkinHeight"); }
        }

        /// <summary>
        /// Gets or sets the current style.
        /// </summary>
        /// <value>
        /// The current style.
        /// </value>
        public string CurrentStyle
        {
            get { return _currentStyle; }
            set { _currentStyle = value; NotifyPropertyChanged("CurrentStyle"); NotifyPropertyChanged("PreviewImage"); }
        }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        /// <value>
        /// The current language.
        /// </value>
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

        /// <summary>
        /// Gets or sets the skin options.
        /// </summary>
        public ObservableCollection<SkinOption> SkinOptions
        {
            get { return _skinOptions; }
            set { _skinOptions = value; NotifyPropertyChanged("SkinOptions"); }
        }

        /// <summary>
        /// Gets or sets the name of the skin.
        /// </summary>
        public string SkinName
        {
            get { return _skinName; }
            set { _skinName = value; NotifyPropertyChanged("SkinName"); }
        }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public string Author
        {
            get { return _author; }
            set { _author = value; NotifyPropertyChanged("Author"); }
        }

        /// <summary>
        /// Gets or sets the skin folder path.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SkinFolderPath
        {
            get { return _skinFolderPath; }
            set { _skinFolderPath = value; NotifyPropertyChanged("SkinFolderPath"); }
        }

        /// <summary>
        /// Gets the skin info path.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SkinInfoPath
        {
            get { return SkinFolderPath + "\\SkinInfo.xml"; }
        }

        /// <summary>
        /// Gets the skin language path.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SkinLanguagePath
        {
            get { return SkinFolderPath + "\\Language.xml"; }
        }

        /// <summary>
        /// Gets the skin image folder.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SkinImageFolder
        {
            get { return SkinFolderPath + "\\Images"; }
        }

        /// <summary>
        /// Gets the skin style folder.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string SkinStyleFolder
        {
            get { return SkinFolderPath + "\\Styles\\"; }
        }

        /// <summary>
        /// Gets the preview image.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public string PreviewImage
        {
            get
            { 
                var filename = string.Format("{0}\\Preview_{1}.png",SkinImageFolder, CurrentStyle);
                if (File.Exists(filename))
                {
                    return filename;
                }
                return SkinImageFolder + "\\Preview.png";
            }
        }

        /// <summary>
        /// Gets the styles.
        /// </summary>
        public IEnumerable<string> Styles
        {
            get
            {
                return Directory.Exists(SkinStyleFolder) ? Directory.GetFiles(SkinStyleFolder, "*.xml").Select(Path.GetFileNameWithoutExtension) : null;
            }
        }

        /// <summary>
        /// Gets the languages.
        /// </summary>
        public IEnumerable<string> Languages
        {
            get
            {
                if (!File.Exists(SkinLanguagePath)) return null;
                try
                {
                    return XElement.Load(SkinLanguagePath).Descendants("LanguageValue").Select(x => x.FirstAttribute.Value).Distinct();
                }
                catch
                {
                    // ignored
                }
                return null;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

    /// <summary>
    /// Slimline version class for XmlSkinOption
    /// </summary>
    [Serializable]
    [XmlType(TypeName = "SkinOption")]
    public class SkinOption : INotifyPropertyChanged
    {
        #region Fields

        private string _name = string.Empty;
        private bool _isEnabled;
        private string _description = string.Empty;
        private string _previewImage = string.Empty;
        private bool _isPreviewImageEnabled;
        private string _group;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }
     
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute(AttributeName = "Group")]
        public string Group
        {
            get { return _group; }
            set { _group = value; NotifyPropertyChanged("Group"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is enabled]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [XmlAttribute(AttributeName = "IsEnabled")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; NotifyPropertyChanged("IsEnabled"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is preview image enabled].
        /// </summary>
        /// <value>
        /// <c>true</c> if [is preview image enabled]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        [XmlAttribute(AttributeName = "IsPreviewImageEnabled")]
        public bool IsPreviewImageEnabled
        {
            get { return _isPreviewImageEnabled; }
            set { _isPreviewImageEnabled = value; NotifyPropertyChanged("IsPreviewImageEnabled"); }
        }

        /// <summary>
        /// Gets or sets the preview image.
        /// </summary>
        [DefaultValue("")]
        [XmlAttribute(AttributeName = "PreviewImage")]
        public string PreviewImage
        {
            get { return _previewImage; }
            set { _previewImage = value; NotifyPropertyChanged("PreviewImage"); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [XmlAttribute(AttributeName = "Description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; NotifyPropertyChanged("Description"); }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }

}
