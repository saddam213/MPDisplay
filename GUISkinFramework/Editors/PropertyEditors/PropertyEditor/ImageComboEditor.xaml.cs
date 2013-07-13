using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls.PropertyGrid;
using MPDisplay.Common.Controls.PropertyGrid.Editors;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class ImageComboEditor : UserControl, ITypeEditor
    {
    
        private PropertyItem _Item;

        public ImageComboEditor()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ImageComboEditor),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnValueChanged)));

    
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as ImageComboEditor;
            string value = e.NewValue as string;
            if (_this.SelectedImage == null)
            {
              //  _this.SelectedImage = SkinInfo.Images.FirstOrDefault(i => i.XmlName.Equals(value, StringComparison.OrdinalIgnoreCase));
            }
        }

        private XmlSkinInfo _skinInfo;

        public XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
            set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        }
        

        public XmlImageFile SelectedImage
        {
            get { return (XmlImageFile)GetValue(SelectedImageProperty); }
            set { SetValue(SelectedImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImageProperty =
            DependencyProperty.Register("SelectedImage", typeof(XmlImageFile), typeof(ImageComboEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedImageChanged)));

        private static void OnSelectedImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as ImageComboEditor;
            var xmlImage = e.NewValue as XmlImageFile;
            if (xmlImage != null && !xmlImage.DisplayName.Equals(_this.Value))
            {
                _this.Value = xmlImage.DisplayName;
                _this._Item.Value = _this.Value;
            }
        }


        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _Item = propertyItem;
            Value = _Item.Value as string;
            SkinInfo = _Item.PropertyGrid.Tag as XmlSkinInfo;
            return this;
        }





        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion









      

   
        
    }


}
