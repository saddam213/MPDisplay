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
using Common.Helpers;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for SkinInfoEditorDialog.xaml
    /// </summary>
    public partial class SkinInfoEditorDialog : Window, INotifyPropertyChanged
    {
        private XmlSkinInfo _skinInfo;

        public SkinInfoEditorDialog(XmlSkinInfo info)
        {
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
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
