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
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

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
    }
}
