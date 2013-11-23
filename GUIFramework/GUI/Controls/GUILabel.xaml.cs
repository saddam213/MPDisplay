using GUIFramework.Managers;
using GUISkinFramework.Controls;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUILabel.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlLabel))]  
    public partial class GUILabel : GUIControl
    {
        #region Fields

        private string _label;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUILabel"/> class.
        /// </summary>
        public GUILabel()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlLabel SkinXml
        {
            get { return BaseXml as XmlLabel; }
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyPropertyChanged("Label"); }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.LabelText);
        }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.LabelText);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.LabelText);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();
            string text = await PropertyRepository.GetProperty<string>(SkinXml.LabelText);
            Label = !string.IsNullOrEmpty(text) ? text : await PropertyRepository.GetProperty<string>(SkinXml.DefaultLabelText);
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Label = string.Empty;
        }

        #endregion
    }
}
