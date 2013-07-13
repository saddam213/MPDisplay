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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUISkinFramework.Windows;

namespace GUIFramework.GUI.Windows
{
    /// <summary>
    /// Interaction logic for GUIMPDWindow.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlMPWindow))]
    public partial class GUIMPWindow : GUIWindow
    {
        public GUIMPWindow()
        {
            InitializeComponent();
        }
    }
}
