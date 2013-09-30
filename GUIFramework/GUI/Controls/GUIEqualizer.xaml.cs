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
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using GUIFramework.Managers;
using System.Collections.ObjectModel;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlEqualizer))]  
    public partial class GUIEqualizer : GUIControl
    {
        public GUIEqualizer()
        {
            InitializeComponent(); 
        }

        public XmlEqualizer SkinXml
        {
            get { return BaseXml as XmlEqualizer; }
        }

        public int EQDataLength
        {
            get { return SkinXml != null ? SkinXml.BandCount : 0; }
        }

        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            GenericDataRepository.RegisterEQData(EQDataReceived);
        }

        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            GenericDataRepository.DegisterEQData(this);
        }

        private void EQDataReceived(byte[] data)
        {
            Dispatcher.BeginInvoke((Action)delegate { EqualizerCanvas.SetEQData(data); });
        }
    }
}
