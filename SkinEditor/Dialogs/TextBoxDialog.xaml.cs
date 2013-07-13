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

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for TextBoxDialog.xaml
    /// </summary>
    public partial class TextBoxDialog : Window, INotifyPropertyChanged
    {
        private string _valueTitle;
        private string _value;

        public static string ShowDialog(string title, string valueTitle, string initialText = "")
        {
            var dialog = new TextBoxDialog { Title = title, ValueTitle = valueTitle, Value = initialText };
            return dialog.ShowDialog() == true ? dialog.Value : string.Empty;
        }

        public TextBoxDialog()
        {
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            InitializeComponent();
        }

        public string ValueTitle
        {
            get { return _valueTitle; }
            set { _valueTitle = value; NotifyPropertyChanged("ValueTitle"); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; NotifyPropertyChanged("Value"); }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
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
