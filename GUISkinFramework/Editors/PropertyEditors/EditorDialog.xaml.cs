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

namespace GUISkinFramework.PropertyEditors
{
    /// <summary>
    /// Interaction logic for EditorDialog.xaml
    /// </summary>
    public partial class EditorDialog : Window, INotifyPropertyChanged
    {
        private UserControl _editorContent;
        private bool _hasCancelButton = true;

        public EditorDialog(UserControl editorContent, bool canCancel)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            EditorContent = editorContent;
            HasCancelButton = canCancel;
        }

        public UserControl EditorContent
        {
            get { return _editorContent; }
            set { _editorContent = value; NotifyPropertyChanged("EditorContent"); }
        }

        public bool HasCancelButton
        {
            get { return _hasCancelButton; }
            set { _hasCancelButton = value; NotifyPropertyChanged("HasCancelButton"); }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
