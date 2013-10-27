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
using GUISkinFramework.Property;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    [XmlSkinType(typeof(XmlButton))]
    public partial class GUIButton : GUIControl
    {
        private string _label;
        private BitmapImage _image;


        public GUIButton()
        {
            InitializeComponent();
        }

        public XmlButton SkinXml
        {
            get { return BaseXml as XmlButton; }
        }

        public GUIActionCollection ActionCollection { get; set; }

        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyPropertyChanged("Label"); }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

 

        public override void CreateControl()
        {
            base.CreateControl();
            ActionCollection = new GUIActionCollection(SkinXml.Actions);
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.LabelText);
            RegisteredProperties.AddRange(PropertyRepository.GetRegisteredProperties(this, SkinXml.Image));
        }


        public override async void OnTouchUp()
        {
            await ActionCollection.ExecuteActions();
            base.OnTouchUp();
        }

        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.LabelText);
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.Image);
        }

        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.LabelText);
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.Image);
        }

        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();

            string text = await PropertyRepository.GetProperty<string>(SkinXml.LabelText);
            Label = !string.IsNullOrEmpty(text) ? text : await PropertyRepository.GetProperty<string>(SkinXml.DefaultLabelText);
            var img = await PropertyRepository.GetProperty<byte[]>(SkinXml.Image)
                 ?? await PropertyRepository.GetProperty<byte[]>(SkinXml.DefaultImage);
            Image = GUIImageManager.GetImage(img);
        }

        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Image = null;
            Label = string.Empty;
        }
     }
}
