using System.ComponentModel;
using System.Windows;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for SkinEditorOptions.xaml
    /// </summary>
    public partial class SkinEditorOptionsDialog : INotifyPropertyChanged
    {
        private bool  _autoLoadLastSkin;

        public SkinEditorOptionsDialog(GlobalSettings currentSettings)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            AutoLoadLastSkin = currentSettings.AutoLoadLastSkin;
        }

        public bool AutoLoadLastSkin
        {
            get { return _autoLoadLastSkin; }
            set { _autoLoadLastSkin = value; NotifyPropertyChanged("AutoLoadLastSkin"); }
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
