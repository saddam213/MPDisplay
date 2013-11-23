using GUISkinFramework.Windows;

namespace GUIFramework.GUI.Windows
{
    /// <summary>
    /// Interaction logic for GUIMPDWindow.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPDWindow))]
    public partial class GUIMPDWindow : GUIWindow
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIMPDWindow"/> class.
        /// </summary>
        public GUIMPDWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
