using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Common.Logging;
using GUIFramework.Managers;
using GUISkinFramework.Animations;
using GUISkinFramework.Common;
using GUISkinFramework.Windows;

namespace GUIFramework.GUI
{
    [GUISkinElement(typeof(XmlWindow))]
    public class GUIWindow : GUISurfaceElement
    {
        private XmlWindow _baseXml;
        protected Log Log = LoggingManager.GetLog(typeof(GUIWindow));

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIWindow"/> class.
        /// </summary>
        public GUIWindow()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
            DataContext = this;
        }

        /// <summary>
        /// Gets or sets the windows skin Xml.
        /// </summary>
        public XmlWindow BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); } 
        }

        /// <summary>
        /// Gets a value indicating whether this is the default window.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Gets or sets the animations.
        /// </summary>
        public AnimationCollection Animations { get; set; }

        /// <summary>
        /// Gets or sets the visible condition.
        /// </summary>
        public GUIVisibleCondition VisibleCondition { get; set; }

        /// <summary>
        /// Initializes the specified skin XML.
        /// </summary>
        /// <param name="skinXml">The skin XML.</param>
        public virtual void Initialize(XmlWindow skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            IsDefault = BaseXml.IsDefault;
            VisibleCondition = new GUIVisibleCondition(this);
            CreateAnimations();
            CreateControls();
        }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        public override void CreateControls()
        {
            foreach (var xmlControl in BaseXml.Controls)
            {
                var control = GUIElementFactory.CreateControl(Id, xmlControl);
                control.ParentId = Id;
                Controls.Add(control);
            }
        }

        /// <summary>
        /// Opens the window
        /// </summary>
        /// <returns></returns>
        public virtual Task WindowOpen()
        {
            return Dispatcher.InvokeAsync(() =>
            {
                GUIVisibilityManager.RegisterControlVisibility(this);
                GUIActionManager.RegisterAction(XmlActionType.ControlVisible, ToggleControlVisibility);
                GUIVisibilityManager.RegisterMessage(VisibleMessageType.ControlVisibilityChanged, UpdateControlVisibility);
                InfoRepository.RegisterMessage<int>(InfoMessageType.FocusedWindowControlId, OnMediaPortalFocusedControlChanged);
                foreach (var control in Controls.GetControls())
                {
                    control.OnWindowOpen();
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowOpen);

                OnMediaPortalFocusedControlChanged(_baseXml.DefaultMediaPortalFocusedControlId == -1
                    ? InfoRepository.Instance.FocusedWindowControlId
                    : _baseXml.DefaultMediaPortalFocusedControlId);
            }).Task;
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <returns></returns>
        public virtual Task WindowClose()
        {
            return Dispatcher.InvokeAsync(() =>
            {
                GUIVisibilityManager.DeregisterControlVisibility(this);
                GUIVisibilityManager.DeregisterMessage(VisibleMessageType.ControlVisibilityChanged, this);
                GUIActionManager.DeregisterAction(XmlActionType.ControlVisible, this);
                InfoRepository.DeregisterMessage(InfoMessageType.FocusedWindowControlId, this);

                foreach (var control in Controls.GetControls())
                {
                    control.OnWindowClose();
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowClose);
            }).Task;
        }

        /// <summary>
        /// Called when [media portal focused control changed].
        /// </summary>
        /// <param name="controlId">The control id.</param>
        public void OnMediaPortalFocusedControlChanged(int controlId)
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.SetFocusedControlId(controlId)));
        }

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        private void UpdateControlVisibility()
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.UpdateControlVisibility()));
        }

        /// <summary>
        /// Toggles the control visibility.
        /// </summary>
        /// <param name="obj">The obj.</param>
        private void ToggleControlVisibility(XmlAction obj)
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.ToggleControlVisibility(obj)));
        }


        #region Animations

        /// <summary>
        /// Creates the animations.
        /// </summary>
        private void CreateAnimations()
        {
            Animations = new AnimationCollection(this, BaseXml.Animations, OnAnimationStarted, OnAnimationCompleted);
        }

        /// <summary>
        /// Called when an animation completes.
        /// </summary>
        /// <param name="condition">The condition.</param>
        private void OnAnimationCompleted(XmlAnimationCondition condition)
        {
            switch (condition)
            {
                case XmlAnimationCondition.WindowOpen:
                    break;
                case XmlAnimationCondition.WindowClose:
                    Visibility = System.Windows.Visibility.Collapsed;
                    foreach (var control in Controls.GetControls())
                    {
                        control.ClearInfoData();
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when an animation starts.
        /// </summary>
        /// <param name="condition">The condition.</param>
        private void OnAnimationStarted(XmlAnimationCondition condition)
        {
            switch (condition)
            {
                case XmlAnimationCondition.WindowOpen:
                    Visibility = Visibility.Visible;
                    break;
                case XmlAnimationCondition.WindowClose:
                    break;
            }
        }

        #endregion
       
    }
}
