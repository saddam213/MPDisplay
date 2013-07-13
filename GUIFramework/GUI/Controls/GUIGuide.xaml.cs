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
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using GUIFramework.Managers;
using System.Collections.ObjectModel;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlGuide))]  
    public partial class GUIGuide : GUIControl
    {
        public GUIGuide()
        {
            InitializeComponent(); 
        }

        public XmlGuide SkinXml
        {
            get { return BaseXml as XmlGuide; }
        }
    }
}
