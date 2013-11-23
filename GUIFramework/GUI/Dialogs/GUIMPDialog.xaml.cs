using GUISkinFramework.Dialogs;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIMPDialog.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPDialog))]  
    public partial class GUIMPDialog : GUIDialog
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
        public XmlMPDialog SkinXml
        {
            get { return BaseXml as XmlMPDialog; }
        }

        #endregion
    }
}
