using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GUISkinFramework;
using Common.Logging;
using SkinEditor.Dialogs;
using SkinEditor.Helpers;
using SkinEditor.Views;
using System.Linq;
using System.Windows.Input;
using MPDisplay.Common.Utils;
using Common.Settings;
using Common.Helpers;

namespace SkinEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _settingsFile;
        private XmlSkinInfo _currentSkinInfo;
        private SkinEditorSettings _settings;

        public MainWindow()
        {
           
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);

            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 24 });
            TextOptions.TextFormattingModeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata { DefaultValue = TextFormattingMode.Display });
            TextOptions.TextRenderingModeProperty.OverrideMetadata(typeof(TextBlock), new FrameworkPropertyMetadata { DefaultValue = TextRenderingMode.Aliased });

//#if DEBUG
//            LoggingManager.AddLog(new WindowLogger("SkinEditor"));
//#else
           LoggingManager.AddLog(new FileLogger(RegistrySettings.ProgramDataPath + "Logs", "SkinEditor", RegistrySettings.LogLevel));
//#endif

            _settingsFile = Environment.CurrentDirectory + "\\Settings.xml";
            InitializeComponent();
            CreateContextMenuCommands();
          //  Application.Current.Resources = new ResourceDictionary { Source = new Uri("/GUIFramework;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute) };
            Settings = SkinEditorSettings.LoadSettings(_settingsFile);
            LoadViews();

            SkinOpenRecent(App.StartupSkinInfoFilename);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
          
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
        }

        #region Properties
         
        private ObservableCollection<EditorViewModel> _editorViews = new ObservableCollection<EditorViewModel>();

        public ObservableCollection<EditorViewModel> EditorViews
        {
            get { return _editorViews; }
            set { _editorViews = value; }
        }
        

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
            if (EditorViews.Count == 0)
            {
                EditorViews.Add(new SkinEditorView{ EditorSettings = Settings.SkinEditorViewSettings});
                EditorViews.Add(new StyleEditorView{ EditorSettings = Settings.StyleEditorViewSettings});
                EditorViews.Add(new ImageEditorView{ EditorSettings = Settings.ImageEditorViewSettings});
                EditorViews.Add(new SkinInfoEditorView { EditorSettings = Settings.SkinInfoEditorViewSettings });
                EditorViews.Add(new TestEditorView {  });
            }
        }

        private void LoadSkin(XmlSkinInfo skinInfo)
        {
            skinInfo.LoadXmlSkin();
            SkinEditorInfoManager.LoasSkinInfo(skinInfo);
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
            if (HasPendingChanges)
            {
                MessageBoxResult result = MessageBox.Show(string.Format("There are unsaved changes in the current skin{0}{0}Would you like to save changes now?", Environment.NewLine), "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return false;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Yes:
                        SkinSave();
                        return true;
                    default:
                        break;
                }
            }
            return true;
        }

        #region FileMenu

        public ICommand NewSkinCommand { get; internal set; }
        public ICommand OpenSkinCommand { get; internal set; }
        public ICommand SaveSkinCommand { get; internal set; }
        public ICommand SaveAsSkinCommand { get; internal set; }
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
                if (SavePendingChanges())
                {
                    ClearPendingChanges();
                    var newSkinDialog = new NewSkinDialog(Settings.GlobalSettings.LastSkinDirectory);
                    if (newSkinDialog.ShowDialog() == true)
                    {
                        Settings.GlobalSettings.LastSkinDirectory = newSkinDialog.SkinFolder;
                        CurrentSkinInfo = newSkinDialog.NewSkinInfo;
                        Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                        LoadSkin(CurrentSkinInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured creating skin:{0}{0}{1}", Environment.NewLine, ex.ToString()), "Error!");
            }
        }

        private void SkinOpen()
        {
            try
            {
                if (SavePendingChanges())
                {
                    ClearPendingChanges();
                    var skinInfoFile = FileSystemHelper.OpenFileDialog(Settings.GlobalSettings.LastSkinDirectory, "SkinInfo (SkinInfo.xml)|SkinInfo.xml");
                    if (!string.IsNullOrEmpty(skinInfoFile))
                    {
                        var skinInfo = SerializationHelper.Deserialize<XmlSkinInfo>(skinInfoFile);
                        if (skinInfo != null)
                        {
                            skinInfo.SkinFolderPath = System.IO.Path.GetDirectoryName(skinInfoFile);
                            Settings.GlobalSettings.LastSkinDirectory = Directory.GetParent(skinInfo.SkinFolderPath).FullName;
                            CurrentSkinInfo = skinInfo;
                            Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                            LoadSkin(CurrentSkinInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured opening skin:{0}{0}{1}", Environment.NewLine, ex.ToString()), "Error!");
            }
        }

        private void SkinSave()
        {
            try
            {
                if (CurrentSkinInfo != null)
                {
                    CurrentSkinInfo.SaveSkin();
                    ClearPendingChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured saving skin:{0}{0}{1}", Environment.NewLine, ex.ToString()), "Error!");
            }
        }

        private void SkinSaveAs()
        {
            try
            {
                var newSkinDialog = new NewSkinDialog(Settings.GlobalSettings.LastSkinDirectory);
                newSkinDialog.IsNewSkin = false;
                newSkinDialog.SkinName = CurrentSkinInfo.SkinName;
                if (newSkinDialog.ShowDialog() == true)
                {

                    CurrentSkinInfo.SaveSkinAs(newSkinDialog.SkinName, newSkinDialog.SkinFolder);
                    CurrentSkinInfo = newSkinDialog.NewSkinInfo;
                    Settings.GlobalSettings.LastSkinDirectory = newSkinDialog.SkinFolder;
                    Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                    ClearPendingChanges();
                    LoadSkin(CurrentSkinInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An exception occured saving skin:{0}{0}{1}", Environment.NewLine, ex.ToString()), "Error!");
            }
        }

     

        private void SkinOpenRecent(string recentSkin)
        {
            if (!string.IsNullOrEmpty(recentSkin) && File.Exists(recentSkin))
            {
                if (CurrentSkinInfo == null || !CurrentSkinInfo.SkinInfoPath.Equals(recentSkin))
                {
                    if (SavePendingChanges())
                    {
                        ClearPendingChanges();
                        try
                        {
                            var skinInfo = SerializationHelper.Deserialize<XmlSkinInfo>(recentSkin);
                            if (skinInfo != null)
                            {
                                skinInfo.SkinFolderPath = System.IO.Path.GetDirectoryName(recentSkin);
                                CurrentSkinInfo = skinInfo;
                                Settings.GlobalSettings.AddToRecentList(CurrentSkinInfo.SkinInfoPath);
                                LoadSkin(CurrentSkinInfo);
                            }
                        }
                        catch (Exception ex)
                        {
                           MessageBox.Show(string.Format("An exception occured opening recent skin:{0}{0}{1}", Environment.NewLine, ex.ToString()), "Error!");
                        }
                    }
                }
            }
        }

        private void SkinEditorExit()
        {
            if (SavePendingChanges())
            {
                ClearPendingChanges();
                Close();
            }
        }


        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property)); 
            }
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
