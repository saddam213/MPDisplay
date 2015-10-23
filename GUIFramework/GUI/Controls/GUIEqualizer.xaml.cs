using System;
using GUIFramework.Repositories;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIEqualizer.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlEqualizer))]  
    public partial class GUIEqualizer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIEqualizer"/> class.
        /// </summary>
        public GUIEqualizer()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlEqualizer SkinXml
        {
            get { return BaseXml as XmlEqualizer; }
        }

        /// <summary>
        /// Gets the length of the eq data.
        /// </summary>
        public int EQDataLength
        {
            get { return SkinXml != null ? SkinXml.BandCount : 0; }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            GenericDataRepository.RegisterEQData(EQDataReceived);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            GenericDataRepository.DegisterEQData(this);
        }

        /// <summary>
        /// Eqs the data received.
        /// </summary>
        /// <param name="data">The data.</param>
        private void EQDataReceived(byte[] data)
        {
            Dispatcher.BeginInvoke((Action)delegate { EqualizerCanvas.SetEQData(data); });
        }

        #endregion
    }
}
