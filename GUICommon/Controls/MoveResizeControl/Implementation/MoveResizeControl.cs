using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class MoveResizeControl : ContentControl
    {

        static MoveResizeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MoveResizeControl), new FrameworkPropertyMetadata(typeof(MoveResizeControl)));
        }

        public bool IsSnapToGrid
        {
            get { return (bool)GetValue(IsSnapToGridProperty); }
            set { SetValue(IsSnapToGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSnapToGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSnapToGridProperty =
            DependencyProperty.Register("IsSnapToGrid", typeof(bool), typeof(MoveResizeControl), new PropertyMetadata(false));



        public int GridSize
        {
            get { return (int)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(int), typeof(MoveResizeControl), new PropertyMetadata(0));



        public Brush ResizeHighlightColor
        {
            get { return (Brush)GetValue(ResizeHighlightColorProperty); }
            set { SetValue(ResizeHighlightColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResizeHighlightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeHighlightColorProperty =
            DependencyProperty.Register("ResizeHighlightColor", typeof(Brush), typeof(MoveResizeControl), new PropertyMetadata(new SolidColorBrush(Colors.LightBlue)));




        public Brush ResizeSizeInfoColor
        {
            get { return (Brush)GetValue(ResizeSizeInfoColorProperty); }
            set { SetValue(ResizeSizeInfoColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResizeSizeInfoColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeSizeInfoColorProperty =
            DependencyProperty.Register("ResizeSizeInfoColor", typeof(Brush), typeof(MoveResizeControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));



    }
}
