using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class RoundedImage : Image
    {
        private RectangleGeometry _clipRect = new RectangleGeometry();
  
        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        public int CornerRadius
        {
            get { return (int)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(int), typeof(RoundedImage), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _clipRect.RadiusX = _clipRect.RadiusY = CornerRadius;
            _clipRect.Rect = new Rect(new Size( ActualWidth, ActualHeight));
            Clip = _clipRect;
            if (sizeInfo != null)
            {
                base.OnRenderSizeChanged(sizeInfo);        
            }
        }

        //TODO: This seems a bit shit,
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Source" || e.Property.Name == "CornerRadius" || e.Property.Name == "Stretch" || e.Property.Name == "Margin")
            {
                OnRenderSizeChanged(null);
            }
            base.OnPropertyChanged(e);
        }
      
    } 
}
