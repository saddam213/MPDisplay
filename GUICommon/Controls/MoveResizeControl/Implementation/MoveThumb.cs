using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (IsSnapToGrid && GridSize != 0)
            {
                double snap = value % GridSize;
                snap = (snap <= GridSize / 2.0) ? snap *= -1 : GridSize - snap;
                return (int)(snap + value);
            }
            return (int)value;
        }
        

        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;

            if (designerItem != null)
            {
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);
                Canvas.SetLeft(designerItem,ResizeMoveCalculateSnap( left + e.HorizontalChange));
                Canvas.SetTop(designerItem,ResizeMoveCalculateSnap( top + e.VerticalChange));
            }
        }
    }
}
