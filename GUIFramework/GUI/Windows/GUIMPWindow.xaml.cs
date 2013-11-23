using GUISkinFramework.Windows;

namespace GUIFramework.GUI.Windows
{
    /// <summary>
    /// Interaction logic for GUIMPWindow.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlMPWindow))]
    public partial class GUIMPWindow : GUIWindow
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIMPWindow"/> class.
        /// </summary>
        public GUIMPWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
