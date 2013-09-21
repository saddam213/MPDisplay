using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GUISkinFramework;
using SkinEditor.Helpers;
using System.IO;
using MPDisplay.Common.Settings;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for NewSkinDialog.xaml
    /// </summary>
    public partial class NewSkinDialog : Window, INotifyPropertyChanged
    {
        private string _skinName = "";
        private string _skinFolder = "";
        private int _skinWidth = 1280;
        private int _skinHeight = 720;
        private bool _isNewSkin = true;

        public NewSkinDialog(string lastDirectory)
        {
            Owner = App.Current.MainWindow;
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
            string newPath = System.IO.Path.Combine(SkinFolder, SkinName);
            if (FileSystemHelper.DirecoryExists(newPath))
            {
                MessageBox.Show(string.Format("Directory {0} already exists{1}please choose another directory", newPath, Environment.NewLine), "Error");
                SkinFolder = string.Empty;
                return;
            }

            if (!FileSystemHelper.CreateDirectory(newPath))
            {
                MessageBox.Show(string.Format("Failed to create {0}{1}please check that you have permission to acess this folder", newPath, Environment.NewLine), "Error");
                SkinFolder = string.Empty;
                return;
            }

            NewSkinInfo = new XmlSkinInfo
            {
                SkinName = this.SkinName,
                SkinFolderPath = newPath,
                SkinWidth = this.SkinWidth,
                SkinHeight = this.SkinHeight
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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
