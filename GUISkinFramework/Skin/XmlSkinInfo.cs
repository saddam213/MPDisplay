using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Common.Helpers;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Language;
using GUISkinFramework.Property;
using GUISkinFramework.Skin;
using GUISkinFramework.Styles;
using GUISkinFramework.Windows;
using MPDisplay.Common.Log;

namespace GUISkinFramework
{
    [Serializable]
    public class XmlSkinInfo : INotifyPropertyChanged
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
        private ObservableCollection<XmlSkinOption> _skinOptions = new ObservableCollection<XmlSkinOption>();
      
        [Browsable(false)]
        public int SkinWidth
        {
            get { return _skinWidth; }
            set { _skinWidth = value; NotifyPropertyChanged("SkinWidth"); }
        }

         [Browsable(false)]
        public int SkinHeight
        {
            get { return _skinHeight; }
            set { _skinHeight = value; NotifyPropertyChanged("SkinHeight"); }
        }

         [Browsable(false)]
        public string CurrentStyle
        {
            get { return _currentStyle; }
            set { _currentStyle = value; NotifyPropertyChanged("CurrentStyle"); }
        }

         [Browsable(false)]
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set 
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    NotifyPropertyChanged("CurrentLanguage");
                }
            }
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
            set { _textVerticalScrollSeperator = value; NotifyPropertyChanged("TextVerticalScrollSeperator");  }
        }

      

        public ObservableCollection<XmlSkinOption> SkinOptions
        {
            get { return _skinOptions; }
            set { _skinOptions = value; NotifyPropertyChanged("SkinOptions"); }
        }

        private string _skinName;

        [Browsable(false)]
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
            set { _skinFolderPath = value; NotifyPropertyChanged("SkinFolderPath");  }
        }

          [XmlIgnore]
          [Browsable(false)]
          public string SkinInfoPath
          {
              get { return SkinFolderPath + "\\SkinInfo.xml"; }
          }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlFolder 
        {
            get { return SkinFolderPath + "\\SkinFiles\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlWindowFolder
        {
            get { return SkinXmlFolder + "Windows\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlDialogFolder
        {
            get { return SkinXmlFolder + "Dialogs\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinImageFolder
        {
            get { return SkinFolderPath + "\\Images\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SkinStyleFolder
        {
            get { return SkinFolderPath + "\\Styles\\"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string SplashScreenImage
        {
            get { return SkinImageFolder + "SplashScreen.png"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string ErrorScreenImage
        {
            get { return SkinImageFolder + "ErrorScreen.png"; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string PreviewImage
        {
            get { return SkinImageFolder + "Preview.png"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }




























        private Log Log = LoggingManager.GetLog(typeof(XmlSkinInfo));
        private ObservableCollection<XmlImageFile> _images = new ObservableCollection<XmlImageFile>();

        private Dictionary<string, XmlStyleCollection> _styles = new Dictionary<string, XmlStyleCollection>();
        private XmlStyleCollection _style = new XmlStyleCollection();
        private ObservableCollection<XmlWindow> _windows = new ObservableCollection<XmlWindow>();
        private ObservableCollection<XmlDialog> _dialogs = new ObservableCollection<XmlDialog>();

        private XmlLanguage _language = new XmlLanguage();

        private XmlPropertyInfo _propertyInfo = new XmlPropertyInfo();

        public void LoadXmlSkin()
        {
            Log.Message(LogLevel.Info, "Loading skin: {0} ....", _skinName);
            if (Directory.Exists(SkinFolderPath))
            {
                LoadImages();
                LoadLanguage();
                LoadProperties();
                LoadStyles();
                LoadWindows();
            }
            Log.Message(LogLevel.Info, "Loading skin: {0} complete.", _skinName);
        }

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlImageFile> Images
        {
            get { return _images; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlProperty> Properties
        {
            get { return _propertyInfo.AllProperties; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public XmlLanguage Language
        {
            get { return _language; }
        }





        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlWindow> Windows
        {
            get { return _windows; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlDialog> Dialogs
        {
            get { return _dialogs; }
        }

        private void LoadWindows()
        {
            // Load Windows
            _windows.Clear();
            _dialogs.Clear();
            if (Directory.Exists(SkinXmlFolder))
            {
                foreach (var windowXml in DirectoryHelpers.GetFiles(SkinXmlWindowFolder, "*.xml"))
                {

                    var window = SerializationHelper.Deserialize<XmlWindow>(windowXml);
                    if (window != null)
                    {
                        window.ApplyStyle(_style);
                        window.Width = SkinWidth;
                        window.Height = SkinHeight;
                        foreach (var control in window.Controls.GetControls())
                        {
                            control.WindowId = window.Id;
                            control.ApplyStyle(_style);
                        }
                        _windows.Add(window);
                    }
                }

                foreach (var dialogXml in DirectoryHelpers.GetFiles(SkinXmlDialogFolder, "*.xml"))
                {
                    var dialog = SerializationHelper.Deserialize<XmlDialog>(dialogXml);
                    if (dialog != null)
                    {
                        dialog.ApplyStyle(_style);
                        foreach (var control in dialog.Controls.GetControls())
                        {
                            control.WindowId = dialog.Id;
                            control.ApplyStyle(_style);
                        }
                        _dialogs.Add(dialog);
                    }
                }
            }
        }



        public void AddWindow(XmlWindow window)
        {
            if (window != null)
            {
                foreach (var control in window.Controls.GetControls())
                {
                    control.WindowId = window.Id;
                }
                Windows.Add(window);
            }
        }

        public void AddDialog(XmlDialog xmlDialog)
        {
            if (xmlDialog != null)
            {
                foreach (var control in xmlDialog.Controls.GetControls())
                {
                    control.WindowId = xmlDialog.Id;
                }
                Dialogs.Add(xmlDialog);
            }
        }


        public void CreateSkin()
        {
            DirectoryHelpers.CreateIfNotExists(SkinFolderPath);
            DirectoryHelpers.CreateIfNotExists(SkinImageFolder);
            DirectoryHelpers.CreateIfNotExists(SkinStyleFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlWindowFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlDialogFolder);
            SerializationHelper.Serialize<XmlSkinInfo>(this, Path.Combine(SkinFolderPath, "SkinInfo.xml"));
            SerializationHelper.Serialize<XmlPropertyInfo>(new XmlPropertyInfo(), Path.Combine(SkinFolderPath, "PropertyInfo.xml"));
            SerializationHelper.Serialize<XmlLanguage>(new XmlLanguage(), Path.Combine(SkinFolderPath, "Language.xml"));
            SerializationHelper.Serialize<XmlStyleCollection>(new XmlStyleCollection(), Path.Combine(SkinStyleFolder, "Default.xml"));
        }


        public void SaveSkin()
        {
            DirectoryHelpers.CreateIfNotExists(SkinXmlFolder);
            SerializationHelper.Serialize<XmlSkinInfo>(this, Path.Combine(SkinFolderPath, "SkinInfo.xml"));
            SaveWindowXmls(Windows, SkinXmlWindowFolder);
            SaveDialogXmls(Dialogs, SkinXmlDialogFolder);
            SaveStyles();
            SaveLanguage();
            SaveProperties();
        }

        public void SaveSkinAs(string skinName, string directory)
        {
            var newSkinInfo = new XmlSkinInfo
            {
                SkinName = skinName,
                SkinFolderPath = Path.Combine(directory,skinName),
                SkinWidth = this.SkinWidth,
                SkinHeight = this.SkinHeight
            };
            newSkinInfo.CreateSkin();
            newSkinInfo._windows = _windows;
            newSkinInfo._dialogs = _dialogs;
            newSkinInfo._propertyInfo = _propertyInfo;
            newSkinInfo._language = _language;
            newSkinInfo._styles = _styles;
            SaveImages(newSkinInfo);
            newSkinInfo.SaveSkin();
        }


        private void SaveWindowXmls(IEnumerable<XmlWindow> windows, string directory)
        {
            DirectoryHelpers.CreateIfNotExists(directory);
            var savedFiles = new List<string>();
            foreach (var window in windows)
            {
                string windowFile = Path.Combine(directory, window.Name + ".xml");
                var saveCopy = window.CreateCopy();
                CleanWindowStyles(saveCopy);
                SerializationHelper.Serialize<XmlWindow>(saveCopy, windowFile);
                savedFiles.Add(windowFile);
            }
            FileHelpers.TryDelete(DirectoryHelpers.GetFiles(directory, "*.xml").Where(f => !savedFiles.Contains(f)));
        }

        private void SaveDialogXmls(IEnumerable<XmlDialog> windows, string directory)
        {
            DirectoryHelpers.CreateIfNotExists(directory);
            var savedFiles = new List<string>();
            foreach (var dialog in windows)
            {
                string dialogFile = Path.Combine(directory, dialog.Name + ".xml");
                var saveCopy = dialog.CreateCopy();
                CleanWindowStyles(saveCopy);
                SerializationHelper.Serialize<XmlDialog>(saveCopy, dialogFile);
                savedFiles.Add(dialogFile);
            }
            FileHelpers.TryDelete(DirectoryHelpers.GetFiles(directory, "*.xml").Where(f => !savedFiles.Contains(f)));
        }



        #region Styles

        [XmlIgnore]
        [Browsable(false)]
        public XmlStyleCollection Style
        {
            get { return _style; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<string, XmlStyleCollection> Styles
        {
            get { return _styles; }
        }

        public void LoadStyles()
        {
            _styles.Clear();
            if (Directory.Exists(SkinStyleFolder))
            {
                foreach (var styleXml in Directory.GetFiles(SkinStyleFolder, "*.xml"))
                {
                    string styleName = Path.GetFileNameWithoutExtension(styleXml);
                    var styleCollection = SerializationHelper.Deserialize<XmlStyleCollection>(styleXml);
                    if (styleCollection != null && !_styles.ContainsKey(styleName))
                    {
                        styleCollection.Name = styleName;
                        _styles.Add(styleName, styleCollection);
                        styleCollection.InitializeStyleCollection();
                    }
                }

                if (!_styles.ContainsKey("Default"))
                {
                    _styles.Add("Default", new XmlStyleCollection());
                }

                _style = _styles.ContainsKey(CurrentStyle) ? _styles[CurrentStyle] : _styles["Default"];
            }
        }

        public void SaveStyles()
        {
            DirectoryHelpers.CreateIfNotExists(SkinStyleFolder);
            foreach (var styleCollection in _styles)
            {
                string filename = Path.Combine(SkinStyleFolder, string.Format("{0}.xml", styleCollection.Key));
                var newCollection = styleCollection.Value.CreateCopy();
                if (newCollection != null)
                {
                    foreach (var controlStyle in newCollection.ControlStyles)
                    {
                        CleanStyles(controlStyle);
                    }
                    SerializationHelper.Serialize<XmlStyleCollection>(newCollection, filename);
                }
            }
        }

        public void SetStyle(string _selectedStyle)
        {
            if (!string.IsNullOrEmpty(_selectedStyle) && _styles.ContainsKey(_selectedStyle))
            {
                _style = Styles[_selectedStyle];

                foreach (var window in Windows)
                {
                    window.ApplyStyle(_style);
                    foreach (var control in window.Controls.GetControls())
                    {
                        control.ApplyStyle(_style);
                    }
                }

                foreach (var dialog in Dialogs)
                {
                    dialog.ApplyStyle(_style);
                    foreach (var control in dialog.Controls.GetControls())
                    {
                        control.ApplyStyle(_style);
                    }
                }
            }
        }

        public void CycleTheme()
        {
            if (Styles.Count > 1)
            {
                var themes = Styles.Keys.ToList();
                int current = themes.IndexOf(_style.Name);
                int next = (current == (themes.Count -1)) ? 0 : current + 1;
                SetStyle(themes[next]);
            }
        }

        private void CleanWindowStyles(IXmlControlHost window)
        {
            CleanStyles(window);
            foreach (var item in window.Controls.GetControls())
            {
                CleanStyles(item);
            }
        }

        private void CleanStyles(object obj)
        {
            if (obj != null)
            {
                foreach (var property in obj.GetType().GetProperties().Where(p => typeof(XmlStyle).IsAssignableFrom(p.PropertyType)))
                {
                    var style = property.GetValue(obj) as XmlStyle;
                    if (style != null)
                    {
                        if (!string.IsNullOrEmpty(style.StyleId))
                        {
                            var newStyle = Activator.CreateInstance(style.GetType()) as XmlStyle;
                            if (newStyle is XmlBrush)
                            {
                                newStyle = new XmlBrush();
                            }
                            newStyle.StyleId = style.StyleId;
                            property.SetValue(obj, newStyle);
                        }
                        CleanStyles(style);
                    }
                }
            }
        }

        #endregion

        #region Images

        private void LoadImages()
        {
            // Load Images
            _images.Clear();
            Log.Message(LogLevel.Info, "Loading skin images...");
            if (Directory.Exists(SkinImageFolder))
            {
                Log.Message(LogLevel.Info, "Image directory found, Directory: {0}", SkinImageFolder);
                foreach (var imageFile in Directory.GetFiles(SkinImageFolder, "*.*", SearchOption.AllDirectories))
                {
                    string subfolder = Path.GetDirectoryName(imageFile).Replace(SkinImageFolder.Trim('\\'), "");
                    string xmlname = imageFile.Replace(SkinImageFolder.Trim('\\'), "");
                    string displayName = Path.GetFileNameWithoutExtension(imageFile);
                    Images.Add(new XmlImageFile
                    {
                        DisplayName = displayName,
                        XmlName = xmlname,
                        SubFolder = subfolder == string.Empty ? "Images" : subfolder.TrimStart('\\'),
                        FileName = imageFile
                    });
                    Log.Message(LogLevel.Verbose, "Skin image added, Name: {0}, File: {1}", xmlname, imageFile);
                }
                Log.Message(LogLevel.Info, "Loading skin iamges complete, Image count: {0}", Images.Count);
                return;
            }
            Log.Message(LogLevel.Warn, "Image directory not found, Directory missing: {0}", SkinImageFolder);
        }

        public void ReloadImages()
        {
            LoadImages();
        }

        private void SaveImages(XmlSkinInfo skinInfo)
        {
            if (skinInfo.SkinImageFolder != SkinImageFolder)
            {
                DirectoryHelpers.CreateIfNotExists(skinInfo.SkinImageFolder);
                foreach (var image in Images)
                {
                    FileHelpers.CopyFile(image.FileName, image.FileName.Replace(SkinImageFolder,skinInfo.SkinImageFolder));
                }
            }
        }

        public byte[] GetImageValue(string xmlname)
        {
            if (_images != null)
            {
                var image = _images.FirstOrDefault(x => x.XmlName.Equals(xmlname, StringComparison.OrdinalIgnoreCase));
                if (image != null && File.Exists(image.FileName))
                {
                    return FileHelpers.ReadBytesFromFile(image.FileName);
                }
            }
            return null;
        }

        #endregion

        #region Properties

        private void LoadProperties()
        {
            Log.Message(LogLevel.Info, "Loading skin properties...");
            string propertyXmlFile = Path.Combine(SkinFolderPath, "PropertyInfo.xml");
            if (File.Exists(propertyXmlFile))
            {
                Log.Message(LogLevel.Info, "PropertyInfo file found: {0}", propertyXmlFile);
                 var info = SerializationHelper.Deserialize<XmlPropertyInfo>(propertyXmlFile);
                if (info != null)
                {
                    _propertyInfo = new XmlPropertyInfo();

                    var internals = info.AllProperties.Where(x => x.IsInternal).Select(x => x.SkinTag).ToList();
                    foreach (var item in info.AllProperties.Where(x => !x.IsInternal).OrderByDescending(p => p.PropertyType))
                    {
                        if (!internals.Contains(item.SkinTag))
                        {
                            _propertyInfo.Properties.Add(item);
                        }
                    }

                    Log.Message(LogLevel.Info, "Loaded PropertyInfo file.");
                    return;
                }
                _propertyInfo = new XmlPropertyInfo();
                Log.Message(LogLevel.Warn, "Failed to load PropertyInfo file.");
                return;
            }
            Log.Message(LogLevel.Warn, "PropertyInfo file not found: {0}", propertyXmlFile);
        }

        private void SaveProperties()
        {
            Log.Message(LogLevel.Info, "Saving skin PropertyInfo...");
            string propertyXmlFile = Path.Combine(SkinFolderPath, "PropertyInfo.xml");
            var info = new XmlPropertyInfo();
            var internals = _propertyInfo.AllProperties.Where(x => x.IsInternal).Select(x => x.SkinTag).ToList();
            foreach (var item in _propertyInfo.AllProperties.Where(x => !x.IsInternal).OrderByDescending(p => p.PropertyType))
            {
                if (!internals.Contains(item.SkinTag))
                {
                    info.Properties.Add(item);
                }
            }

            if (!SerializationHelper.Serialize<XmlPropertyInfo>(info, propertyXmlFile))
            {
                Log.Message(LogLevel.Warn, "Failed to save skin PropertyInfo.");
                return;
            }
            Log.Message(LogLevel.Info, "Saving skin PropertyInfo complete.");
        }

        #endregion

        #region Language

        public void SetLanguage(string _selectedLanguage)
        {
            CurrentLanguage = _selectedLanguage;
            foreach (var window in Windows)
            {
                foreach (var control in window.Controls.GetControls())
                {
                    control.NotifyPropertyChanged("Image");
                    control.NotifyPropertyChanged("LabelText");
                }
            }

            foreach (var dialog in Dialogs)
            {
                foreach (var control in dialog.Controls.GetControls())
                {
                    control.NotifyPropertyChanged("Image");
                    control.NotifyPropertyChanged("LabelText");
                }
            }
        }

        public string GetLanguageValue(string skinTag)
        {
            if (_language != null)
            {
                var entry = _language.LanguageEntries.FirstOrDefault(x => x.SkinTag == skinTag);
                if (entry != null)
                {
                    var value = entry.Values.FirstOrDefault(x => x.Language == CurrentLanguage) ?? entry.Values.FirstOrDefault();
                    return value == null ? skinTag : value.Value;
                }
            }
            return skinTag;
        }

        public IEnumerable<string> Languages
        {
            get
            {
                if (_language != null)
                {
                    return _language.LanguageEntries.SelectMany(x => x.Values).Select(x => x.Language).Distinct();
                }
                return null;
            }
        }

        private void LoadLanguage()
        {
            // Load Language
            string languageFile = Path.Combine(SkinFolderPath, "Language.xml");
            Log.Message(LogLevel.Info, "Loading skin language file...{0}", languageFile);
            if (File.Exists(languageFile))
            {
                _language = SerializationHelper.Deserialize<XmlLanguage>(languageFile);
                if (_language == null)
                {
                    Log.Message(LogLevel.Warn, "Failed to load language file, File: {0}", languageFile);
                    return;
                }
                Log.Message(LogLevel.Info, "Loading skin languages complete. Current language: {0}", CurrentLanguage);
             
            }
            Log.Message(LogLevel.Warn, "Language file not found, File missing: {0}", languageFile);
        }

        private void SaveLanguage()
        {
            Log.Message(LogLevel.Info, "Saving skin language...");
            string languageFile = Path.Combine(SkinFolderPath, "Language.xml");
            if (!SerializationHelper.Serialize<XmlLanguage>(_language, languageFile))
            {
                Log.Message(LogLevel.Error, "Failed to save language file, File: {0}", languageFile);
            }
            Log.Message(LogLevel.Info, "Saving skin language complete.");
        }

        #endregion


      
    }
}
