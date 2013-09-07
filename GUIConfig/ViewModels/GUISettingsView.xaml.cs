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
using System.Drawing;

namespace GUIConfig.ViewModels
{
    /// <summary>
    /// Interaction logic for BasicSettingsView.xaml
    /// </summary>
    public partial class GUISettingsView : ViewModelBase
    {
        private Log Log = LoggingManager.GetLog(typeof(GUISettingsView));

        public GUISettingsView()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "MPDisplay"; }
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

        private Screen _selectedDisplay;

        public Screen SelectedDisplay
        {
            get { return _selectedDisplay; }
            set { _selectedDisplay = value; NotifyPropertyChanged("SelectedDisplay"); SetScreenSettings(DataObject as GUISettings); }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetScreenSettings(DataObject as GUISettings);
        }

        private void SetScreenSettings(GUISettings settings)
        {
            if (settings != null && !settings.CustomResolution)
            {
                settings.ScreenHeight = SelectedDisplay.Bounds.Height;
                settings.ScreenWidth = SelectedDisplay.Bounds.Width;
                settings.ScreenOffSetX = SelectedDisplay.Bounds.X;
                settings.ScreenOffSetY = SelectedDisplay.Bounds.Y;
            }
        }

        public override void SaveChanges()
        {
            base.SaveChanges();

        }

    


     
        

    }
}
