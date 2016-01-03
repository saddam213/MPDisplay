using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for GradientBrushEditor.xaml
    /// </summary>
    public partial class ImageBrushEditor : INotifyPropertyChanged
    {
        private string _imageName;
        private Stretch _imageStretch;

        public ImageBrushEditor()
        {
            InitializeComponent();
        }

        public delegate void ImageBrushChangedEvent(XmlImageBrush imageBrush);
        public event ImageBrushChangedEvent OnImageBrushChanged;

     
        public XmlImageBrush ImageBrush
        {
            get { return (XmlImageBrush)GetValue(ImageBrushProperty); }
            set { SetValue(ImageBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GradientBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageBrushProperty =
            DependencyProperty.Register("ImageBrush", typeof(XmlImageBrush), typeof(ImageBrushEditor)
            , new PropertyMetadata(null, (d, e) => ((ImageBrushEditor) d).OnBrushChanged(e.NewValue as XmlImageBrush)));

     


        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(ImageBrushEditor), new PropertyMetadata(null));



        private bool _isChangingImage;
        private void OnBrushChanged(XmlImageBrush image)
        {
            if (string.IsNullOrEmpty(image?.ImageName)) return;
            _isChangingImage = true;
            ImageName = image.ImageName;
            ImageStretch = image.ImageStretch;
            _isChangingImage = false;
        }

        private void UpdateDisplayBrush()
        {
            if (_isChangingImage || string.IsNullOrEmpty(ImageName)) return;
            if (OnImageBrushChanged == null) return;
            if (ImageBrush == null) return;
            ImageBrush.ImageName = ImageName;
            ImageBrush.ImageStretch = ImageStretch;
            OnImageBrushChanged(new XmlImageBrush
            {
                ImageName = ImageName,
                ImageStretch = ImageStretch
            });
        }


        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (_imageName == value) return;
                _imageName = value;
                NotifyPropertyChanged("ImageName");
                UpdateDisplayBrush();
            }
        }

        public Stretch ImageStretch
        {
            get { return _imageStretch; }
            set 
            {
                if (_imageStretch == value) return;
                _imageStretch = value;
                NotifyPropertyChanged("ImageStretch");
                UpdateDisplayBrush();
            }
        }

        private void ImageBrowse_Click(object sender, RoutedEventArgs e)
        {
            new EditorDialog(new ImagePicker { Width = 700, Height = 600, SkinInfo = SkinInfo }, false).ShowDialog();
        }

          public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
