using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Common.Helpers;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushTypeEditor.xaml
    /// </summary>
    public partial class BrushTypeEditor : INotifyPropertyChanged
    {
        private XmlBrush _selectedStyle;
        private XmlBrush _brush;
        private BrushType _currentBrushType = BrushType.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrushTypeEditor"/> class.
        /// </summary>
        public BrushTypeEditor()
        { 
            InitializeComponent();
        }



        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(BrushTypeEditor), new PropertyMetadata(new XmlSkinInfo()));

        
        

        /// <summary>
        /// Occurs when [on brush changed].
        /// </summary>
        /// <param name="brush">The brush.</param>
        public delegate void BrushChangedEvent(XmlBrush brush);
        public event BrushChangedEvent OnBrushChanged;

        ///// <summary>
        ///// Gets the styles.
        ///// </summary>
        //public ObservableCollection<XmlBrush> Styles
        //{
        //    get { return SkinInfo.Style.BrushStyles; }
        //}

        /// <summary>
        /// Gets the skin image folder.
        /// </summary>
        public string SkinImageFolder => SkinInfo != null ? SkinInfo.SkinImageFolder : string.Empty;

        /// <summary>
        /// Gets or sets the edit brush.
        /// </summary>
        public XmlBrush EditBrush
        {
            get { return (XmlBrush)GetValue(EditBrushProperty); }
            set { SetValue(EditBrushProperty, value); }
        }

        /// <summary>
        /// The edit brush property
        /// </summary>
        public static readonly DependencyProperty EditBrushProperty =
            DependencyProperty.Register("EditBrush", typeof(XmlBrush)
            , typeof(BrushTypeEditor), new PropertyMetadata(null, OnEditBrushChanged));

        /// <summary>
        /// Called when [edit brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEditBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var editor = d as BrushTypeEditor;
            var brush = (e.NewValue as XmlBrush).CreateCopy();
            if (editor == null) return;
            editor.SetBrushType(brush);
            editor.Brush = brush;
            if (brush != null)
            {
                editor.SelectedStyle = editor.SkinInfo.Style.BrushStyles.FirstOrDefault(s => s.StyleId == brush.StyleId);
            }
        }

        /// <summary>
        /// Gets or sets the selected style.
        /// </summary>
        public XmlBrush SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                _selectedStyle = value;
                if (_selectedStyle != null)
                {
                    Brush = _selectedStyle;
                    FireBrushChanged(Brush);
                }
                NotifyPropertyChanged("SelectedStyle");
            }
        }

        /// <summary>
        /// Gets or sets the brush.
        /// </summary>
        public XmlBrush Brush
        {
            get { return _brush; }
            set { _brush = value; NotifyPropertyChanged("Brush"); }
        }

        /// <summary>
        /// Gets or sets the type of the current brush.
        /// </summary>
        public BrushType CurrentBrushType
        {
            get { return _currentBrushType; }
            set
            {
                _currentBrushType = value;
                SetBrushType(_currentBrushType);
                NotifyPropertyChanged("CurrentBrushType");
            }
        }

        /// <summary>
        /// Sets the type of the brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void SetBrushType(XmlStyle brush)
        {
            BrushType brushType;

            if (!string.IsNullOrEmpty(brush?.StyleId))
            {
                brushType = BrushType.Style;
            }
            else if (brush is XmlColorBrush)
            {
                brushType = BrushType.Color;
            }
            else if (brush is XmlGradientBrush)
            {
                brushType = BrushType.Gradient;
            }
            else if (brush is XmlImageBrush)
            {
                brushType = BrushType.Image;
            }
            else
            {
                brushType = BrushType.None;
            }

            if (CurrentBrushType != brushType)
            {
                CurrentBrushType = brushType;
            }
        }

        /// <summary>
        /// Sets the type of the brush.
        /// </summary>
        /// <param name="currentBrushType">Type of the current brush.</param>
        public void SetBrushType(BrushType currentBrushType)
        {
            if (currentBrushType != BrushType.Style)
            {
                SelectedStyle = null;
            }

            switch (currentBrushType)
            {
                case BrushType.None:
                case BrushType.Style:
                    Brush = new XmlBrush();
                    break;
                case BrushType.Color:
                    Brush = new XmlColorBrush();
                    break;
                case BrushType.Gradient:
                    Brush = new XmlGradientBrush();
                    break;
                case BrushType.Image:
                    Brush = new XmlImageBrush();
                    break;
            }
            FireBrushChanged(Brush);
        }

        /// <summary>
        /// Fires the brush changed.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void FireBrushChanged(XmlBrush brush)
        {
            OnBrushChanged?.Invoke(brush);
        }

        /// <summary>
        /// XMLs the image brush_ brush changed.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void XmlImageBrush_BrushChanged(XmlImageBrush brush)
        {
            if (CurrentBrushType != BrushType.Image) return;

            Brush = brush;
            FireBrushChanged(brush);
        }

        /// <summary>
        /// XMLs the gradient brush_ brush changed.
        /// </summary>
        /// <param name="brush">The brush.</param>
        private void XmlGradientBrush_BrushChanged(XmlGradientBrush brush)
        {
            if (CurrentBrushType != BrushType.Gradient) return;

            Brush = brush;
            FireBrushChanged(brush);
        }

        /// <summary>
        /// XMLs the color brush_ brush changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void XmlColorBrush_BrushChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (CurrentBrushType != BrushType.Color) return;

            Brush = new XmlColorBrush
            {
                Color = e.NewValue.ToString(),
                StyleId = SelectedStyle == null ? string.Empty : SelectedStyle.StyleId
            };
            FireBrushChanged(Brush);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public enum BrushType
    { 
        None,
        Style,
        Color,
        Gradient,
        Image 
    }

    public enum GradientAngle
    {
        Vertical,
        Horizontal,
        Custom
    };
}
