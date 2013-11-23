using GUISkinFramework.Windows;

namespace GUIFramework.GUI.Windows
{
    /// <summary>
    /// Interaction logic for GUIPlayerWindow.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlPlayerWindow))]
    public partial class GUIPlayerWindow : GUIWindow
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIPlayerWindow"/> class.
        /// </summary>
        public GUIPlayerWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
