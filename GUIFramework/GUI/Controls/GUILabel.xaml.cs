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
    [XmlSkinType(typeof(XmlLabel))]  
    public partial class GUILabel : GUIControl
    {
        private string _label;

        public GUILabel()
        {
            InitializeComponent(); 
        }

        public XmlLabel SkinXml
        {
            get { return BaseXml as XmlLabel; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyPropertyChanged("Label"); }
        }

        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.LabelText);
        }

     

        public override void RegisterInfoData()
        {
            base.RegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.LabelText);
        }

        public override void DeregisterInfoData()
        {
            base.DeregisterInfoData();
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.LabelText);
        }

        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();
            Label = await PropertyRepository.GetProperty<string>(SkinXml.LabelText);
        }

        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Label = string.Empty;
        }




       
    }
}
