using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MPDisplay.Common.Controls
{
    public class MoveThumb : Thumb
    {


        public bool IsSnapToGrid
        {
            get { return (bool)GetValue(IsSnapToGridProperty); }
            set { SetValue(IsSnapToGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSnapToGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSnapToGridProperty =
            DependencyProperty.Register("IsSnapToGrid", typeof(bool), typeof(MoveThumb), new PropertyMetadata(false));



        public int GridSize
        {
            get { return (int)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(int), typeof(MoveThumb), new PropertyMetadata(0));

        private int ResizeMoveCalculateSnap(double value)
        {
            if (!IsSnapToGrid || GridSize == 0) return (int) value;

            var snap = value % GridSize;
            snap = (snap <= GridSize / 2.0) ? snap*-1 : GridSize - snap;
            return (int)(snap + value);
        }
        

        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem == null) return;
            var left = Canvas.GetLeft(designerItem);
            var top = Canvas.GetTop(designerItem);
            Canvas.SetLeft(designerItem,ResizeMoveCalculateSnap( left + e.HorizontalChange));
            Canvas.SetTop(designerItem,ResizeMoveCalculateSnap( top + e.VerticalChange));
        }
    }
}
