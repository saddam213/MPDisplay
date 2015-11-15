using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    [StyleTypedProperty(Property = "BusyStyle", StyleTargetType = typeof(Control))]
    public class LoadingAdorner : Decorator
    {
        private readonly BackgroundVisualHost _busyHost = new BackgroundVisualHost();

        #region IsBusyIndicatorShowing Property
        /// <summary>
        /// Identifies the IsBusyIndicatorShowing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBusyIndicatorShowingProperty = DependencyProperty.Register(
            "IsBusyIndicatorShowing", 
            typeof(bool), 
            typeof(LoadingAdorner), 
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets if the BusyIndicator is being shown.
        /// </summary>
        public bool IsBusyIndicatorShowing
        {
            get { return (bool)GetValue(IsBusyIndicatorShowingProperty); }
            set { SetValue(IsBusyIndicatorShowingProperty, value); }
        }
        #endregion

        #region BusyStyle
        ///<summary>
        /// Identifies the <see cref="BusyStyle" /> property.
        /// </summary>
        public static readonly DependencyProperty BusyStyleProperty =
            DependencyProperty.Register(
            "BusyStyle",
            typeof(Style),
            typeof(LoadingAdorner),
            new FrameworkPropertyMetadata(OnBusyStyleChanged));

        /// <summary>
        /// Gets or sets the Style to apply to the Control that is displayed as the busy indication.
        /// </summary>
        public Style BusyStyle
        {
            get { return (Style)GetValue(BusyStyleProperty); }
            set { SetValue(BusyStyleProperty, value); }
        }

        static void OnBusyStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
   
            var bd = (LoadingAdorner)d;
            var nVal = (Style)e.NewValue;
            bd._busyHost.CreateContent = () => new Control { Style = nVal };
        }
        #endregion

        #region BusyHorizontalAlignment
        ///<summary>
        /// Identifies the <see cref="BusyHorizontalAlignment" /> property.
        /// </summary>
        public static readonly DependencyProperty BusyHorizontalAlignmentProperty = DependencyProperty.Register(
          "BusyHorizontalAlignment",
          typeof(HorizontalAlignment),
          typeof(LoadingAdorner),
          new FrameworkPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or sets the HorizontalAlignment to use to layout the control that contains the busy indicator control.
        /// </summary>
        public HorizontalAlignment BusyHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(BusyHorizontalAlignmentProperty); }
            set { SetValue(BusyHorizontalAlignmentProperty, value); }
        }
        #endregion

        #region BusyVerticalAlignment
        ///<summary>
        /// Identifies the <see cref="BusyVerticalAlignment" /> property.
        /// </summary>
        public static readonly DependencyProperty BusyVerticalAlignmentProperty = DependencyProperty.Register(
          "BusyVerticalAlignment",
          typeof(VerticalAlignment),
          typeof(LoadingAdorner),
          new FrameworkPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the the VerticalAlignment to use to layout the control that contains the busy indicator.
        /// </summary>
        public VerticalAlignment BusyVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(BusyVerticalAlignmentProperty); }
            set { SetValue(BusyVerticalAlignmentProperty, value); }
        }
        #endregion

        static LoadingAdorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LoadingAdorner),
                new FrameworkPropertyMetadata(typeof(LoadingAdorner)));
        }

        protected override int VisualChildrenCount => Child != null ? 2 : 1;

        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (Child != null)
                    yield return Child;

                yield return _busyHost;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (Child != null)
            {
                switch (index)
                {
                    case 0:
                        return Child;

                    case 1:
                        return _busyHost;
                }
            }
            else if (index == 0)
                return _busyHost;

            throw new IndexOutOfRangeException("index");
        }

      

        public LoadingAdorner()
        {
            AddLogicalChild(_busyHost);
            AddVisualChild(_busyHost);

            SetBinding(_busyHost, IsBusyIndicatorShowingProperty, BackgroundVisualHost.IsContentShowingProperty);
            SetBinding(_busyHost, BusyHorizontalAlignmentProperty, HorizontalAlignmentProperty);
            SetBinding(_busyHost, BusyVerticalAlignmentProperty, VerticalAlignmentProperty);
        }

        private void SetBinding(DependencyObject obj, DependencyProperty source, DependencyProperty target)
        {
            if (obj == null) return;
            var b = new Binding {Source = this, Path = new PropertyPath(source)};
            BindingOperations.SetBinding(obj, target, b);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var ret = new Size(0, 0);
            if (Child != null)
            {
                Child.Measure(constraint);
                ret = Child.DesiredSize;
            }
            
            _busyHost.Measure(constraint);

            return new Size(Math.Max(ret.Width, _busyHost.DesiredSize.Width), Math.Max(ret.Height, _busyHost.DesiredSize.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var ret = new Size(0, 0);
            if (Child != null)
            {
                Child.Arrange(new Rect(arrangeSize));
                ret = Child.RenderSize;
            }

            _busyHost.Arrange(new Rect(arrangeSize));

            return new Size(Math.Max(ret.Width, _busyHost.RenderSize.Width), Math.Max(ret.Height, _busyHost.RenderSize.Height));
        }
    }
}
