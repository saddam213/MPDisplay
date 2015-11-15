using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlRectangle))]
    public partial class GUIRectangle
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIRectangle"/> class.
        /// </summary>
        public GUIRectangle()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlRectangle SkinXml => BaseXml as XmlRectangle;

        #endregion
    }
}
