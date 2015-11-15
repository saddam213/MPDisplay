using System.ComponentModel;
using System.Windows;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class ImageComboEditor : ITypeEditor
    {
    
        private PropertyItem _item;

        public ImageComboEditor()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ImageComboEditor),
                                                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as ImageComboEditor;
            if (_this != null && _this.SelectedImage == null)
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
            DependencyProperty.Register("SelectedImage", typeof(XmlImageFile), typeof(ImageComboEditor), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedImageChanged));

        private static void OnSelectedImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _this = d as ImageComboEditor;
            var xmlImage = e.NewValue as XmlImageFile;
            if (_this == null || (xmlImage == null || xmlImage.DisplayName.Equals(_this.Value))) return;

            _this.Value = xmlImage.DisplayName;
            _this._item.Value = _this.Value;
        }


        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _item = propertyItem;
            Value = _item.Value as string;
            SkinInfo = _item.PropertyGrid.Tag as XmlSkinInfo;
            return this;
        }





        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion









      

   
        
    }


}
