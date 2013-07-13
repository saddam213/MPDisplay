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
using MPDisplay.Common.Log;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlGroup))]  
    public partial class GUIGroup : GUIControl, IControlHost
    {
        private ObservableCollection<GUIControl> _controls = new ObservableCollection<GUIControl>();
       
        public GUIGroup()
        {
            InitializeComponent(); 
        }

        public XmlGroup SkinXml
        {
            get { return BaseXml as XmlGroup; }
        }

        public override void CreateControl()
        {
            base.CreateControl();
            CreateControls();
        }

        public ObservableCollection<GUIControl> Controls
        {
            get { return _controls; }
            set { _controls = value; NotifyPropertyChanged("Controls"); }
        }

        public void CreateControls()
        {
            foreach (var xmlControl in SkinXml.Controls)
            {
                if (Controls.Any(c => c.Id == xmlControl.Id))
                {
                    Log.Message(LogLevel.Warn, "Duplicate Control - Group '{0}' already contains a control with Id:{1}, Duplicate control '{2}' id cannot be added to group.", BaseXml.Name, xmlControl.Id, xmlControl.Name);
                    continue;
                }
                Controls.Add(GUIElementFactory.CreateControl(ParentId, xmlControl));
            }
        }

      
    }
}
