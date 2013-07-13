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
    public partial class BasicSettingsView : ViewModelBase
    {
        private Log Log = LoggingManager.GetLog(typeof(BasicSettingsView));

        public BasicSettingsView()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Settings"; }
        }

        public override void OnModelOpen()
        {
            base.OnModelOpen();
        }

        public override void OnModelClose()
        {
            base.OnModelClose();
        }
    }
}
