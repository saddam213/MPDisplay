using System;
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
            , new FrameworkPropertyMetadata(0.0, OnHorizontalChanged));

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
            var viewer = d as ScrollViewer;
            viewer?.ScrollToHorizontalOffset((double)e.NewValue);
        }


        // Using a DependencyProperty as the backing store for SearchValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimatableVerticalOffsetProperty =
            DependencyProperty.RegisterAttached("AnimatableVerticalOffset", typeof(double), typeof(ItemsControlExtensions), new FrameworkPropertyMetadata(0.0, OnVerticalChanged));

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
            var viewer = d as ScrollViewer;
            viewer?.ScrollToVerticalOffset((double)e.NewValue);
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
            DependencyProperty.RegisterAttached("ScrollItemIntoView", typeof(bool), typeof(ItemsControlExtensions), new FrameworkPropertyMetadata(false, OnScrollItemIntoViewChanged));

        private static void OnScrollItemIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox)) return;

            var viewer = (ListBox) d;
            if (viewer.IsSynchronizedWithCurrentItem == true)
            {
                viewer.Items.CurrentChanged += (s, r) => viewer.Dispatcher.BeginInvoke((Action)delegate { viewer.ScrollIntoView(viewer.Items.CurrentItem); }, DispatcherPriority.Background); 
            }
            else
            {
                viewer.SelectionChanged += (s, r) => viewer.Dispatcher.BeginInvoke((Action)delegate { viewer.ScrollIntoView(viewer.SelectedItem); }, DispatcherPriority.Background); 
            }
        }

        #endregion

        #region Extension Methods

        public static Point? GetItemOffsetFromCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Find the container
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;

            // Find the ScrollContentPresenter
            var presenter = container?.GetAscendantByType<ScrollContentPresenter>();
            if (presenter == null) return null;

            // Find the IScrollInfo
            IScrollInfo scrollInfo = presenter;
            if (presenter.CanContentScroll)
            {
                scrollInfo = presenter.Content as IScrollInfo ?? (presenter.Content as ItemsPresenter).FirstVisualChild() as IScrollInfo ?? presenter;
            }

            // Compute the center point of the container relative to the scrollInfo
            var size = container.RenderSize;
            var center = container.TransformToAncestor((Visual)scrollInfo).Transform(new Point(size.Width / 2, size.Height / 2));
            center.Y += scrollInfo.VerticalOffset;
            center.X += scrollInfo.HorizontalOffset;

            return new Point(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth) 
                , CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
        }

        private static double CenteringOffset(double center, double viewport, double extent)
        {
            return Math.Min(extent, Math.Max(0, center - viewport / 2));
        }

        #endregion

    }
}
