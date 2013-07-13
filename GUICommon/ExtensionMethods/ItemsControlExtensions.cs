using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace MPDisplay.Common.ExtensionMethods
{
    public static class ItemsControlExtensions
    {
        #region Attached Properties

        #region Animatable ScrollOffset

        public static readonly DependencyProperty AnimatableHorizontalOffsetProperty
            = DependencyProperty.RegisterAttached("AnimatableHorizontalOffset", typeof(double), typeof(ItemsControlExtensions)
            , new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalChanged)));

        public static double GetAnimatableHorizontalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(AnimatableHorizontalOffsetProperty);
        }

        public static void SetAnimatableHorizontalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(AnimatableHorizontalOffsetProperty, value);
        }

        /// <summary>
        /// Called when horizontal changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer viewer = d as ScrollViewer;
            viewer.ScrollToHorizontalOffset((double)e.NewValue);
        }


        // Using a DependencyProperty as the backing store for SearchValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimatableVerticalOffsetProperty =
            DependencyProperty.RegisterAttached("AnimatableVerticalOffset", typeof(double), typeof(ItemsControlExtensions),
                new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalChanged)));

        public static double GetAnimatableVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(AnimatableVerticalOffsetProperty);
        }

        public static void SetAnimatableVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(AnimatableVerticalOffsetProperty, value);
        }

        /// <summary>
        /// Called when horizontal changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer viewer = d as ScrollViewer;
            viewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        #endregion



        public static bool GetScrollItemIntoView(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollItemIntoViewProperty);
        }

        public static void SetScrollItemIntoView(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollItemIntoViewProperty, value);
        }

        // Using a DependencyProperty as the backing store for ScrollItemIntoView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollItemIntoViewProperty =
            DependencyProperty.RegisterAttached("ScrollItemIntoView", typeof(bool), typeof(ItemsControlExtensions), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnScrollItemIntoViewChanged)));

        private static void OnScrollItemIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox)
            {
                ListBox viewer = d as ListBox;
                if (viewer.IsSynchronizedWithCurrentItem == true)
                {
                    viewer.Items.CurrentChanged += (s, r) => viewer.Dispatcher.BeginInvoke((Action)delegate { viewer.ScrollIntoView(viewer.Items.CurrentItem); }, DispatcherPriority.Background); 
                }
                else
                {
                    viewer.SelectionChanged += (s, r) => viewer.Dispatcher.BeginInvoke((Action)delegate { viewer.ScrollIntoView(viewer.SelectedItem); }, DispatcherPriority.Background); 
                }
            }
        }

        


        #endregion

        #region Extension Methods

        public static Point? GetItemOffsetFromCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Find the container
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
            if (container != null)
            {
                // Find the ScrollContentPresenter
                ScrollContentPresenter presenter = container.GetAscendantByType<ScrollContentPresenter>();
                if (presenter != null)
                {
                    // Find the IScrollInfo
                    IScrollInfo scrollInfo = presenter;
                    if (presenter.CanContentScroll)
                    {
                        scrollInfo = presenter.Content as IScrollInfo
                            ?? VisualTreeExtensions.FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollInfo
                            ?? presenter;
                    }

                    // Compute the center point of the container relative to the scrollInfo
                    Size size = container.RenderSize;
                    Point center = container.TransformToAncestor((Visual)scrollInfo).Transform(new Point(size.Width / 2, size.Height / 2));
                    center.Y += scrollInfo.VerticalOffset;
                    center.X += scrollInfo.HorizontalOffset;

                    return new Point(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth)
                                   , CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
                }
            }
            return null;
        }

        private static double CenteringOffset(double center, double viewport, double extent)
        {
            var t =  (extent - center) - (viewport / 2);
            return Math.Min(extent, Math.Max(0, center - viewport / 2));
        }


        #endregion

    }
}
