using System.Windows.Media.Imaging;
using GUIFramework.Managers;
using GUISkinFramework.Controls;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIImage.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlImage))]
    public partial class GUIImage : GUIControl
    {
        #region Fields

        private BitmapImage _image = new BitmapImage();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIImage"/> class.
        /// </summary>
        public GUIImage()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlImage SkinXml
        {
            get { return BaseXml as XmlImage; }
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
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.Image);
        }


        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.Image);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
             PropertyRepository.DeregisterPropertyMessage(this, SkinXml.Image);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();

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
        }

        #endregion
    }
}
