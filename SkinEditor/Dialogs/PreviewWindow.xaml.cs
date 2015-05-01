using GUISkinFramework.Skin;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow
    {
        public PreviewWindow(XmlWindow window)
        {
            InitializeComponent();
            
            DisplayWindow(window);
        }

        /// <summary>
        /// Displays the window.
        /// </summary>
        /// <param name="window">The window.</param>
        private void DisplayWindow(XmlWindow window)
        {
            //var pre = GUIFactory.CreateWindow(window);
            //if (pre != null)
            //{
            //    DisplaySurface.Child = pre;
            //}
        }
    }
}
