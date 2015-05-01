using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIMPDDialog.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPDDialog))]
    public partial class GUIMPDDialog
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIMPDDialog"/> class.
        /// </summary>
        public GUIMPDDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Propereties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlMPDDialog SkinXml
        {
            get { return BaseXml as XmlMPDDialog; }
        }

        #endregion
    }
}
