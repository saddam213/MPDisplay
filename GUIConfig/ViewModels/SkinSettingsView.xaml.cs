using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Common.Helpers;
using Common.Logging;
using Common.Settings;
using MPDisplay.Common.Utils;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class SkinSettingsView : ViewModelBase
    {
        #region Fields

        private Log Log = LoggingManager.GetLog(typeof(SkinSettingsView));
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
            set { _selectedSkin = value; NotifyPropertyChanged("SelectedSkin"); }
        }

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public override string Title
        {
            get { return "Skin Settings"; }
        }

        #endregion 

        #region Methods

        /// <summary>
        /// Called when model tab opens.
        /// </summary>
        public override void OnModelOpen()
        {
            base.OnModelOpen();
        }

        /// <summary>
        /// Called when model tab closes.
        /// </summary>
        public override void OnModelClose()
        {
            base.OnModelClose();
        }

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
                SerializationHelper.Serialize<SkinInfo>(skin, skin.SkinInfoPath);
            }
        }

        /// <summary>
        /// Determines whether this instance [can launch skin editor].
        /// </summary>
        /// <returns></returns>
        private bool CanLaunchSkinEditor()
        {
            return File.Exists(RegistrySettings.SkinEditorExePath);
        }

        /// <summary>
        /// Launches the skin editor.
        /// </summary>
        private void LaunchSkinEditor()
        {
            Log.Message(LogLevel.Info, "Launching MPDisplay SkinEditor, EditorPath: {0}, SkinInfo: {1}", RegistrySettings.SkinEditorExePath, SelectedSkin.SkinInfoPath);
            Process.Start(RegistrySettings.SkinEditorExePath, SelectedSkin.SkinInfoPath);
        }

        /// <summary>
        /// Loads the skins.
        /// </summary>
        private void LoadSkins()
        {
            Log.Message(LogLevel.Info, "Loading skins...");
            if (Directory.Exists(RegistrySettings.MPDisplaySkinFolder))
            {
                _skins.Clear();
                var skinXmls = Directory.GetFiles(RegistrySettings.MPDisplaySkinFolder, "SkinInfo.xml", SearchOption.AllDirectories);
                if (skinXmls.Any())
                {
                    foreach (var skinXml in skinXmls)
                    {
                        var skin = SerializationHelper.Deserialize<SkinInfo>(skinXml);
                        if (skin != null)
                        {
                            Log.Message(LogLevel.Info, "Sucessfully loaded SkinInfo.Xml, Skin: {0}", skin.SkinName);
                            skin.SkinFolderPath = System.IO.Path.GetDirectoryName(skinXml);
                            foreach (var option in skin.SkinOptions)
                            {
                                option.PreviewImage = skin.SkinImageFolder + option.PreviewImage;
                            }
                            _skins.Add(skin);
                        }
                        else
                        {
                            Log.Message(LogLevel.Error, "Failed to load SkinInfo.Xml, File: {0}", skinXml);
                        }

                    }
                }
            }
            else
            {
                Log.Message(LogLevel.Error, "MPDisplay skin dorectory not found!. Directory: {0}", RegistrySettings.MPDisplaySkinFolder);
            }
        }

        #endregion
    }
}
