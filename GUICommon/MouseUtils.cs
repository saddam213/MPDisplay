using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MPDisplay.Common
{
    public class MouseTouchDevice : TouchDevice
    {
        #region Class Members

        private static MouseTouchDevice _device;

        public Point Position { get; set; }

        #endregion

        #region Public Static Methods

        public static void RegisterEvents(FrameworkElement root)
        {
            root.PreviewMouseDown += MouseDown;
            root.PreviewMouseMove += MouseMove;
            root.PreviewMouseUp += MouseUp;
            root.LostMouseCapture += LostMouseCapture;
            root.MouseLeave += MouseLeave;
        }

        #endregion

        #region Private Static Methods

        private static void MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_device != null &&
                _device.IsActive)
            {
                _device.ReportUp();
                _device.Deactivate();
                _device = null;
            }
            _device = new MouseTouchDevice(e.MouseDevice.GetHashCode());
            _device.SetActiveSource(e.MouseDevice.ActiveSource);
            _device.Position = e.GetPosition(null);
            _device.Activate();
            _device.ReportDown();
        }

        private static void MouseMove(object sender, MouseEventArgs e)
        {
            if (_device == null || !_device.IsActive) return;

            _device.Position = e.GetPosition(null);
            _device.ReportMove();
        }

        private static void MouseUp(object sender, MouseButtonEventArgs e)
        {
            LostMouseCapture(sender, e);
        }

        static void LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (_device == null || !_device.IsActive) return;

            _device.Position = e.GetPosition(null);
            _device.ReportUp();
            _device.Deactivate();
            _device = null;
        }

        static void MouseLeave(object sender, MouseEventArgs e)
        {
            LostMouseCapture(sender, e);
        }

        #endregion

        #region Constructors

        public MouseTouchDevice(int deviceId) :
            base(deviceId)
        {
            Position = new Point();
        }

        #endregion

        #region Overridden methods

        public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
        {
            return new TouchPointCollection();
        }

        public override TouchPoint GetTouchPoint(IInputElement relativeTo)
        {
            var point = Position;
            if (relativeTo != null)
            {
                if (ActiveSource != null)
                    point = ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(Position);
            }

            var rect = new Rect(point, new Size(1, 1));

            return new TouchPoint(this, point, rect, TouchAction.Move);
        }

        #endregion
    }
}
