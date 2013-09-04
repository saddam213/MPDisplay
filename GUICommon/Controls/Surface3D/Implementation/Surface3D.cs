using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MPDisplay.Common.Controls
{
    public class Surface3D : ContentControl, INotifyPropertyChanged
    {
        #region private fields

        /// <summary>
        /// The 3D scene used to render the control.
        /// </summary>
        public Viewport3D _viewport;

        /// <summary>
        /// The camera, which determines the view of the scene.
        /// </summary>
        private PerspectiveCamera _camera;

        /// <summary>
        /// The ContentPresenter used to display content on the front.
        /// </summary>
        public Border _content;

      //  public Viewbox _frontViewbox;

     

        /// <summary>
        /// The model used to display content on the front.
        /// </summary>
        protected Viewport2DVisual3D _frontModel;

        /// <summary>
        /// The model used to display content on the back.
        /// </summary>
        protected Viewport2DVisual3D _backModel;

        /// <summary>
        /// The 3D scale transform, used to keep the content at the right size.
        /// </summary>
        private ScaleTransform3D _scale;

        /// <summary>
        /// The 3D rotation transform, used to rotate the content in 3D.
        /// </summary>
        private RotateTransform3D _rotate;

        /// <summary>
        /// The quaternion component of the 3D rotation.
        /// </summary>
        private QuaternionRotation3D _quaternion;

        /// <summary>
        /// A 2D rotation for the content visuals when they are shown in 2D, so that they match the 3D rotation.
        /// </summary>
        private RotateTransform _fixedTransform;

        /// <summary>
        /// A set of lights which add shading when the object is rotated.
        /// </summary>
        private ModelVisual3D _directionalLights;

        /// <summary>
        /// A set of lights which don't shade the objects.
        /// </summary>
        private ModelVisual3D _ambientLights;

        /// <summary>
        /// Whether the last rotation change had the visual showing its back side.
        /// </summary>
        private bool _lastIsBackShowing;
     
        /// <summary>
        /// The content bounds.
        /// </summary>
        private Rect _bounds;

        #endregion

        #region Static lookups

        /// <summary>
        /// Optimization, only create the axis once.
        /// </summary>
        static private readonly Vector3D _axisX = new Vector3D(1, 0, 0);

        /// <summary>
        /// Optimization, only create the axis once.
        /// </summary>
        static private readonly Vector3D _axisY = new Vector3D(0, 1, 0);

        /// <summary>
        /// Optimization, only create the axis once.
        /// </summary>
        static private readonly Vector3D _axisZ = new Vector3D(0, 0, 1);

        #endregion

        #region Setup

        /// <summary>
        /// Initializes static members of the <see cref="Surface3D"/> class.
        /// </summary>
        static Surface3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Surface3D), new FrameworkPropertyMetadata(typeof(Surface3D)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Surface3D"/> class.
        /// </summary>
        public Surface3D()
        {
            SizeChanged += (sender, e) => UpdateRotation();
            Loaded += (sender, e) => UpdateRotation();
            BorderBrush = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(0.1);
            RenderTransform = new ScaleTransform(1,1);
            RenderTransformOrigin = new Point(0.5, 0.5);
           
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _viewport = GetTemplateChild("PART_Viewport") as Viewport3D;
            _camera = GetTemplateChild("PART_Camera") as PerspectiveCamera;
            _content = GetTemplateChild("PART_FrontContent") as Border;
            _frontModel = GetTemplateChild("PART_FrontModel") as Viewport2DVisual3D;
            _backModel = GetTemplateChild("PART_BackModel") as Viewport2DVisual3D;
            _scale = GetTemplateChild("PART_Scale") as ScaleTransform3D;
            _rotate = GetTemplateChild("PART_Rotate") as RotateTransform3D;
            _quaternion = GetTemplateChild("PART_Quaternion") as QuaternionRotation3D;
            _fixedTransform = GetTemplateChild("PART_FixedTransform") as RotateTransform;
            _directionalLights = GetTemplateChild("PART_DirectionalLights") as ModelVisual3D;
            _ambientLights = GetTemplateChild("PART_AmbientLights") as ModelVisual3D;
           // _frontViewbox = GetTemplateChild("PART_FrontViewbox") as Viewbox;

            SizeChanged += (sender, e) => UpdateBounds();
            _content.SizeChanged += (sender, e) => UpdateBounds();
            UpdateBounds();
            UpdateRotation();
            UpdateLights();
            base.OnApplyTemplate();
        }

        #endregion

        #region Properties

        #region RotationX

        /// <summary>
        /// Gets or sets the rotation on the X axis.
        /// </summary>
        /// <value>The rotation on the X axis.</value>
        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationX.
        /// </summary>
        public static readonly DependencyProperty RotationXProperty = DependencyProperty.Register(
            "RotationX",
            typeof(double),
            typeof(Surface3D),
            new FrameworkPropertyMetadata(0.0, (sender, e) => ((Surface3D)sender).UpdateRotation(), (sender, baseValue) => (double)baseValue % 360));

        #endregion

        #region RotationY

        /// <summary>
        /// Gets or sets the rotation on the Y aYis.
        /// </summary>
        /// <value>The rotation on the Y aYis.</value>
        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationY.
        /// </summary>
        public static readonly DependencyProperty RotationYProperty = DependencyProperty.Register(
            "RotationY",
            typeof(double),
            typeof(Surface3D),
            new FrameworkPropertyMetadata(0.0, (sender, e) => ((Surface3D)sender).UpdateRotation(), (sender, baseValue) => (double)baseValue % 360));

        #endregion

        #region RotationZ

        /// <summary>
        /// Gets or sets the rotation on the Z aZis.
        /// </summary>
        /// <value>The rotation on the Z aZis.</value>
        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationZ.
        /// </summary>
        public static readonly DependencyProperty RotationZProperty = DependencyProperty.Register(
            "RotationZ",
            typeof(double),
            typeof(Surface3D),
            new FrameworkPropertyMetadata(0.0, (sender, e) => ((Surface3D)sender).UpdateRotation(), (sender, baseValue) => (double)baseValue % 360));

        #endregion

        #region RotationCenterX

        /// <summary>
        /// Gets or sets the center of the rotation on the X axis.
        /// </summary>
        /// <value>The center of the rotation on the X axis.</value>
        public double RotationCenterX
        {
            get { return (double)GetValue(RotationCenterXProperty); }
            set { SetValue(RotationCenterXProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationCenterX.
        /// </summary>
        public static readonly DependencyProperty RotationCenterXProperty = DependencyProperty.Register(
            "RotationCenterX",
            typeof(double),
            typeof(Surface3D),
            new PropertyMetadata(0.0, (sender, e) => (sender as Surface3D).UpdateRotationCenter()));

        #endregion

        #region RotationCenterY

        /// <summary>
        /// Gets or sets the center of the rotation on the X axis.
        /// </summary>
        /// <value>The center of the rotation on the X axis.</value>
        public double RotationCenterY
        {
            get { return (double)GetValue(RotationCenterYProperty); }
            set { SetValue(RotationCenterYProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationCenterY.
        /// </summary>
        public static readonly DependencyProperty RotationCenterYProperty = DependencyProperty.Register(
            "RotationCenterY",
            typeof(double),
            typeof(Surface3D),
            new PropertyMetadata(0.0, (sender, e) => (sender as Surface3D).UpdateRotationCenter()));

        #endregion

        #region RotationCenterZ

        /// <summary>
        /// Gets or sets the center of the rotation on the Z aZis.
        /// </summary>
        /// <value>The center of the rotation on the Z aZis.</value>
        public double RotationCenterZ
        {
            get { return (double)GetValue(RotationCenterZProperty); }
            set { SetValue(RotationCenterZProperty, value); }
        }

        /// <summary>
        /// Backing store for RotationCenterZ.
        /// </summary>
        public static readonly DependencyProperty RotationCenterZProperty = DependencyProperty.Register(
            "RotationCenterZ",
            typeof(double),
            typeof(Surface3D),
            new PropertyMetadata(0.0, (sender, e) => (sender as Surface3D).UpdateRotationCenter()));

        #endregion

        #region FieldOfView

        /// <summary>
        /// Gets or sets the camera's field of view.
        /// </summary>
        /// <value>The camera's field of view.</value>
        public double FieldOfView
        {
            get { return (double)GetValue(FieldOfViewProperty); }
            set { SetValue(FieldOfViewProperty, value); }
        }

        /// <summary>
        /// Backing store for FieldOfView.
        /// </summary>
        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
            "FieldOfView",
            typeof(double),
            typeof(Surface3D),
            new FrameworkPropertyMetadata(45.0, (sender, e) => ((Surface3D)sender).UpdateCamera(), (sender, baseValue) => Math.Min(Math.Max((double)baseValue, 0.5), 179.9)));

        #endregion

        #region UseLights

        /// <summary>
        /// Gets or sets a value indicating whether to use directional lighting.
        /// </summary>
        /// <value><c>true</c> if using directional lighting; otherwise, <c>false</c>.</value>
        public bool UseLights
        {
            get { return (bool)GetValue(UseLightsProperty); }
            set { SetValue(UseLightsProperty, value); }
        }

        /// <summary>
        /// Backing store for UseLights.
        /// </summary>
        public static readonly DependencyProperty UseLightsProperty = DependencyProperty.Register(
            "UseLights",
            typeof(bool),
            typeof(Surface3D),
            new PropertyMetadata(true, (sender, e) => (sender as Surface3D).UpdateLights()));

        #endregion

        #endregion

        #region Rendering

        /// <summary>
        /// Refreshes the surface. This eliminates blurry text, by rendering the content thus disposing of the Viewport2DVisual3D
        /// </summary>
        public void RefreshSurface()
        {
            if (_content != null)
            {
                _content.InvalidateVisual();
            }
        }

        /// <summary>
        /// Updates the bounds.
        /// </summary>
        public void UpdateBounds()
        {
            _bounds = VisualTreeHelper.GetDescendantBounds(_content);
            UpdateRotation();
            UpdateCamera();
            UpdateRotationCenter();
        }

        /// <summary>
        /// Updates the camera.
        /// </summary>
        private void UpdateCamera()
        {
            if (_camera == null)
            {
                return;
            }

            double fovInRadians = FieldOfView * (Math.PI / 180);
            double z = _bounds.Width / Math.Tan(fovInRadians / 2) / 2;
            _camera.Position = new Point3D(_bounds.Width / 2, _bounds.Height / 2, z);
            _camera.FieldOfView = FieldOfView;
            _scale.ScaleX = _bounds.Width;
            _scale.ScaleY = _bounds.Height;
        }

        /// <summary>
        /// Updates the rotation.
        /// </summary>
        public virtual void UpdateRotation()
        {
            if (_frontModel == null || _bounds == Rect.Empty)
            {
                return;
            }

          
            // Determine if we're showing the backside.
            bool isBackShowing = IsBackShowingRotation(RotationX, RotationY, RotationZ);

            // Update the rotation of the 3D model.
            UpdateQuaternion();

            if (isBackShowing != _lastIsBackShowing)
            {
                // Decide which visual to be showing.
                if (isBackShowing)
                {
                    _frontModel.Visual = null;
                    _backModel.Visual = _content;
                }
                else
                {
                    _backModel.Visual = null;
                    _frontModel.Visual = _content;
                }

                UpdateRotationCenter();
            }
            _lastIsBackShowing = isBackShowing;

        
        }

        /// <summary>
        /// Updates the quaternion.
        /// </summary>
        protected void UpdateQuaternion()
        {
            if (_quaternion == null)
            {
                return;
            }

            Quaternion qx = new Quaternion(_axisX, RotationX);
            Quaternion qy = new Quaternion(_axisY, RotationY);
            Quaternion qz = new Quaternion(_axisZ, RotationZ);
            _quaternion.Quaternion = qx * qy * qz;
        }

        /// <summary>
        /// Updates the rotation center.
        /// </summary>
        protected void UpdateRotationCenter()
        {
            if (_fixedTransform == null || _rotate == null)
            {
                return;
            }

            _rotate.CenterX = (_bounds.Width / 2) + RotationCenterX;
            _rotate.CenterY = (_bounds.Height / 2) - RotationCenterY;
            _rotate.CenterZ = RotationCenterZ;
        }

        /// <summary>
        /// Updates the lights.
        /// </summary>
        private void UpdateLights()
        {
            if (_viewport == null)
            {
                return;
            }

            _viewport.Children.Remove(_ambientLights);
            _viewport.Children.Remove(_directionalLights);
            _viewport.Children.Insert(0, UseLights ? _directionalLights : _ambientLights);
        }

     
        /// <summary>
        /// Determines whether the back of the plane is showing, given a rotation.
        /// </summary>
        /// <param name="x">The x rotation.</param>
        /// <param name="y">The y rotation.</param>
        /// <param name="z">The z rotation.</param>
        /// <returns>
        /// <c>true</c> if the back of the plane is showing, given a rotation; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBackShowingRotation(double x, double y, double z)
        {
            Matrix3D rotMatrix = new Matrix3D();
            rotMatrix.Rotate(new Quaternion(new Vector3D(1, 0, 0), x));
            rotMatrix.Rotate(new Quaternion(new Vector3D(0, 1, 0) * rotMatrix, y));
            rotMatrix.Rotate(new Quaternion(new Vector3D(0, 0, 1) * rotMatrix, z));

            Vector3D transformZ = rotMatrix.Transform(new Vector3D(0, 0, 1));
            return Vector3D.DotProduct(new Vector3D(0, 0, 1), transformZ) < 0;
        }

        #endregion

        private bool _isMouseDown;
        public bool IsMouseDown
        {
            get { return _isMouseDown; }
            set { _isMouseDown = value; NotifyPropertyChanged("IsMouseDown"); }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            IsMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            IsMouseDown = false;
            base.OnMouseLeave(e);
        }



        public event PropertyChangedEventHandler PropertyChanged;
      
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }





        public bool Is3DControl
        {
            get { return (bool)GetValue(Is3DControlProperty); }
            set { SetValue(Is3DControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Is3DControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Is3DControlProperty =
            DependencyProperty.Register("Is3DControl", typeof(bool), typeof(Surface3D),new FrameworkPropertyMetadata(true));

        
        
    }
}
