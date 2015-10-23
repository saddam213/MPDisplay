using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Common.Helpers;
using Common.Settings;
using SkinEditor.Views;

namespace SkinEditor
{
    [Serializable]
    public class SkinEditorSettings : EditorSettingsObject
    {
        public static SkinEditorSettings LoadSettings(string filename)
        {
            var settings = new SkinEditorSettings { Filename = filename }; 
            if (File.Exists(filename))
            {
                settings = SerializationHelper.Deserialize<SkinEditorSettings>(filename) ?? new SkinEditorSettings { Filename = filename };
            }
            settings.InitializeSettings();
            return settings;
        }

      
        private GlobalSettings _globalSettings = new GlobalSettings();
        private SkinEditorViewSettings _skinEditorViewSettings = new SkinEditorViewSettings();
        private StyleEditorViewSettings _styleEditorViewSettings = new StyleEditorViewSettings();
        private ImageEditorViewSettings _imageEditorViewSettings = new ImageEditorViewSettings();
        private SkinInfoEditorViewSettings _skinInfoEditorViewSettings = new SkinInfoEditorViewSettings();
        private InfoEditorViewSettings _infoEditorViewSettings = new InfoEditorViewSettings();

        public string Filename { get; set; }

        public GlobalSettings GlobalSettings
        {
            get { return _globalSettings; }
            set { _globalSettings = value; NotifyPropertyChanged("GlobalSettings"); }
        }
        
        public SkinEditorViewSettings SkinEditorViewSettings
        {
            get { return _skinEditorViewSettings; }
            set { _skinEditorViewSettings = value; NotifyPropertyChanged("SkinEditorViewSettings"); }
        }

        public StyleEditorViewSettings StyleEditorViewSettings
        {
            get { return _styleEditorViewSettings; }
            set { _styleEditorViewSettings = value; NotifyPropertyChanged("GlobaStyleEditorViewSettingslSettings"); }
        }

        public ImageEditorViewSettings ImageEditorViewSettings
        {
            get { return _imageEditorViewSettings; }
            set { _imageEditorViewSettings = value; NotifyPropertyChanged("ImageEditorViewSettings"); }
        }

        public SkinInfoEditorViewSettings SkinInfoEditorViewSettings
        {
            get { return _skinInfoEditorViewSettings; }
            set { _skinInfoEditorViewSettings = value; NotifyPropertyChanged("SkinInfoEditorViewSettings"); }
        }

        public InfoEditorViewSettings InfoEditorViewSettings
        {
            get { return _infoEditorViewSettings; }
            set { _infoEditorViewSettings = value; NotifyPropertyChanged("InfoEditorViewSettings"); }
        }


        public override void InitializeSettings()
        {
            base.InitializeSettings();
            GlobalSettings.InitializeSettings();
            SkinEditorViewSettings.InitializeSettings();
            StyleEditorViewSettings.InitializeSettings();
            ImageEditorViewSettings.InitializeSettings();
        }

        public void Save(string settingsFile)
        {
            SerializationHelper.Serialize(this, settingsFile);
        }

       
    }

    public class EditorSettingsObject : INotifyPropertyChanged
    {
        public virtual void InitializeSettings()
        {
        }

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

    public class GlobalSettings : EditorSettingsObject
    {
        private string _lastSkinDirectory = RegistrySettings.MPDisplaySkinFolder;
        public string LastSkinDirectory
        {
            get { return _lastSkinDirectory; }
            set { _lastSkinDirectory = value; NotifyPropertyChanged("LastSkinDirectory"); }
        }

        private ObservableCollection<string> _recentSkins = new ObservableCollection<string>();
        public ObservableCollection<string> RecentSkins
        {
            get { return _recentSkins; }
            set { _recentSkins = value; NotifyPropertyChanged("RecentSkins"); }
        }

        public override void InitializeSettings()
        {
            base.InitializeSettings();
            RecentSkins = new ObservableCollection<string>(RecentSkins.Where(File.Exists).Take(10));
        }

        public void AddToRecentList(string skinInfoFile)
        {
            if (RecentSkins.Contains(skinInfoFile))
            {
                RecentSkins.Move(RecentSkins.IndexOf(skinInfoFile), 0);
                return;
            }
            RecentSkins.Insert(0, skinInfoFile);
            if (RecentSkins.Count > 10)
            {
                RecentSkins.Remove(RecentSkins.Last());
            }
        }
    }

 
    
}
