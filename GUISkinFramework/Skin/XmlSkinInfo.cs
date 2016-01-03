using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Common.Helpers;
using Common.Log;
using Common.Settings;

namespace GUISkinFramework.Skin
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
                if (_currentLanguage == value) return;
                _currentLanguage = value;
                NotifyPropertyChanged("CurrentLanguage");
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
          public string SkinInfoPath => SkinFolderPath + "\\SkinInfo.xml";

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlFolder => SkinFolderPath + "\\SkinFiles\\";

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlWindowFolder => SkinXmlFolder + "Windows\\";

        [XmlIgnore]
        [Browsable(false)]
        public string SkinXmlDialogFolder => SkinXmlFolder + "Dialogs\\";

        [XmlIgnore]
        [Browsable(false)]
        public string SkinImageFolder => SkinFolderPath + "\\Images\\";

        [XmlIgnore]
        [Browsable(false)]
        public string SkinStyleFolder => SkinFolderPath + "\\Styles\\";

        [XmlIgnore]
        [Browsable(false)]
        public string SplashScreenImage => SkinImageFolder + "SplashScreen.png";

        [XmlIgnore]
        [Browsable(false)]
        public string ErrorScreenImage => SkinImageFolder + "ErrorScreen.png";

        [XmlIgnore]
        [Browsable(false)]
        public string PreviewImage => SkinImageFolder + "Preview.png";

        public event PropertyChangedEventHandler PropertyChanged;
    
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private readonly Log _log = LoggingManager.GetLog(typeof(XmlSkinInfo));
        private readonly ObservableCollection<XmlImageFile> _images = new ObservableCollection<XmlImageFile>();

        private Dictionary<string, XmlStyleCollection> _styles = new Dictionary<string, XmlStyleCollection>();
        private XmlStyleCollection _style = new XmlStyleCollection();
        private ObservableCollection<XmlWindow> _windows = new ObservableCollection<XmlWindow>();
        private ObservableCollection<XmlDialog> _dialogs = new ObservableCollection<XmlDialog>();

        private XmlLanguage _language = new XmlLanguage();

        private XmlPropertyInfo _propertyInfo = new XmlPropertyInfo();

        public void LoadXmlSkin()
        {
            _log.Message(LogLevel.Info, "Loading skin: {0} ....", _skinName);
            if (Directory.Exists(SkinFolderPath))
            {
                LoadImages();
                LoadLanguage();
                LoadProperties();
                LoadStyles();
                LoadWindows();
            }
            _log.Message(LogLevel.Info, "Loading skin: {0} complete.", _skinName);
        }

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlImageFile> Images => _images;

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlProperty> Properties => _propertyInfo.AllProperties;

        [XmlIgnore]
        [Browsable(false)]
        public XmlLanguage Language => _language;


        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlWindow> Windows => _windows;

        [XmlIgnore]
        [Browsable(false)]
        public ObservableCollection<XmlDialog> Dialogs => _dialogs;

        private void LoadWindows()
        {
            // Load Windows
            _windows.Clear();
            _dialogs.Clear();
            if (!Directory.Exists(SkinXmlFolder)) return;

            foreach (var window in DirectoryHelpers.GetFiles(SkinXmlWindowFolder, "*.xml").Select(SerializationHelper.Deserialize<XmlWindow>).Where(window => window != null))
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

            foreach (var dialog in DirectoryHelpers.GetFiles(SkinXmlDialogFolder, "*.xml").Select(SerializationHelper.Deserialize<XmlDialog>).Where(dialog => dialog != null))
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



        public void AddWindow(XmlWindow window)
        {
            if (window == null) return;

            foreach (var control in window.Controls.GetControls())
            {
                control.WindowId = window.Id;
            }
            Windows.Add(window);
        }

        public void AddDialog(XmlDialog xmlDialog)
        {
            if (xmlDialog == null) return;

            foreach (var control in xmlDialog.Controls.GetControls())
            {
                control.WindowId = xmlDialog.Id;
            }
            Dialogs.Add(xmlDialog);
        }

        public void CreateSkin()
        {
            DirectoryHelpers.CreateIfNotExists(SkinFolderPath);
            DirectoryHelpers.CreateIfNotExists(SkinImageFolder);
            DirectoryHelpers.CreateIfNotExists(SkinStyleFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlWindowFolder);
            DirectoryHelpers.CreateIfNotExists(SkinXmlDialogFolder);
            SerializationHelper.Serialize(this, Path.Combine(SkinFolderPath, "SkinInfo.xml"));
            SerializationHelper.Serialize(new XmlPropertyInfo(), Path.Combine(SkinFolderPath, "PropertyInfo.xml"));
            SerializationHelper.Serialize(new XmlLanguage(), Path.Combine(SkinFolderPath, "Language.xml"));
            SerializationHelper.Serialize(new XmlStyleCollection(), Path.Combine(SkinStyleFolder, "Default.xml"));
        }


        public void SaveSkin()
        {
            DirectoryHelpers.CreateIfNotExists(SkinXmlFolder);
            SerializationHelper.Serialize(this, Path.Combine(SkinFolderPath, "SkinInfo.xml"));
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
                SkinWidth = SkinWidth,
                SkinHeight = SkinHeight
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


        private static void SaveWindowXmls(IEnumerable<XmlWindow> windows, string directory)
        {
            DirectoryHelpers.CreateIfNotExists(directory);
            var savedFiles = new List<string>();
            foreach (var window in windows)
            {
                var windowFile = Path.Combine(directory, window.Name + ".xml");
                var saveCopy = window.CreateCopy();
                CleanWindowStyles(saveCopy);
                SerializationHelper.Serialize(saveCopy, windowFile);
                savedFiles.Add(windowFile);
            }
            FileHelpers.TryDelete(DirectoryHelpers.GetFiles(directory, "*.xml").Where(f => !savedFiles.Contains(f)));
        }

        private static void SaveDialogXmls(IEnumerable<XmlDialog> windows, string directory)
        {
            DirectoryHelpers.CreateIfNotExists(directory);
            var savedFiles = new List<string>();
            foreach (var dialog in windows)
            {
                var dialogFile = Path.Combine(directory, dialog.Name + ".xml");
                var saveCopy = dialog.CreateCopy();
                CleanWindowStyles(saveCopy);
                SerializationHelper.Serialize(saveCopy, dialogFile);
                savedFiles.Add(dialogFile);
            }
            FileHelpers.TryDelete(DirectoryHelpers.GetFiles(directory, "*.xml").Where(f => !savedFiles.Contains(f)));
        }



        #region Styles

        [XmlIgnore]
        [Browsable(false)]
        public XmlStyleCollection Style => _style;

        [XmlIgnore]
        [Browsable(false)]
        public Dictionary<string, XmlStyleCollection> Styles => _styles;

        public void LoadStyles()
        {
            _styles.Clear();
            if (!Directory.Exists(SkinStyleFolder)) return;
            foreach (var styleXml in Directory.GetFiles(SkinStyleFolder, "*.xml"))
            {
                var styleName = Path.GetFileNameWithoutExtension(styleXml);
                var styleCollection = SerializationHelper.Deserialize<XmlStyleCollection>(styleXml);
                if (styleName != null && (styleCollection == null || _styles.ContainsKey(styleName))) continue;
                styleCollection.Name = styleName;
                if (styleName != null) _styles.Add(styleName, styleCollection);
                styleCollection.InitializeStyleCollection();
            }

            if (!_styles.ContainsKey("Default"))
            {
                _styles.Add("Default", new XmlStyleCollection());
            }

            _style = _styles.ContainsKey(CurrentStyle) ? _styles[CurrentStyle] : _styles["Default"];
        }

        public void SaveStyles()
        {
            DirectoryHelpers.CreateIfNotExists(SkinStyleFolder);
            foreach (var styleCollection in _styles)
            {
                var filename = Path.Combine(SkinStyleFolder, $"{styleCollection.Key}.xml");
                var newCollection = styleCollection.Value.CreateCopy();
                if (newCollection == null) continue;

                foreach (var controlStyle in newCollection.ControlStyles)
                {
                    CleanStyles(controlStyle);
                }
                SerializationHelper.Serialize(newCollection, filename);
            }
        }

        public void SetStyle(string selectedStyle)
        {
            if (string.IsNullOrEmpty(selectedStyle) || !_styles.ContainsKey(selectedStyle)) return;

            _style = Styles[selectedStyle];

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

        public void CycleTheme()
        {
            if (Styles.Count <= 1) return;
            var themes = Styles.Keys.ToList();
            var current = themes.IndexOf(_style.Name);
            var next = current == themes.Count -1 ? 0 : current + 1;
            SetStyle(themes[next]);
        }

        private static void CleanWindowStyles(IXmlControlHost window)
        {
            CleanStyles(window);
            foreach (var item in window.Controls.GetControls())
            {
                CleanStyles(item);
            }
        }

        private static void CleanStyles(object obj)
        {
            if (obj == null) return;
            foreach (var property in obj.GetType().GetProperties().Where(p => typeof(XmlStyle).IsAssignableFrom(p.PropertyType)))
            {
                var style = property.GetValue(obj) as XmlStyle;
                if (style == null) continue;
                if (!string.IsNullOrEmpty(style.StyleId))
                {
                    var newStyle = Activator.CreateInstance(style.GetType()) as XmlStyle;
                    if (newStyle is XmlBrush)
                    {
                        newStyle = new XmlBrush();
                    }
                    if (newStyle != null)
                    {
                        newStyle.StyleId = style.StyleId;
                        property.SetValue(obj, newStyle);
                    }
                }
                CleanStyles(style);
            }
        }

        #endregion

        #region Images

        private void LoadImages()
        {
            // Load Images
            _images.Clear();
            _log.Message(LogLevel.Info, "Loading skin images...");
            if (Directory.Exists(SkinImageFolder))
            {
                _log.Message(LogLevel.Info, "Image directory found, Directory: {0}", SkinImageFolder);
                foreach (var imageFile in Directory.GetFiles(SkinImageFolder, "*.*", SearchOption.AllDirectories))
                {
                    var directoryName = Path.GetDirectoryName(imageFile);
                    if (directoryName == null) continue;
                    var subfolder = directoryName.Replace(SkinImageFolder.Trim('\\'), "");
                    var xmlname = imageFile.Replace(SkinImageFolder.Trim('\\'), "");
                    var displayName = Path.GetFileNameWithoutExtension(imageFile);
                    Images.Add(new XmlImageFile
                    {
                        DisplayName = displayName,
                        XmlName = xmlname,
                        SubFolder = subfolder == string.Empty ? "Images" : subfolder.TrimStart('\\'),
                        FileName = imageFile
                    });
                    _log.Message(LogLevel.Verbose, "Skin image added, Name: {0}, File: {1}", xmlname, imageFile);
                }
                _log.Message(LogLevel.Info, "Loading skin images complete, Image count: {0}", Images.Count);
                return;
            }
            _log.Message(LogLevel.Warn, "Image directory not found, Directory missing: {0}", SkinImageFolder);
        }

        public void ReloadImages()
        {
            LoadImages();
        }

        private void SaveImages(XmlSkinInfo skinInfo)
        {
            if (skinInfo.SkinImageFolder == SkinImageFolder) return;
            DirectoryHelpers.CreateIfNotExists(skinInfo.SkinImageFolder);
            foreach (var image in Images)
            {
                FileHelpers.CopyFile(image.FileName, image.FileName.Replace(SkinImageFolder,skinInfo.SkinImageFolder));
            }
        }

        public byte[] GetImageValue(string xmlname)
        {
            var image = _images?.FirstOrDefault(x => x.XmlName.Equals(xmlname, StringComparison.OrdinalIgnoreCase));
            if (image != null && File.Exists(image.FileName))
            {
                return FileHelpers.ReadBytesFromFile(image.FileName);
            }
            return null;
        }

        #endregion

        #region Properties

        private void LoadProperties()
        {
            _log.Message(LogLevel.Info, "Loading skin properties...");
            var propertyXmlFile = Path.Combine(SkinFolderPath, "PropertyInfo.xml");
            if (File.Exists(propertyXmlFile))
            {
                _log.Message(LogLevel.Info, "PropertyInfo file found: {0}", propertyXmlFile);
                var info = SerializationHelper.Deserialize<XmlPropertyInfo>(propertyXmlFile);
                if (info != null)
                {
                    var addImageSettings = SettingsManager.Load<AddImageSettings>(Path.Combine(RegistrySettings.ProgramDataPath, "AddImageSettings.xml")) ??
                                           new AddImageSettings();

                    _propertyInfo = new XmlPropertyInfo();

                    foreach (var p in addImageSettings.AddImagePropertySettings)
                    {
                        _propertyInfo.AddInternalProperty(new XmlProperty { SkinTag = p.MPDSkinTag, DesignerValue = "New...", PropertyType = XmlPropertyType.Image });
                    }

                    var internals = info.AllProperties.Where(x => x.IsInternal).Select(x => x.SkinTag).ToList();
                    foreach (var item in info.AllProperties.Where(x => !x.IsInternal).OrderByDescending(p => p.PropertyType).Where(item => !internals.Contains(item.SkinTag)))
                    {
                        _propertyInfo.Properties.Add(item);
                    }

                    _log.Message(LogLevel.Info, "Loaded PropertyInfo file.");
                    return;
                }
                _propertyInfo = new XmlPropertyInfo();
                _log.Message(LogLevel.Warn, "Failed to load PropertyInfo file.");
                return;
            }
            _log.Message(LogLevel.Warn, "PropertyInfo file not found: {0}", propertyXmlFile);
        }

        private void SaveProperties()
        {
            _log.Message(LogLevel.Info, "Saving skin PropertyInfo...");
            var propertyXmlFile = Path.Combine(SkinFolderPath, "PropertyInfo.xml");
            var info = new XmlPropertyInfo();
            var internals = _propertyInfo.AllProperties.Where(x => x.IsInternal).Select(x => x.SkinTag).ToList();
            foreach (var item in _propertyInfo.AllProperties.Where(x => !x.IsInternal).OrderByDescending(p => p.PropertyType).Where(item => !internals.Contains(item.SkinTag)))
            {
                info.Properties.Add(item);
            }

            if (!SerializationHelper.Serialize(info, propertyXmlFile))
            {
                _log.Message(LogLevel.Warn, "Failed to save skin PropertyInfo.");
                return;
            }
            _log.Message(LogLevel.Info, "Saving skin PropertyInfo complete.");
        }

        #endregion

        #region Language

        public void SetLanguage(string selectedLanguage)
        {
            CurrentLanguage = selectedLanguage;
            foreach (var control in Windows.SelectMany(window => window.Controls.GetControls()))
            {
                control.NotifyPropertyChanged("Image");
                control.NotifyPropertyChanged("LabelText");
            }

            foreach (var control in Dialogs.SelectMany(dialog => dialog.Controls.GetControls()))
            {
                control.NotifyPropertyChanged("Image");
                control.NotifyPropertyChanged("LabelText");
            }
        }

        public string GetLanguageValue(string skinTag)
        {
            if (_language == null) return skinTag;
            var entry = _language.LanguageEntries.FirstOrDefault(x => x.SkinTag == skinTag);
            if (entry == null) return skinTag;
            var value = entry.Values.FirstOrDefault(x => x.Language == CurrentLanguage) ?? entry.Values.FirstOrDefault();
            return value == null ? skinTag : value.Value;
        }

        public IEnumerable<string> Languages
        {
            get
            {
                return _language?.LanguageEntries.SelectMany(x => x.Values).Select(x => x.Language).Distinct();
            }
        }

        private void LoadLanguage()
        {
            // Load Language
            var languageFile = Path.Combine(SkinFolderPath, "Language.xml");
            _log.Message(LogLevel.Info, "Loading skin language file...{0}", languageFile);
            if (File.Exists(languageFile))
            {
                _language = SerializationHelper.Deserialize<XmlLanguage>(languageFile);
                if (_language == null)
                {
                    _log.Message(LogLevel.Warn, "Failed to load language file, File: {0}", languageFile);
                    return;
                }
                _log.Message(LogLevel.Info, "Loading skin languages complete. Current language: {0}", CurrentLanguage);
            }
            else
            {
                _log.Message(LogLevel.Warn, "Language file not found, File missing: {0}", languageFile);
            }
        }

        private void SaveLanguage()
        {
            _log.Message(LogLevel.Info, "Saving skin language...");
            var languageFile = Path.Combine(SkinFolderPath, "Language.xml");
            if (!SerializationHelper.Serialize(_language, languageFile))
            {
                _log.Message(LogLevel.Error, "Failed to save language file, File: {0}", languageFile);
            }
            _log.Message(LogLevel.Info, "Saving skin language complete.");
        }

        #endregion


      
    }
}
