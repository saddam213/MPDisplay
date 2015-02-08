using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GUIFramework.Managers;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Utils;
using MPDisplay.Common.ExtensionMethods;

namespace GUIFramework.GUI
{
    [GUISkinElement(typeof(XmlControl))]
    public class GUIDraggableListView : GUIControl
    {
        protected ScrollViewer _scrollViewer;

        private const double LOWSPEEDLEVEL = 200.0;
        private const double HIGHSPEEDLEVEL = 1000.0;
        private const double SCROLLDURATION = 1.0;
        private const double SCROLLFACTOR = 0.3;
        protected bool isMouseDoubleClick = false;
        protected bool isMouseDown = false;
        protected bool mayBeOutOfSync = false;
        private Point initialOffset;
        private Point mouseDownPoint;
        private Point mouseMovePoint1;
        private Point mouseMovePoint2;
        private Point mouseMovePoint3;
        private DateTime mouseDownTime;
        private DateTime mouseMoveTime1;
        private DateTime mouseMoveTime2;
        private DateTime mouseMoveTime3;
        private Point mouseCurrentPoint;
        protected bool isScrolling;
        private Storyboard storyboard;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListBox3D"/> class.
        /// </summary>
        public GUIDraggableListView() : base()
        {

        }

        #endregion

        #region Control Base
        /// <summary>
        /// Called when [window open].
        /// </summary>
        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
         }

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
       }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
        }


        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
       }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public override void UpdateInfoData()
        {
            base.UpdateInfoData();
        }

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

            // Set isMouseDown to true
            isMouseDown = true;
            if (_scrollViewer != null)
            {
                // Check whether it is a mouse double click
                isMouseDoubleClick = (e.ClickCount > 1);

                // Set isScrolling to false
                // isScrolling is true when initialOffset is saved and mouse cursor
                // is updated for scrolling
                isScrolling = false;

                if (storyboard != null)
                {
                    // Pause any storyboard if still active
                    if (storyboard.GetCurrentState(this) == ClockState.Active)
                    {
                        storyboard.Pause(this);
                    }
                    // Save the current horizontal and vertical offset
                    double tempHorizontalOffset = _scrollViewer.HorizontalOffset;
                    double tempVerticalOffset = _scrollViewer.VerticalOffset;
                    // Clear out the value provided by the animation
                    BeginAnimation(GUIDraggableListView.ScrollViewerHorizontalOffsetProperty, null);
                    // Set the current horizontal offset back
                    _scrollViewer.ScrollToHorizontalOffset(tempHorizontalOffset);
                    // Clear out the value provided by the animation
                    BeginAnimation(GUIDraggableListView.ScrollViewerVerticalOffsetProperty, null);
                    // Set the current vertical offset back
                    _scrollViewer.ScrollToVerticalOffset(tempVerticalOffset);

                    storyboard = null;
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
            if (e.LeftButton == MouseButtonState.Pressed && _scrollViewer != null && this.IsMouseActualOver(e.GetPosition(this)))
            {
                // Get the current mouse position
                mouseCurrentPoint = e.GetPosition(this);

                // The default: IsMouseCaptured is true
                if (!(this.IsMouseCaptured))
                    this.CaptureMouse();

                if (!isScrolling)
                {
                    // Set isScrolling to ture
                    // isScrolling is true when initialOffset is saved and mouse cursor
                    // is updated for scrolling
                    isScrolling = true;

                    // Save the HorizontalOffset and VerticalOffset
                    initialOffset.X = _scrollViewer.HorizontalOffset;
                    initialOffset.Y = _scrollViewer.VerticalOffset;

                    // Initialize Point1 Point2, and Point3, and set mouseDownPoint
                    mouseDownPoint = mouseMovePoint1 = mouseMovePoint2 = mouseMovePoint3 = e.GetPosition(this);
                    mouseDownTime = mouseMoveTime1 = mouseMoveTime2 = mouseMoveTime3 = DateTime.Now;

                    // Update the cursor
                    if (_scrollViewer.ExtentWidth > _scrollViewer.ViewportWidth &&
                        _scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight)
                        this.Cursor = Cursors.ScrollAll;
                    else if (_scrollViewer.ExtentWidth > _scrollViewer.ViewportWidth)
                        this.Cursor = Cursors.ScrollWE;
                    else if (_scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight)
                        this.Cursor = Cursors.ScrollNS;
                    else
                        this.Cursor = Cursors.Arrow;
                }
                // Calculate the delta from mouseDownPoint
                Point delta = new Point(this.mouseDownPoint.X - mouseCurrentPoint.X,
                    this.mouseDownPoint.Y - mouseCurrentPoint.Y);

                // If scrolling reaches either edge, save this as a new starting point
                if ((_scrollViewer.ScrollableHeight > 0 &&
                    ((_scrollViewer.VerticalOffset == 0 && delta.Y < 0) ||
                    (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight && delta.Y > 0))) ||
                    _scrollViewer.ScrollableWidth > 0 &&
                    ((_scrollViewer.HorizontalOffset == 0 && delta.X < 0) ||
                    (_scrollViewer.HorizontalOffset == _scrollViewer.ScrollableWidth && delta.X > 0)))
                {
                    // Save the HorizontalOffset and VerticalOffset
                    initialOffset.X = _scrollViewer.HorizontalOffset;
                    initialOffset.Y = _scrollViewer.VerticalOffset;

                    // Initialize Point1 Point2, and Point3, and set mouseDownPoint
                    mouseDownPoint = mouseMovePoint1 = mouseMovePoint2 = mouseMovePoint3 = e.GetPosition(this);
                    mouseDownTime = mouseMoveTime1 = mouseMoveTime2 = mouseMoveTime3 = DateTime.Now;
                }
                else
                {
                    // Scroll the ScrollViewer
                    _scrollViewer.ScrollToHorizontalOffset(this.initialOffset.X + delta.X);
                    _scrollViewer.ScrollToVerticalOffset(this.initialOffset.Y + delta.Y);
                }

                // DEBUG
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: delta =" + delta);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ExtentHeight =" + listViewScrollViewer.ExtentHeight);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ViewportHeight =" + listViewScrollViewer.ViewportHeight);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ScrollableHeight =" + listViewScrollViewer.ScrollableHeight);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.VerticalOffset =" + listViewScrollViewer.VerticalOffset);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ExtentWidth =" + listViewScrollViewer.ExtentWidth);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ViewportWidth =" + listViewScrollViewer.ViewportWidth);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.ScrollableWidth =" + listViewScrollViewer.ScrollableWidth);
                //System.Diagnostics.Trace.WriteLine("OnPreviewMouseMove: listViewScrollViewer.HorizontalOffset =" + listViewScrollViewer.HorizontalOffset);

                // Save the last three points of the mouse move
                // this is used to estimate the speed of the mouse move
                if (mouseMoveTime1 == mouseDownTime && mouseMoveTime2 == mouseDownTime && mouseMoveTime3 == mouseDownTime)
                {
                    mouseMovePoint1 = mouseCurrentPoint;
                    mouseMoveTime1 = DateTime.Now;
                }
                else if (mouseMoveTime2 == mouseDownTime && mouseMoveTime3 == mouseDownTime)
                {
                    mouseMovePoint2 = mouseCurrentPoint;
                    mouseMoveTime2 = DateTime.Now;
                }
                else if (mouseMoveTime3 == mouseDownTime)
                {
                    mouseMovePoint3 = mouseCurrentPoint;
                    mouseMoveTime3 = DateTime.Now;
                }
                else
                {
                    mouseMovePoint1 = mouseMovePoint2;
                    mouseMovePoint2 = mouseMovePoint3;
                    mouseMovePoint3 = mouseCurrentPoint;
                    mouseMoveTime1 = mouseMoveTime2;
                    mouseMoveTime2 = mouseMoveTime3;
                    mouseMoveTime3 = DateTime.Now;
                }

                // Sync selected Item(s)
                if (mayBeOutOfSync)
                {
                    SelectListItem();
                    mayBeOutOfSync = false;
                }
            }
            // Set isScrolling to false when mouse button is Pressed but is not over the control
            else if (e.LeftButton == MouseButtonState.Pressed && _scrollViewer != null && !(this.IsMouseActualOver(e.GetPosition(this))))
            {
                // Set isScrolling to false and update the cursor
                this.Cursor = Cursors.Arrow;
                isScrolling = false;
                // Release mouse capture when the mouse is not over the control
                // did this to disable scrolling while mouse is not over the control
                if (this.IsMouseCaptured)
                    this.ReleaseMouseCapture();
            }
            base.OnPreviewMouseMove(e);
        }

        /// <summary>
        /// Handles Mouse.PreviewMouseUp event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            // DEBUG
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: mouse up point " + e.GetPosition(this));
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: isScrolling =" + isScrolling);
            //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: mouse ClickCount: " + e.ClickCount);

            // Set isMouseDown to false
            isMouseDown = false;

            if (_scrollViewer != null)
            {
                // Sync selected Item(s)
                if (mayBeOutOfSync)
                {
                    SelectListItem();
                    mayBeOutOfSync = false;
                }
            }

            if (_scrollViewer != null && isScrolling)
            {
                // Set isScrolling to false and update the cursor
                this.Cursor = Cursors.Arrow;
                isScrolling = false;
                if (this.IsMouseCaptured)
                    this.ReleaseMouseCapture();

                // Estimate scroll speed
                double speed = 0.0, speedX = 0.0, speedY = 0.0, speed1 = 0.0, speed2 = 0.0;
                double mouseScrollTime = 0.0;
                double mouseScrollDistance = 0.0;
                Direction leftOrRight, upOrDown;

                if (mouseMoveTime1 != mouseDownTime && mouseMoveTime2 != mouseDownTime && mouseMoveTime3 != mouseDownTime)
                {
                    // Case 1: estimate speed based on Point1 and Point3
                    mouseScrollTime = mouseMoveTime3.Subtract(mouseMoveTime1).TotalSeconds;
                    mouseScrollDistance = Math.Sqrt(Math.Pow(mouseMovePoint3.X - mouseMovePoint1.X, 2.0) +
                        Math.Pow(mouseMovePoint3.Y - mouseMovePoint1.Y, 2.0));
                    if (mouseScrollTime != 0 && mouseScrollDistance != 0)
                        speed1 = mouseScrollDistance / mouseScrollTime;

                    // Case 2: estimate speed based on mouseDownPoint and Point3
                    mouseScrollTime = mouseMoveTime3.Subtract(mouseDownTime).TotalSeconds;
                    mouseScrollDistance = Math.Sqrt(Math.Pow(mouseMovePoint3.X - mouseDownPoint.X, 2.0) +
                        Math.Pow(mouseMovePoint3.Y - mouseDownPoint.Y, 2.0));

                    if (mouseScrollTime != 0 && mouseScrollDistance != 0)
                        speed2 = mouseScrollDistance / mouseScrollTime;

                    // Pick the larger of speed1 and speed2, which is more likely to be correct
                    // also, calculate moving direction and speed on X and Y axles
                    if (speed1 > speed2)
                    {
                        speed = speed1;
                        leftOrRight = (mouseMovePoint3.X > mouseMovePoint1.X) ? Direction.Left : Direction.Right;
                        upOrDown = (mouseMovePoint3.Y > mouseMovePoint1.Y) ? Direction.Up : Direction.Down;
                        speedX = Math.Abs(mouseMovePoint3.X - mouseMovePoint1.X) / mouseMoveTime3.Subtract(mouseMoveTime1).TotalSeconds;
                        speedY = Math.Abs(mouseMovePoint3.Y - mouseMovePoint1.Y) / mouseMoveTime3.Subtract(mouseMoveTime1).TotalSeconds;
                    }
                    else
                    {
                        speed = speed2;
                        leftOrRight = (mouseMovePoint3.X > mouseDownPoint.X) ? Direction.Left : Direction.Right;
                        upOrDown = (mouseMovePoint3.Y > mouseDownPoint.Y) ? Direction.Up : Direction.Down;
                        speedX = Math.Abs(mouseMovePoint3.X - mouseDownPoint.X) / mouseMoveTime3.Subtract(mouseDownTime).TotalSeconds;
                        speedY = Math.Abs(mouseMovePoint3.Y - mouseDownPoint.Y) / mouseMoveTime3.Subtract(mouseDownTime).TotalSeconds;
                    }

                    this.RegisterName("myGUIDraggableListView", this);

                    // DEBUG
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: leftOrRight =" + leftOrRight);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: upOrDown =" + upOrDown);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: speedX =" + speedX);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: speedY =" + speedY);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: speed1 =" + speed1);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: speed2 =" + speed2);
                    //System.Diagnostics.Trace.WriteLine("OnPreviewMouseUp: speed =" + speed);

                    // If speed is higher than HIGHSPEEDLEVEL, scrolling will
                    // continue until it reaches either edge or a mouse is clicked
                    if (speed > HIGHSPEEDLEVEL)
                    {
                        double durationX = 0.0, durationY = 0.0;

                        // Calculate durationX and durationY
                        if (_scrollViewer.ScrollableWidth > 0 && speedX != 0)
                        {
                            if (leftOrRight == Direction.Right)
                            {
                                // time needed to move to right edge
                                durationX = (_scrollViewer.ScrollableWidth - _scrollViewer.HorizontalOffset) / speedX;
                            }
                            else
                            {
                                // time needed to move to left edge
                                durationX = _scrollViewer.HorizontalOffset / speedX;
                            }
                        }
                        else // Cannot scroll horizontally
                        {
                            durationX = 0.0;
                        }

                        if (_scrollViewer.ScrollableHeight > 0 && speedY != 0)
                        {
                            if (upOrDown == Direction.Down)
                            {
                                // time needed to move down to bottom
                                durationY = (_scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset) / speedY;
                            }
                            else
                            {
                                // time needed to move up to top
                                durationY = _scrollViewer.VerticalOffset / speedY;
                            }
                        }
                        else // Cannot scroll vertically
                        {
                            durationY = 0.0;
                        }

                        storyboard = new Storyboard();

                        DoubleAnimation horizontalScrollAnimation = new DoubleAnimation();
                        DoubleAnimation verticalScrollAnimation = new DoubleAnimation();

                        horizontalScrollAnimation.From = _scrollViewer.HorizontalOffset;
                        verticalScrollAnimation.From = _scrollViewer.VerticalOffset;

                        if (leftOrRight == Direction.Right && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset + durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset + durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else if (leftOrRight == Direction.Right && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset + durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset - durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else if (leftOrRight == Direction.Left && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset - durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset + durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }
                        else //if (leftOrRight == Direction.Left && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset - durationX * speedX;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationX)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset - durationY * speedY;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(durationY)));
                        }

                        Storyboard.SetTargetName(horizontalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(horizontalScrollAnimation, new PropertyPath(GUIDraggableListView.ScrollViewerHorizontalOffsetProperty));
                        Storyboard.SetTargetName(verticalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(verticalScrollAnimation, new PropertyPath(GUIDraggableListView.ScrollViewerVerticalOffsetProperty));
                        storyboard.Children.Add(horizontalScrollAnimation);
                        storyboard.Children.Add(verticalScrollAnimation);

                        storyboard.Begin(this, true);
                    }
                    // If speed is higher than LOWSPEEDLEVEL, scrolling will
                    // continue for SCROLLDURATION (one second) or stop after a mouse is clicked
                    else if (speed > LOWSPEEDLEVEL)
                    {
                        storyboard = new Storyboard();

                        DoubleAnimation horizontalScrollAnimation = new DoubleAnimation();
                        DoubleAnimation verticalScrollAnimation = new DoubleAnimation();

                        horizontalScrollAnimation.From = _scrollViewer.HorizontalOffset;
                        horizontalScrollAnimation.DecelerationRatio = 1.0;
                        verticalScrollAnimation.From = _scrollViewer.VerticalOffset;
                        verticalScrollAnimation.DecelerationRatio = 1.0;

                        if (leftOrRight == Direction.Right && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset + SCROLLDURATION * speedX * SCROLLFACTOR;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset + SCROLLDURATION * speedY * SCROLLFACTOR;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                        }
                        else if (leftOrRight == Direction.Right && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset + SCROLLDURATION * speedX * SCROLLFACTOR;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset - SCROLLDURATION * speedY * SCROLLFACTOR;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                        }
                        else if (leftOrRight == Direction.Left && upOrDown == Direction.Down)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset - SCROLLDURATION * speedX * SCROLLFACTOR;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset + SCROLLDURATION * speedY * SCROLLFACTOR;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                        }
                        else //if (leftOrRight == Direction.Left && upOrDown == Direction.Up)
                        {
                            horizontalScrollAnimation.To = _scrollViewer.HorizontalOffset - SCROLLDURATION * speedX * SCROLLFACTOR;
                            horizontalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                            verticalScrollAnimation.To = _scrollViewer.VerticalOffset - SCROLLDURATION * speedY * SCROLLFACTOR;
                            verticalScrollAnimation.Duration = new Duration(new TimeSpan(0, 0, Convert.ToInt32(SCROLLDURATION)));
                        }

                        Storyboard.SetTargetName(horizontalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(horizontalScrollAnimation, new PropertyPath(GUIDraggableListView.ScrollViewerHorizontalOffsetProperty));
                        Storyboard.SetTargetName(verticalScrollAnimation, "myGUIDraggableListView");
                        Storyboard.SetTargetProperty(verticalScrollAnimation, new PropertyPath(GUIDraggableListView.ScrollViewerVerticalOffsetProperty));
                        storyboard.Children.Add(horizontalScrollAnimation);
                        storyboard.Children.Add(verticalScrollAnimation);

                        storyboard.Begin(this, true);
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

            if (_scrollViewer != null)
            {
                // Sync selected Item(s)
                if (mayBeOutOfSync)
                {
                    mayBeOutOfSync = false;
                }
                // Reset mouse double click to false
                this.isMouseDoubleClick = false;
            }
        }

        /// <summary>
        /// Update VerticalTotalPage, VerticalCurrentPage, HorizontalTotalPage, HorizontalCurrentPage
        /// when ScrollChanged event fires
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GUIList_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            int newValue;

            // Check whether VerticalTotalPage changed
            newValue = GetTotalPage(Orientation.Vertical);
            if (this.VerticalTotalPage != newValue)
                this.VerticalTotalPage = newValue;

            // Check whether VerticalCurrentPage changed
            newValue = GetCurrentPage(Orientation.Vertical);
            if (this.VerticalCurrentPage != newValue)
                this.VerticalCurrentPage = newValue;

            // Check whether HorizontalTotalPage changed
            newValue = GetTotalPage(Orientation.Horizontal);
            if (this.HorizontalTotalPage != newValue)
                this.HorizontalTotalPage = newValue;

            // Check whether HorizontalCurrentPage changed
            newValue = GetCurrentPage(Orientation.Horizontal);
            if (this.HorizontalCurrentPage != newValue)
                this.HorizontalCurrentPage = newValue;
        }

        /// <summary>
        /// Scroll to the new horizontal offset
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GUIDraggableListView gl = (GUIDraggableListView)d;
            if (gl._scrollViewer != null)
            {
                if ((double)e.NewValue >= 0.0 && (double)e.NewValue <= gl._scrollViewer.ScrollableWidth)
                {
                    gl._scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
                    // DEBUG
                    //System.Diagnostics.Trace.WriteLine("HorizontalOffsetChanged: HorizontalOffset =" + (double)e.NewValue);
                }
            }
        }

        /// <summary>
        /// Scroll to the new vertical offset
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void VerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GUIDraggableListView gl = (GUIDraggableListView)d;
            if (gl._scrollViewer != null)
            {
                if ((double)e.NewValue >= 0.0 && (double)e.NewValue <= gl._scrollViewer.ScrollableHeight)
                {
                    gl._scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
                    // DEBUG
                    //System.Diagnostics.Trace.WriteLine("HorizontalOffsetChanged: VerticalOffset =" + (double)e.NewValue);
                }
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
                if (_scrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(_scrollViewer.ExtentHeight / _scrollViewer.ViewportHeight + 0.5));
                    }
                    catch (System.OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            else  // Orientation.Horizontal
            {
                if (_scrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(_scrollViewer.ExtentWidth / _scrollViewer.ViewportWidth + 0.5));
                    }
                    catch (System.OverflowException)
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
                if (_scrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(_scrollViewer.VerticalOffset / _scrollViewer.ViewportHeight + 0.5) + 1);
                    }
                    catch (System.OverflowException)
                    {
                        intVal = 0;
                    }
                else
                    intVal = 0;
            }
            else  // Orientation.Horizontal
            {
                if (_scrollViewer != null)
                    try
                    {
                        intVal = Convert.ToInt32(Math.Round(_scrollViewer.HorizontalOffset / _scrollViewer.ViewportWidth + 0.5) + 1);
                    }
                    catch (System.OverflowException)
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
            if (p.X >= 0 && p.X < this.ActualWidth && p.Y >= 0 && p.Y < this.ActualHeight)
                return true;
            else
                return false;
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
