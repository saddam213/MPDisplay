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
    [XmlSkinType(typeof(XmlImage))]
    public partial class GUIImage : GUIControl
    {
        private BitmapImage _image = new BitmapImage();
        public GUIImage()
        {
            InitializeComponent(); 
        }

        public XmlImage SkinXml
        {
            get { return BaseXml as XmlImage; }
        }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; NotifyPropertyChanged("Image"); }
        }

    

        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.Image);
        }


        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.Image);
        }

        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
             PropertyRepository.DeregisterPropertyMessage(this, SkinXml.Image);
        }

        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();

            var img = await PropertyRepository.GetProperty<byte[]>(SkinXml.Image) 
                   ?? await PropertyRepository.GetProperty<byte[]>(SkinXml.DefaultImage);
            Image = GUIImageManager.GetImageFromBytes(img);
        }

        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Image = null;
        }


    }
}
