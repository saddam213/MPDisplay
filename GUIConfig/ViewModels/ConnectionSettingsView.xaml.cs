using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MPDisplay.Common.Settings;
using System.ComponentModel;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsView.xaml
    /// </summary>
    public partial class ConnectionSettingsView : UserControl, INotifyPropertyChanged
    {
        public ConnectionSettingsView()
        {
            InitializeComponent();
        }

        public ConnectionSettings ConnectionSettings
        {
            get { return (ConnectionSettings)GetValue(ConnectionSettingsProperty); }
            set { SetValue(ConnectionSettingsProperty, value); NotifyPropertyChanged("ConnectionSettings"); }
        }

        // Using a DependencyProperty as the backing store for ConnectionSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectionSettingsProperty =
            DependencyProperty.Register("ConnectionSettings", typeof(ConnectionSettings), typeof(ConnectionSettingsView));//, new PropertyMetadata(new ConnectionSettings));

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    
        
    }
}
