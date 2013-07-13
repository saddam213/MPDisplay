using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GUISkinFramework.Windows;

namespace SkinEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
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
