using GUISkinFramework.Controls;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlRectangle))]
    public partial class GUIRectangle : GUIControl
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
        public XmlRectangle SkinXml
        {
            get { return BaseXml as XmlRectangle; }
        }

        #endregion
    }
}
