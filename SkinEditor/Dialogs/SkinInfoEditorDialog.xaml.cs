using System.ComponentModel;
using System.Windows;
using Common.Helpers;
using GUISkinFramework.Skin;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for SkinInfoEditorDialog.xaml
    /// </summary>
    public partial class SkinInfoEditorDialog : INotifyPropertyChanged
    {
        private XmlSkinInfo _skinInfo;

        public SkinInfoEditorDialog(XmlSkinInfo info)
        {
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            InitializeComponent();
            SkinInfo = info.CreateCopy();
        }

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
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

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
