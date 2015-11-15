using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIMPDialog.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPDialog))]  
    public partial class GUIMPDialog
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIMPDialog"/> class.
        /// </summary>
        public GUIMPDialog()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlMPDialog SkinXml => BaseXml as XmlMPDialog;

        #endregion
    }
}
