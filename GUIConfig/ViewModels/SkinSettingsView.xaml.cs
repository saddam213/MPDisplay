using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using GUIConfig.Settings;
using MPDisplay.Common.Utils;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class SkinSettingsView
    {
        #region Fields

        private readonly Log _log = LoggingManager.GetLog(typeof(SkinSettingsView));
        private ObservableCollection<SkinInfo> _skins = new ObservableCollection<SkinInfo>();
        private SkinInfo _selectedSkin;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinSettingsView"/> class.
        /// </summary>
        public SkinSettingsView()
        {
            SkinEditorCommand = new RelayCommand(LaunchSkinEditor, CanLaunchSkinEditor);
            InitializeComponent();
            LoadSkins();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the skin editor command.
        /// </summary>
        public ICommand SkinEditorCommand { get; set; }

        /// <summary>
        /// Gets or sets the skins.
        /// </summary>
        public ObservableCollection<SkinInfo> Skins
        {
            get { return _skins; }
            set { _skins = value; }
        }

        /// <summary>
        /// Gets or sets the selected skin.
        /// </summary>
        public SkinInfo SelectedSkin
        {
            get { return _selectedSkin; }
            set { _selectedSkin = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public override string Title => "Skin Settings";

        #endregion 

        #region Methods

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public override void SaveChanges()
        {
            base.SaveChanges();
            foreach (var skin in Skins)
            {
                foreach (var option in skin.SkinOptions)
                {
                    option.PreviewImage = option.PreviewImage.Replace(skin.SkinImageFolder, "");
                }
                SerializationHelper.Serialize(skin, skin.SkinInfoPath);
            }
        }

        /// <summary>
        /// Determines whether this instance [can launch skin editor].
        /// </summary>
        /// <returns></returns>
        private static bool CanLaunchSkinEditor()
        {
            return File.Exists(RegistrySettings.SkinEditorExePath);
        }

        /// <summary>
        /// Launches the skin editor.
        /// </summary>
        private void LaunchSkinEditor()
        {
            _log.Message(LogLevel.Info, "Launching MPDisplay SkinEditor, EditorPath: {0}, SkinInfo: {1}", RegistrySettings.SkinEditorExePath, SelectedSkin.SkinInfoPath);
            Process.Start(RegistrySettings.SkinEditorExePath, SelectedSkin.SkinInfoPath);
        }

        /// <summary>
        /// Loads the skins.
        /// </summary>
        private void LoadSkins()
        {
            _log.Message(LogLevel.Info, "Loading skins...");
            if (Directory.Exists(RegistrySettings.MPDisplaySkinFolder))
            {
                _skins.Clear();
                var skinXmls = Directory.GetFiles(RegistrySettings.MPDisplaySkinFolder, "SkinInfo.xml", SearchOption.AllDirectories);
                if (!skinXmls.Any()) return;

                foreach (var skinXml in skinXmls)
                {
                    var skin = SerializationHelper.Deserialize<SkinInfo>(skinXml);
                    if (skin != null)
                    {
                        _log.Message(LogLevel.Info, "Sucessfully loaded SkinInfo.Xml, Skin: {0}", skin.SkinName);
                        skin.SkinFolderPath = Path.GetDirectoryName(skinXml);
                        foreach (var option in skin.SkinOptions)
                        {
                            option.PreviewImage = skin.SkinImageFolder + option.PreviewImage;
                        }
                        _skins.Add(skin);
                    }
                    else
                    {
                        _log.Message(LogLevel.Error, "Failed to load SkinInfo.Xml, File: {0}", skinXml);
                    }

                }
            }
            else
            {
                _log.Message(LogLevel.Error, "MPDisplay skin dorectory not found!. Directory: {0}", RegistrySettings.MPDisplaySkinFolder);
            }
        }

        #endregion
    }
}
