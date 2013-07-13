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
using GUISkinFramework.Property;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlProgressBar))]  
    public partial class GUIProgressBar : GUIControl, IPropertyControl
    {
        private int _progress;
     

        public GUIProgressBar()
        {
            InitializeComponent(); 
        }

        public XmlProgressBar SkinXml
        {
            get { return BaseXml as XmlProgressBar; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; NotifyPropertyChanged("Progress"); }
        }

        public List<XmlProperty> RegisteredProperties { get; set; }

        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.ProgressValue);
        }

        public void RegisterProperties()
        {
            PropertyRepository.RegisterPropertyMessage(SkinXml.ProgressValue, () => OnPropertyChanging());
            this.OnPropertyChanged();
        }

        public void DergisterProperties()
        {
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.ProgressValue);
        }

        public override async void OnPropertyChanged()
        {
            base.OnPropertyChanged();
            Progress = await PropertyRepository.GetProperty<int>(SkinXml.ProgressValue);
        }
   
        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Progress = 0;
        }
    }
}
