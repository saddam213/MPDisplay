using System.Windows.Media.Imaging;
using GUIFramework.Managers;
using GUIFramework.Repositories;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlButton))]
    public partial class GUIButton
    {
        #region Fields

        private string _label;
        private BitmapImage _image;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIButton"/> class.
        /// </summary>
        public GUIButton()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlButton SkinXml
        {
            get { return BaseXml as XmlButton; }
        }

        /// <summary>
        /// Gets or sets the action collection.
        /// </summary>
        public GUIActionCollection ActionCollection { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyPropertyChanged("Label"); }
        }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        } 

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            ActionCollection = new GUIActionCollection(SkinXml.Actions);
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.LabelText);
            RegisteredProperties.AddRange(PropertyRepository.GetRegisteredProperties(this, SkinXml.Image));
        }

        /// <summary>
        /// Called when touch up.
        /// </summary>
        public override async void OnTouchUp()
        {
            await ActionCollection.ExecuteActions();
            base.OnTouchUp();
        }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.LabelText);
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.Image);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.LabelText);
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.Image);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();

            string text = await PropertyRepository.GetProperty<string>(SkinXml.LabelText, null);
            Label = !string.IsNullOrEmpty(text) ? text : await PropertyRepository.GetProperty<string>(SkinXml.DefaultLabelText, null);
            var img = await PropertyRepository.GetProperty<byte[]>(SkinXml.Image, null)
                 ?? await PropertyRepository.GetProperty<byte[]>(SkinXml.DefaultImage, null);
            Image = GUIImageManager.GetImage(img);
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Image = null;
            Label = string.Empty;
        } 

        #endregion
     }
}
