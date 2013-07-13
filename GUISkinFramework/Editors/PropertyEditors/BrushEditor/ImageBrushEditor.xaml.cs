using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using GUISkinFramework.Editor.PropertyEditors;
using GUISkinFramework.Editor.PropertyEditors.PropertyEditor;
using GUISkinFramework.PropertyEditors;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors.PropertyEditors
{
    /// <summary>
    /// Interaction logic for GradientBrushEditor.xaml
    /// </summary>
    public partial class ImageBrushEditor : UserControl, INotifyPropertyChanged
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
            , new PropertyMetadata(null, (d, e) => (d as ImageBrushEditor).OnBrushChanged(e.NewValue as XmlImageBrush)));

     


        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(ImageBrushEditor), new PropertyMetadata(null));



        private bool _isChangingImage = false;
        private void OnBrushChanged(XmlImageBrush image)
        {
            if (image != null && !string.IsNullOrEmpty(image.ImageName))
            {
                _isChangingImage = true;
                ImageName = image.ImageName;
                ImageStretch = image.ImageStretch;
                _isChangingImage = false;
            }
        }

        private void UpdateDisplayBrush()
        {
            if (_isChangingImage == false && !string.IsNullOrEmpty(this.ImageName))
            {
                if (OnImageBrushChanged != null)
                {
                    if (ImageBrush != null)
                    {
                        ImageBrush.ImageName = this.ImageName;
                        ImageBrush.ImageStretch = this.ImageStretch;
                        OnImageBrushChanged(new XmlImageBrush
                        {
                            ImageName = this.ImageName,
                            ImageStretch = this.ImageStretch
                        });
                    }
                }
            }
        }

       
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (_imageName != value)
                {
                    _imageName = value;
                    NotifyPropertyChanged("ImageName");
                    UpdateDisplayBrush();
                }
            }
        }

        public Stretch ImageStretch
        {
            get { return _imageStretch; }
            set 
            {
                if (_imageStretch != value)
                {
                    _imageStretch = value;
                    NotifyPropertyChanged("ImageStretch");
                    UpdateDisplayBrush();
                }
            }
        }

        private void ImageBrowse_Click(object sender, RoutedEventArgs e)
        {
            new EditorDialog(new ImagePicker() { Width = 700, Height = 600, SkinInfo = SkinInfo }).ShowDialog();
        }

      





      






        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

      
    }
}
