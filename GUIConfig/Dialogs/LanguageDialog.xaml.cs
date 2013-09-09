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
using GUIConfig.Settings.Language;

namespace GUIConfig.Dialogs
{
    /// <summary>
    /// Interaction logic for LanguageDialog.xaml
    /// </summary>
    public partial class LanguageDialog : Window, INotifyPropertyChanged
    {
        private string _selectedLanguage = "English";

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogLanguagePicker"/> class.
        /// </summary>
        public LanguageDialog()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>
         ///Gets the languages.
        ///</summary>
        public IEnumerable<string> Languages
        {
            get { return LanguageHelper.Languages; }
        }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        public string SelectedLanguage
        {
            get { return _selectedLanguage; }
            set { _selectedLanguage = value; }
        }

        /// <summary>
        /// Handles the Click event of the Button_OK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
      
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="info">The info.</param>
        public void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
