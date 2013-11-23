using GUISkinFramework.Dialogs;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIMPDDialog.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPDDialog))]
    public partial class GUIMPDDialog : GUIDialog
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
