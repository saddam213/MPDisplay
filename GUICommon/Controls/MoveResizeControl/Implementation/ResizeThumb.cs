using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class ResizeThumb : Thumb
    {
        public bool IsSnapToGrid
        {
            get { return (bool)GetValue(IsSnapToGridProperty); }
            set { SetValue(IsSnapToGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSnapToGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSnapToGridProperty =
            DependencyProperty.Register("IsSnapToGrid", typeof(bool), typeof(ResizeThumb), new PropertyMetadata(false));



        public int GridSize
        {
            get { return (int)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GridSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(int), typeof(ResizeThumb), new PropertyMetadata(0));

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


        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;

            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height -= deltaVertical;
                        designerItem.Height = ResizeMoveCalculateSnap(designerItem.Height);
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(ResizeMoveCalculateSnap(e.VerticalChange), designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height -= deltaVertical;
                        designerItem.Height = ResizeMoveCalculateSnap(designerItem.Height);
                        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + deltaVertical);
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(ResizeMoveCalculateSnap(e.HorizontalChange), designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width -= deltaHorizontal;
                        designerItem.Width = ResizeMoveCalculateSnap(designerItem.Width);
                        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                      
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width -= deltaHorizontal;
                        designerItem.Width = ResizeMoveCalculateSnap(designerItem.Width);
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
