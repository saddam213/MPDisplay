using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class SkinSettingsView : ViewModelBase
    {
        private Log Log = LoggingManager.GetLog(typeof(SkinSettingsView));

        public SkinSettingsView()
        {
            InitializeComponent();
            LoadSkins();
        }

        public override string Title
        {
            get { return "Skin Settings"; }
        }

        public override void OnModelOpen()
        {
            base.OnModelOpen();
            Log.Message(LogLevel.Debug, "{0} ViewModel opened.", Title);
        }

        public override void OnModelClose()
        {
            base.OnModelClose();
            if (base.HasPendingChanges)
            {

            }
            Log.Message(LogLevel.Debug, "{0} ViewModel closed.", Title);
        }

  


        private void LoadSkins()
        {
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
                            skin.SkinFolderPath = System.IO.Path.GetDirectoryName(skinXml);
                            foreach (var option in skin.SkinOptions)
                            {
                                option.PreviewImage = skin.SkinImageFolder + option.PreviewImage;
                            }
                            _skins.Add(skin);
                        }

                    }
                }
            }
        }

        private ObservableCollection<SkinInfo> _skins = new ObservableCollection<SkinInfo>();

        public ObservableCollection<SkinInfo> Skins
        {
            get { return _skins; }
            set { _skins = value; }
        }

        private SkinInfo _selectedSkin;

        public SkinInfo SelectedSkin
        {
            get { return _selectedSkin; }
            set { _selectedSkin = value; NotifyPropertyChanged("SelectedSkin"); }
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            foreach (var skin in Skins)
            {
                SerializationHelper.Serialize<SkinInfo>(skin, skin.SkinInfoPath);
            }
        }
    }
}
