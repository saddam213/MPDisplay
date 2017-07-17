using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using GUISkinFramework.Skin;
using MPDisplay.Common.Utils;
using SkinEditor.BindingConverters;
using SkinEditor.Dialogs;
using SkinEditor.Helpers;
using SkinEditor.Themes;
using SkinEditor.Views;

namespace SkinEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly string _settingsFile;
        private XmlSkinInfo _currentSkinInfo;
        private SkinEditorSettings _settings;
        private readonly ConnectionHelper _connectionHelper;

        public MainWindow()
        {
           
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 24 });
            TextOptions.TextFormattingModeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata { DefaultValue = TextFormattingMode.Display });
            TextOptions.TextRenderingModeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata { DefaultValue = TextRenderingMode.Aliased });

           LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "SkinEditor", RegistrySettings.LogLevel));

            _settingsFile = Path.Combine(RegistrySettings.ProgramDataPath, "SkinEditor\\Settings.xml");

            InitializeComponent();
            CreateContextMenuCommands();
          //  Application.Current.Resources = new ResourceDictionary { Source = new Uri("/GUIFramework;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) };

            Settings = SkinEditorSettings.LoadSettings(_settingsFile);
            _connectionHelper = new ConnectionHelper();

            LoadViews();

            // If commandline argument was passed, try to open this skin
            if (App.StartupSkinInfoFilename.Length > 1)
            {
                SkinOpenRecent(App.StartupSkinInfoFilename);
            }
            // else try ty load the last skin used, if setting is enabled
            else
            {
                if (Settings.GlobalSettings.AutoLoadLastSkin)
                {
                    SkinOpenRecent(Settings.GlobalSettings.RecentSkins.FirstOrDefault());
                }
                
            }
        }

        private static void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
          
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
        }

        #region Properties

        public ObservableCollection<EditorViewModel> EditorViews { get; set; } = new ObservableCollection<EditorViewModel>();


        public XmlSkinInfo CurrentSkinInfo
        {
            get { return _currentSkinInfo; }
            set { _currentSkinInfo = value; NotifyPropertyChanged("CurrentSkinInfo"); }
        }

        public SkinEditorSettings Settings
        {
            get { return _settings; }
            set { _settings = value; NotifyPropertyChanged("Settings"); }
        }

        public bool HasPendingChanges
        {
            get { return EditorViews.Any(v => v.HasPendingChanges); }
        }

        #endregion

        private void LoadViews()
        {
            if (EditorViews.Count != 0) return;

            EditorViews.Add(new SkinEditorView( _connectionHelper){ EditorSettings = Settings.SkinEditorViewSettings, ConnectSettings = Settings.InfoEditorViewSettings});
            EditorViews.Add(new StyleEditorView { EditorSettings = Settings.StyleEditorViewSettings });
            EditorViews.Add(new ImageEditorView { EditorSettings = Settings.ImageEditorViewSettings });
            EditorViews.Add(new SkinInfoEditorView { EditorSettings = Settings.SkinInfoEditorViewSettings });
            EditorViews.Add(new InfoEditorView(_connectionHelper) { EditorSettings = Settings.InfoEditorViewSettings, ConnectSettings = Settings.InfoEditorViewSettings});
            EditorViews.Add(new TestEditorView());
        }

        private void LoadSkin(XmlSkinInfo skinInfo)
        {
            skinInfo.LoadXmlSkin();
            SkinEditorInfoManager.LoadSkinInfo(skinInfo);
            foreach (var editorView in EditorViews)
            {
                editorView.SkinInfo = skinInfo;
            }  
        }

        private void ClearPendingChanges()
        {
            foreach (var editorView in EditorViews)
            {
                editorView.HasPendingChanges = false;
            }
        }

        private bool SavePendingChanges()
        {
            if (!HasPendingChanges) return true;

            var result = MessageBox.Show(string.Format("There are unsaved changes in the current skin{0}{0}Would you like to save changes now?",
                Environment.NewLine), "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Cancel:
                    return false;
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Yes:
                    SkinSave();
                    return true;
            }
            return true;
        }

        #region FileMenu

        public ICommand NewSkinCommand { get; internal set; }
        public ICommand OpenSkinCommand { get; internal set; }
        public ICommand SaveSkinCommand { get; internal set; }
        public ICommand SaveAsSkinCommand { get; internal set; }
        public ICommand SkinEditorOptionsCommand { get; internal set; }
        public ICommand ExitCommand { get; internal set; }
        public ICommand RecentSkinCommand { get; internal set; }

        /// <summary>
        /// Populates the context menu.
        /// </summary>
        private void CreateContextMenuCommands()
        {
            NewSkinCommand = new RelayCommand(SkinNew);
            OpenSkinCommand = new RelayCommand(SkinOpen);
            SaveSkinCommand = new RelayCommand(SkinSave, CanExecuteSaveSkinCommand);
            SaveAsSkinCommand = new RelayCommand(SkinSaveAs,  CanExecuteSaveAsSkinCommand);
            SkinEditorOptionsCommand = new RelayCommand( SkinEditorOptions);
            ExitCommand = new RelayCommand( SkinEditorExit);
            RecentSkinCommand = new RelayCommand<string>(SkinOpenRecent);
        }
     
        private bool CanExecuteSaveSkinCommand()
        {
            return HasPendingChanges;
        }

        private bool CanExecuteSaveAsSkinCommand()
        {
            return CurrentSkinInfo != null;
        }
     
        private void SkinNew()
        {
            try
            {
                if (!SavePendingChanges()) return;

                ClearPendingChanges();
                var newSkinDialog = new NewSkinDialog(Settings.GlobalSettings.LastSkinDirectory);
                if (newSkinDialog.ShowDialog() != true) return;

                Settings.GlobalSettings.LastSkinDirectory = newSkinDialog.SkinFolder;
                CurrentSkinInfo = newSkinDialog.NewSkinInfo;
                Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                LoadSkin(CurrentSkinInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured creating skin:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }

        private void SkinOpen()
        {
            try
            {
                if (!SavePendingChanges()) return;

                ClearPendingChanges();
                var skinInfoFile = FileSystemHelper.OpenFileDialog(Settings.GlobalSettings.LastSkinDirectory, "SkinInfo (SkinInfo.xml)|SkinInfo.xml");
                if (string.IsNullOrEmpty(skinInfoFile)) return;

                var skinInfo = SerializationHelper.Deserialize<XmlSkinInfo>(skinInfoFile);
                if (skinInfo == null) return;

                skinInfo.SkinFolderPath = Path.GetDirectoryName(skinInfoFile);
                var folderPath = skinInfo.SkinFolderPath;
                if( folderPath != null) Settings.GlobalSettings.LastSkinDirectory = Directory.GetParent(folderPath).FullName;
                CurrentSkinInfo = skinInfo;
                Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                LoadSkin(CurrentSkinInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured opening skin:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }

        private void SkinSave()
        {
            try
            {
                if (CurrentSkinInfo == null) return;

                CurrentSkinInfo.SaveSkin();
                ClearPendingChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured saving skin:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }

        private void SkinSaveAs()
        {
            try
            {
                var newSkinDialog = new NewSkinDialog(Settings.GlobalSettings.LastSkinDirectory)
                {
                    IsNewSkin = false,
                    SkinName = CurrentSkinInfo.SkinName
                };
                if (newSkinDialog.ShowDialog() != true) return;

                CurrentSkinInfo.SaveSkinAs(newSkinDialog.SkinName, newSkinDialog.SkinFolder);
                CurrentSkinInfo = newSkinDialog.NewSkinInfo;
                Settings.GlobalSettings.LastSkinDirectory = newSkinDialog.SkinFolder;
                Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                ClearPendingChanges();
                LoadSkin(CurrentSkinInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured saving skin:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }

        private void SkinEditorOptions()
        {
            try
            {
                var skinEditorOptionsDialog = new SkinEditorOptionsDialog(Settings.GlobalSettings);

                if (skinEditorOptionsDialog.ShowDialog() != true) return;

                Settings.GlobalSettings.AutoLoadLastSkin = skinEditorOptionsDialog.AutoLoadLastSkin;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured editing SkinEditor options:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }
     

        private void SkinOpenRecent(string recentSkin)
        {
            if (string.IsNullOrEmpty(recentSkin) || !File.Exists(recentSkin)) return;

            if (CurrentSkinInfo != null && CurrentSkinInfo.SkinInfoPath.Equals(recentSkin)) return;

            if (!SavePendingChanges()) return;

            ClearPendingChanges();
            try
            {
                var skinInfo = SerializationHelper.Deserialize<XmlSkinInfo>(recentSkin);
                if (skinInfo == null) return;

                skinInfo.SkinFolderPath = Path.GetDirectoryName(recentSkin);
                CurrentSkinInfo = skinInfo;
                Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                LoadSkin(CurrentSkinInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured opening recent skin:{0}{0}{1}", Environment.NewLine, ex), "Error!");
            }
        }

        private void SkinEditorExit()
        {
            if (!SavePendingChanges()) return;

            ClearPendingChanges();
            Close();
        }


        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion 
     
    

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!SavePendingChanges())
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Settings.Save(_settingsFile);
            base.OnClosed(e);
            LoggingManager.Destroy();
        }



     

    }
}
