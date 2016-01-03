using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using GUISkinFramework.Skin;
using SkinEditor.Helpers;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for NewSkinDialog.xaml
    /// </summary>
    public partial class NewSkinDialog : INotifyPropertyChanged
    {
        private string _skinName = "";
        private string _skinFolder = "";
        private int _skinWidth = 1280;
        private int _skinHeight = 720;
        private bool _isNewSkin = true;

        public NewSkinDialog(string lastDirectory)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            SkinFolder = lastDirectory;
        }

        public string SkinName
        {
            get { return _skinName; }
            set { _skinName = value; NotifyPropertyChanged("SkinName"); }
        }

        public string SkinFolder
        {
            get { return _skinFolder; }
            set { _skinFolder = value; NotifyPropertyChanged("SkinFolder"); }
        }

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

    

        public bool IsNewSkin
        {
            get { return _isNewSkin; }
            set { _isNewSkin = value; NotifyPropertyChanged("IsNewSkin"); }
        }
        

        public XmlSkinInfo NewSkinInfo {get;set;}

        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            SkinFolder = FileSystemHelper.OpenFolderDialog("");
        }
  
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            var newPath = Path.Combine(SkinFolder, SkinName);
            if (FileSystemHelper.DirecoryExists(newPath))
            {
                MessageBox.Show(
                    $"Directory {newPath} already exists{Environment.NewLine}please choose another directory", "Error");
                SkinFolder = string.Empty;
                return;
            }

            if (!FileSystemHelper.CreateDirectory(newPath))
            {
                MessageBox.Show(
                    $"Failed to create {newPath}{Environment.NewLine}please check that you have permission to acess this folder", "Error");
                SkinFolder = string.Empty;
                return;
            }

            NewSkinInfo = new XmlSkinInfo
            {
                SkinName = SkinName,
                SkinFolderPath = newPath,
                SkinWidth = SkinWidth,
                SkinHeight = SkinHeight
            };

            if (IsNewSkin)
            {
                NewSkinInfo.CreateSkin();
            }
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
