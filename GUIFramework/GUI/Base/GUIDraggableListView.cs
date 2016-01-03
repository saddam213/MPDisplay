using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    [GUISkinElement(typeof(XmlControl))]
    public class GUIDraggableListView : GUIControl
    {
        protected ScrollViewer ScrollViewer;

        private const double Lowspeedlevel = 200.0;
        private const double Highspeedlevel = 1000.0;
        private const double Scrollduration = 1.0;
        private const double Scrollfactor = 0.3;
        protected bool IsMouseDoubleClick;
        protected bool IsMouseButtonDown;
        protected bool MayBeOutOfSync;
        private Point _initialOffset;
        private Point _mouseDownPoint;
        private Point _mouseMovePoint1;
        private Point _mouseMovePoint2;
        private Point _mouseMovePoint3;
        private DateTime _mouseDownTime;
        private DateTime _mouseMoveTime1;
        private DateTime _mouseMoveTime2;
        private DateTime _mouseMoveTime3;
        private Point _mouseCurrentPoint;
        protected bool IsScrolling;
        private Storyboard _storyboard;

        #region Constructor

        #endregion

        #region Control Base
        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Called when [window open].
        /// </summary>
        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
         }

        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
       }

        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
        }

        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
       }

        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Updates the info data.
        /// </summary>
        public override void UpdateInfoData()
        {
            base.UpdateInfoData();
        }

        // ReSharper disable once RedundantOverridenMember
        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Virtual method called when a list item is selected
        /// </summary>
        protected virtual void SelectListItem()
        {
        }
 
        /// <summary>
        /// Handles Mouse.PreviewMouseDown event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            // DEBUG
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseDown: mouse down point " + e.GetPosition(this));
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseDown:: mouse ClickCount: " + e.ClickCount);

            // Set IsMouseButtonDown to true
            IsMouseButtonDown = true;
            if (ScrollViewer != null)
            {
                // Check whether it is a mouse double click
                IsMouseDoubleClick = e.ClickCount > 1;

                // Set isScrolling to false
                // isScrolling is true when initialOffset is saved and mouse cursor
                // is updated for scrolling
                IsScrolling = false;

                if (_storyboard != null)
                {
                    // Pause any storyboard if still active
                    if (_storyboard.GetCurrentState(this) == ClockState.Active)
                    {
                        _storyboard.Pause(this);
                    }
                    // Save the current horizontal and vertical offset
                    var tempHorizontalOffset = ScrollViewer.HorizontalOffset;
                    var tempVerticalOffset = ScrollViewer.VerticalOffset;
                    // Clear out the value provided by the animation
                    BeginAnimation(ScrollViewerHorizontalOffsetProperty, null);
                    // Set the current horizontal offset back
                    ScrollViewer.ScrollToHorizontalOffset(tempHorizontalOffset);
                    // Clear out the value provided by the animation
                    BeginAnimation(ScrollViewerVerticalOffsetProperty, null);
                    // Set the current vertical offset back
                    ScrollViewer.ScrollToVerticalOffset(tempVerticalOffset);

                    _storyboard = null;
                }
            }
            base.OnPreviewMouseDown(e);
        }

        /// <summary>
        /// Handles Mouse.PreviewMouseMove event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            // DEBUG
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: mouse move point " + e.GetPosition(this));
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: IsMouseCaptured =" + this.IsMouseCaptured);
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: e.LeftButton =" + e.LeftButton);
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: this.IsMouseActualOver(e.GetPosition(this)) =" + this.IsMouseActualOver(e.GetPosition(this)));
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: isScrolling =" + isScrolling);

            // Scroll the ScrollViewer only when mouse button is Pressed and is over the control
            if (e.LeftButton == MouseButtonState.Pressed && ScrollViewer != null && IsMouseActualOver(e.GetPosition(this)))
            {
                // Get the current mouse position
                _mouseCurrentPoint = e.GetPosition(this);

                // The default: IsMouseCaptured is true
                if (!IsMouseCaptured)
                    CaptureMouse();

                if (!IsScrolling)
                {
                    // Set isScrolling to ture
                    // isScrolling is true when initialOffset is saved and mouse cursor
                    // is updated for scrolling
                    IsScrolling = true;

                    // Save the HorizontalOffset and VerticalOffset
                    _initialOffset.X = ScrollViewer.HorizontalOffset;
                    _initialOffset.Y = ScrollViewer.VerticalOffset;

                    // Initialize Point1 Point2, and Point3, and set mouseDownPoint
                    _mouseDownPoint = _mouseMovePoint1 = _mouseMovePoint2 = _mouseMovePoint3 = e.GetPosition(this);
                    _mouseDownTime = _mouseMoveTime1 = _mouseMoveTime2 = _mouseMoveTime3 = DateTime.Now;

                    // Update the cursor
                    if (ScrollViewer.ExtentWidth > ScrollViewer.ViewportWidth &&
                        ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight)
                        Cursor = Cursors.ScrollAll;
                    else if (ScrollViewer.ExtentWidth > ScrollViewer.ViewportWidth)
                        Cursor = Cursors.ScrollWE;
                    else if (ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight)
                        Cursor = Cursors.ScrollNS;
                    else
                        Cursor = Cursors.Arrow;
                }
                // Calculate the delta from mouseDownPoint
                var delta = new Point(_mouseDownPoint.X - _mouseCurrentPoint.X,
                    _mouseDownPoint.Y - _mouseCurrentPoint.Y);

                // If scrolling reaches either edge, save this as a new starting point
                if ((ScrollViewer.ScrollableHeight > 0 &&
                    ((Math.Abs(ScrollViewer.VerticalOffset) < 0.0000001 && delta.Y < 0) ||
                    (Math.Abs(ScrollViewer.VerticalOffset - ScrollViewer.ScrollableHeight) < 0.0000001 && delta.Y > 0))) ||
                    ScrollViewer.ScrollableWidth > 0 &&
                    ((Math.Abs(ScrollViewer.HorizontalOffset) < 0.0000001 && delta.X < 0) ||
                    (Math.Abs(ScrollViewer.HorizontalOffset - ScrollViewer.ScrollableWidth) < 0.0000001 && delta.X > 0)))
                {
                    // Save the HorizontalOffset and VerticalOffset
                    _initialOffset.X = ScrollViewer.HorizontalOffset;
                    _initialOffset.Y = ScrollViewer.VerticalOffset;

                    // Initialize Point1 Point2, and Point3, and set mouseDownPoint
                    _mouseDownPoint = _mouseMovePoint1 = _mouseMovePoint2 = _mouseMovePoint3 = e.GetPosition(this);
                    _mouseDownTime = _mouseMoveTime1 = _mouseMoveTime2 = _mouseMoveTime3 = DateTime.Now;
                }
                else
                {
                    // Scroll the ScrollViewer
                    ScrollViewer.ScrollToHorizontalOffset(_initialOffset.X + delta.X);
                    ScrollViewer.ScrollToVerticalOffset(_initialOffset.Y + delta.Y);
                }

                // Save the last three points of the mouse move
                // this is used to estimate the speed of the mouse move
                if (_mouseMoveTime1 == _mouseDownTime && _mouseMoveTime2 == _mouseDownTime && _mouseMoveTime3 == _mouseDownTime)
                {
                    _mouseMovePoint1 = _mouseCurrentPoint;
                    _mouseMoveTime1 = DateTime.Now;
                }
                else if (_mouseMoveTime2 == _mouseDownTime && _mouseMoveTime3 == _mouseDownTime)
                {
                    _mouseMovePoint2 = _mouseCurrentPoint;
                    _mouseMoveTime2 = DateTime.Now;
                }
                else if (_mouseMoveTime3 == _mouseDownTime)
                {
                    _mouseMovePoint3 = _mouseCurrentPoint;
                    _mouseMoveTime3 = DateTime.Now;
                }
                else
                {
                    _mouseMovePoint1 = _mouseMovePoint2;
                    _mouseMovePoint2 = _mouseMovePoint3;
                    _mouseMovePoint3 = _mouseCurrentPoint;
                    _mouseMoveTime1 = _mouseMoveTime2;
                    _mouseMoveTime2 = _mouseMoveTime3;
                    _mouseMoveTime3 = DateTime.Now;
                }

                // Sync selected Item(s)
                if (MayBeOutOfSync)
                {
                    SelectListItem();
                    MayBeOutOfSync = false;
                }
            }
            // Set isScrolling to false when mouse button is Pressed but is not over the control
            else if (e.LeftButton == MouseButtonState.Pressed && ScrollViewer != null && !IsMouseActualOver(e.GetPosition(this)))
            {
                // Set isScrolling to false and update the cursor
                Cursor = Cursors.Arrow;
                IsScrolling = false;
                // Release mouse capture when the mouse is not over the control
                // did this to disable scrolling while mouse is not over the control
                if (IsMouseCaptured)
                    ReleaseMouseCapture();
            }
            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Handles Mouse.PreviewMouseUp event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            // Set IsMouseButtonDown to false
            IsMouseButtonDown = false;

            if (ScrollViewer != null)
            {
                // Sync selected Item(s)
                if (MayBeOutOfSync)
                {
                    SelectListItem();
                    MayBeOutOfSync = false;
                }
            }

            if (ScrollViewer != null && IsScrolling)
            {
                // Set isScrolling to false and update the cursor
                Cursor = Cursors.Arrow;
                IsScrolling = false;
                if (IsMouseCaptured)
                    ReleaseMouseCapture();

                // Estimate scroll speed
                double speed1 = 0.0, speed2 = 0.0;

                if (_mouseMoveTime1 != _mouseDownTime && _mouseMoveTime2 != _mouseDownTime && _mouseMoveTime3 != _mouseDownTime)
                {
                    // Case 1: estimate speed based on Point1 and Point3
                    var mouseScrollTime = _mouseMoveTime3.Subtract(_mouseMoveTime1).TotalSeconds;
                    var mouseScrollDistance = Math.Sqrt(Math.Pow(_mouseMovePoint3.X - _mouseMovePoint1.X, 2.0) +
                                                           Math.Pow(_mouseMovePoint3.Y - _mouseMovePoint1.Y, 2.0));
                    if (Math.Abs(mouseScrollTime) > 0.0000001 && Math.Abs(mouseScrollDistance) > 0.0000001)
                        speed1 = mouseScrollDistance / mouseScrollTime;

                    // Case 2: estimate speed based on mouseDownPoint and Point3
                    mouseScrollTime = _mouseMoveTime3.Subtract(_mouseDownTime).TotalSeconds;
                    mouseScrollDistance = Math.Sqrt(Math.Pow(_mouseMovePoint3.X - _mouseDownPoint.X, 2.0) +
                        Math.Pow(_mouseMovePoint3.Y - _mouseDownPoint.Y, 2.0));

                    if (Math.Abs(mouseScrollTime) > 0.0000001 && Math.Abs(mouseScrollDistance) > 0.0000001)
                        speed2 = mouseScrollDistance / mouseScrollTime;

                    // Pick the larger of speed1 and speed2, which is more likely to be correct
                    // also, calculate moving direction and speed on X and Y axles
                    double speed;
                    double speedX;
                    double speedY;
                    Direction leftOrRight;
                    Direction upOrDown;
                    if (speed1 > speed2)
                    {
                        speed = speed1;
                        leftOrRight = _mouseMovePoint3.X > _mouseMovePoint1.X ? Direction.Left : Direction.Right;
                        upOrDown = _mouseMovePoint3.Y > _mouseMovePoint1.Y ? Direction.Up : Direction.Down;
                        speedX = Math.Abs(_mouseMovePoint3.X - _mouseMovePoint1.X) / _mouseMoveTime3.Subtract(_mouseMoveTime1).TotalSeconds;
                        speedY = Math.Abs(_mouseMovePoint3.Y - _mouseMovePoint1.Y) / _mouseMoveTime3.Subtract(_mouseMoveTime1).TotalSeconds;
                    }
                    else
                    {
                        speed = speed2;
                        leftOrRight = _mouseMovePoint3.X > _mouseDownPoint.X ? Direction.Left : Direction.Right;
                        upOrDown = _mouseMovePoint3.Y > _mouseDownPoint.Y ? Direction.Up : Direction.Down;
                        speedX = Math.Abs(_mouseMovePoint3.X - _mouseDownPoint.X) / _mouseMoveTime3.Subtract(_mouseDownTime).TotalSeconds;
                        speedY = Math.Abs(_mouseMovePoint3.Y - _mouseDownPoint.Y) / _mouseMoveTime3.Subtract(_mouseDownTime).TotalSeconds;
                    }

                    RegisterName("myGUIDraggableListView", this);

                    // If speed is higher than HIGHSPEEDLEVEL, scrolling will
                    // continue until it reaches either edge or a mouse is clicked
                    if (speed > Highspeedlevel)
                    {
                        double durationX, durationY;

                        // Calculate durationX and durationY
                        if (ScrollViewer.ScrollableWidth > 0 && Math.Abs(speedX) > 0.0000001)
                        {
                            if (leftOrRight == Direction.Right)
                            {
                                // time needed to move to right edge
                                durationX = (ScrollViewer.ScrollableWidth - ScrollViewer.HorizontalOffset) / speedX;
                            }
                            else
                            {
                                // time needed to move to left edge
                                durationX = ScrollViewer.HorizontalOffset / speedX;
                            }
                        }
                        else // Cannot scroll horizontally
                        {
                            durationX = 0.0;
                        }

                        if (ScrollViewer.ScrollableHeight > 0 && Math.Abs(speedY) > 0.0000001)
                        {
                            if (upOrDown == Direction.Down)
                            {
                                // time needed to move down to bottom
                                durationY = (ScrollViewer.ScrollableHeight - ScrollViewer.VerticalOffset) / speedY;
                            }
                            else
                            {
                                // time needed to move up to top
                                durationY = ScrollViewer.VerticalOffset / speedY;
                            }
                        }
                        else // Cannot scroll vertically
                        {
                            durationY = 0.0;
                        }

                        _storyboard = new Storyboard();

                        var horizontalScrollAnimation = new DoubleAnimation();
                        var verticalScrollAnimation = new DoubleAnimation();

                        horizontalScrollAnimation.From = ScrollViewer.HorizontalOffset;
                        verticalScrollAnimation.From = ScrollViewer.VerticalOffset;

                        if (leftOrRight == Direction.Right && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset + durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset + durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else if (leftOrRight == Direction.Right && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset + durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset - durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else if (leftOrRight == Direction.Left && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset - durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset + durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else //if (leftOrRight == Direction.Left && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset - durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset - durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }

                        Storyboard.SetTargetName(horizontalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(horizontalScrollAnimation, new PropertyPath(ScrollViewerHorizontalOffsetProperty));
                        Storyboard.SetTargetName(verticalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(verticalScrollAnimation, new PropertyPath(ScrollViewerVerticalOffsetProperty));
                        _storyboard.Children.Add(horizontalScrollAnimation);
                        _storyboard.Children.Add(verticalScrollAnimation);

                        _storyboard.Begin(this, true);
                    }
                    // If speed is higher than LOWSPEEDLEVEL, scrolling will
                    // continue for SCROLLDURATION (one second) or stop after a mouse is clicked
                    else if (speed > Lowspeedlevel)
                    {
                        _storyboard = new Storyboard();

                        var horizontalScrollAnimation = new DoubleAnimation();
                        var verticalScrollAnimation = new DoubleAnimation();

                        horizontalScrollAnimation.From = ScrollViewer.HorizontalOffset;
                        horizontalScrollAnimation.DecelerationRatio = 1.0;
                        verticalScrollAnimation.From = ScrollViewer.VerticalOffset;
                        verticalScrollAnimation.DecelerationRatio = 1.0;

                        if (leftOrRight == Direction.Right && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset + Scrollduration * speedX * Scrollfactor;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset + Scrollduration * speedY * Scrollfactor;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                        }
                        else if (leftOrRight == Direction.Right && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset + Scrollduration * speedX * Scrollfactor;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset - Scrollduration * speedY * Scrollfactor;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                        }
                        else if (leftOrRight == Direction.Left && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset - Scrollduration * speedX * Scrollfactor;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset + Scrollduration * speedY * Scrollfactor;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                        }
                        else //if (leftOrRight == Direction.Left && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = ScrollViewer.HorizontalOffset - Scrollduration * speedX * Scrollfactor;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                            verticalScrollAnimation.To = ScrollViewer.VerticalOffset - Scrollduration * speedY * Scrollfactor;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(Scrollduration)));
                        }

                        Storyboard.SetTargetName(horizontalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(horizontalScrollAnimation, new PropertyPath(ScrollViewerHorizontalOffsetProperty));
                        Storyboard.SetTargetName(verticalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(verticalScrollAnimation, new PropertyPath(ScrollViewerVerticalOffsetProperty));
                        _storyboard.Children.Add(horizontalScrollAnimation);
                        _storyboard.Children.Add(verticalScrollAnimation);

                        _storyboard.Begin(this, true);
                    }
                }
            }
            base.OnPreviewMouseUp(e);
        }
        #endregion

        #region Public & Internal Properties

        /// <summary>
        /// VerticalTotalPageKey DependencyPropertyKey
        /// </summary>
        internal static readonly DependencyPropertyKey VerticalTotalPageKey =
            DependencyProperty.RegisterReadOnly(
                    "VerticalTotalPage",
                    typeof(int),
                    typeof(GUIDraggableListView),
                    new FrameworkPropertyMetadata(0));

        /// <summary>
        /// VerticalTotalPageProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VerticalTotalPageProperty = VerticalTotalPageKey.DependencyProperty;

        /// <summary>
        /// Gets the VerticalTotalPage of the DraggableListView
        /// </summary>
        public int VerticalTotalPage
        {
            get { return (int)GetValue(VerticalTotalPageProperty); }
            private set { SetValue(VerticalTotalPageKey, value); }
        }

        /// <summary>
        /// VerticalCurrentPageKey DependencyPropertyKey
        /// </summary>
        internal static readonly DependencyPropertyKey VerticalCurrentPageKey =
            DependencyProperty.RegisterReadOnly(
                    "VerticalCurrentPage",
                    typeof(int),
                    typeof(GUIDraggableListView),
                    new FrameworkPropertyMetadata(0));

        /// <summary>
        /// VerticalCurrentPageProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VerticalCurrentPageProperty = VerticalCurrentPageKey.DependencyProperty;

        /// <summary>
        /// Gets the VerticalCurrentPage of the DraggableListView
        /// </summary>
        public int VerticalCurrentPage
        {
            get { return (int)GetValue(VerticalCurrentPageProperty); }
            private set { SetValue(VerticalCurrentPageKey, value); }
        }

        /// <summary>
        /// HorizontalTotalPageKey DependencyPropertyKey
        /// </summary>
        internal static readonly DependencyPropertyKey HorizontalTotalPageKey =
            DependencyProperty.RegisterReadOnly(
                    "HorizontalTotalPage",
                    typeof(int),
                    typeof(GUIDraggableListView),
                    new FrameworkPropertyMetadata(0));

        /// <summary>
        /// HorizontalTotalPageProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty HorizontalTotalPageProperty = HorizontalTotalPageKey.DependencyProperty;

        /// <summary>
        /// Gets the HorizontalTotalPage of the DraggableListView
        /// </summary>
        public int HorizontalTotalPage
        {
            get { return (int)GetValue(HorizontalTotalPageProperty); }
            private set { SetValue(HorizontalTotalPageKey, value); }
        }

        /// <summary>
        /// HorizontalCurrentPageKey DependencyPropertyKey
        /// </summary>
        internal static readonly DependencyPropertyKey HorizontalCurrentPageKey =
            DependencyProperty.RegisterReadOnly(
                    "HorizontalCurrentPage",
                    typeof(int),
                    typeof(GUIDraggableListView),
                    new FrameworkPropertyMetadata(0));

        /// <summary>
        /// HorizontalCurrentPageProperty DependencyProperty
        /// </summary>
        public static readonly DependencyProperty HorizontalCurrentPageProperty = HorizontalCurrentPageKey.DependencyProperty;

        /// <summary>
        /// Gets the HorizontalCurrentPage of the DraggableListView
        /// </summary>
        public int HorizontalCurrentPage
        {
            get { return (int)GetValue(HorizontalCurrentPageProperty); }
            private set { SetValue(HorizontalCurrentPageKey, value); }
        }

        /// <summary>
        /// ScrollViewerHorizontalOffsetProperty DependencyProperty
        /// </summary>
        internal static readonly DependencyProperty ScrollViewerHorizontalOffsetProperty =
            DependencyProperty.Register(
            "ScrollViewerHorizontalOffset",
            typeof(double),
            typeof(GUIDraggableListView),
            new FrameworkPropertyMetadata(HorizontalOffsetChanged));

        /// <summary>
        /// ScrollViewerVerticalOffsetProperty DependencyProperty
        /// </summary>
        internal static readonly DependencyProperty ScrollViewerVerticalOffsetProperty =
            DependencyProperty.Register(
            "ScrollViewerVerticalOffset",
            typeof(double),
            typeof(GUIDraggableListView),
            new FrameworkPropertyMetadata(VerticalOffsetChanged));

        #endregion Public & Internal Properties

        #region Private Functions

        /// <summary>
        /// Handles MouseDown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GUIList_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            // DEBUG
            //System.Diagnostics.Trace.WriteLine("draggableListView_MouseButtonDown: mouse ClickCount: " + e.ClickCount);

            if (ScrollViewer == null) return;

            // Sync selected Item(s)
            if (MayBeOutOfSync)
            {
                MayBeOutOfSync = false;
            }
            // Reset mouse double click to false
            IsMouseDoubleClick = false;
        }

        /// <summary>
        /// Update VerticalTotalPage, VerticalCurrentPage, HorizontalTotalPage, HorizontalCurrentPage
        /// when ScrollChanged event fires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GUIList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Check whether VerticalTotalPage changed
            var newValue = GetTotalPage(Orientation.Vertical);
            if (VerticalTotalPage != newValue)
                VerticalTotalPage = newValue;

            // Check whether VerticalCurrentPage changed
            newValue = GetCurrentPage(Orientation.Vertical);
            if (VerticalCurrentPage != newValue)
                VerticalCurrentPage = newValue;

            // Check whether HorizontalTotalPage changed
            newValue = GetTotalPage(Orientation.Horizontal);
            if (HorizontalTotalPage != newValue)
                HorizontalTotalPage = newValue;

            // Check whether HorizontalCurrentPage changed
            newValue = GetCurrentPage(Orientation.Horizontal);
            if (HorizontalCurrentPage != newValue)
                HorizontalCurrentPage = newValue;
        }

        /// <summary>
        /// Scroll to the new horizontal offset
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gl = (GUIDraggableListView)d;
            if (gl.ScrollViewer == null) return;

            if ((double)e.NewValue >= 0.0 && (double)e.NewValue <= gl.ScrollViewer.ScrollableWidth)
            {
                gl.ScrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
                // DEBUG
                //System.Diagnostics.Trace.WriteLine("HorizontalOffsetChanged: HorizontalOffset =" + (double)e.NewValue);
            }
        }

        /// <summary>
        /// Scroll to the new vertical offset
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void VerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gl = (GUIDraggableListView)d;
            if (gl.ScrollViewer == null) return;

            if ((double)e.NewValue >= 0.0 && (double)e.NewValue <= gl.ScrollViewer.ScrollableHeight)
            {
                gl.ScrollViewer.ScrollToVerticalOffset((double)e.NewValue);
                // DEBUG
                //System.Diagnostics.Trace.WriteLine("HorizontalOffsetChanged: VerticalOffset =" + (double)e.NewValue);
            }
        }

        /// <summary>
        /// Calculate the total pages
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private int GetTotalPage(Orientation direction)
        {
            int intVal;

            if (direction == Orientation.Vertical)
            {
                if (ScrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(ScrollViewer.ExtentHeight / ScrollViewer.ViewportHeight + 0.5));
                    }
                    catch (OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            else  // Orientation.Horizontal
            {
                if (ScrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(ScrollViewer.ExtentWidth / ScrollViewer.ViewportWidth + 0.5));
                    }
                    catch (OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            return intVal;
        }

        /// <summary>
        /// Calculate the current page
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private int GetCurrentPage(Orientation direction)
        {
            int intVal;

            if (direction == Orientation.Vertical)
            {
                if (ScrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(ScrollViewer.VerticalOffset / ScrollViewer.ViewportHeight + 0.5) + 1);
                    }
                    catch (OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            else  // Orientation.Horizontal
            {
                if (ScrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(ScrollViewer.HorizontalOffset / ScrollViewer.ViewportWidth + 0.5) + 1);
                    }
                    catch (OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            return intVal;
        }

        /// <summary>
        /// While mouse button is pressed, the default behavior is mouse capture set.
        /// Need this function to figure out whether mouse is actually over or not
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool IsMouseActualOver(Point p)
        {
            return p.X >= 0 && p.X < ActualWidth && p.Y >= 0 && p.Y < ActualHeight;
        }

        #endregion Private Functions

        internal enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

    }
}
