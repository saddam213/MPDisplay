using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using GUIFramework.Managers;
using GUISkinFramework.Animations;
using GUISkinFramework.Common;
using GUISkinFramework.Dialogs;
using Common.Logging;


namespace GUIFramework.GUI
{
    [GUISkinElement(typeof(XmlDialog))]
    public class GUIDialog : GUISurfaceElement
    {
        #region Fields

        private XmlDialog _baseXml;
        protected Log Log = LoggingManager.GetLog(typeof(GUIDialog));
        private bool _closeOnWindowChanged = false; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIDialog"/> class.
        /// </summary>
        public GUIDialog()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
            DataContext = this;
        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the base XML.
        /// </summary>
        public XmlDialog BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); }
        }

        /// <summary>
        /// Gets a value indicating whether to close on window change.
        /// </summary>
        /// <value>
        /// <c>true</c> if close on window change; otherwise, <c>false</c>.
        /// </value>
        public bool CloseOnWindowChanged
        {
            get { return _closeOnWindowChanged; }
        }

        /// <summary>
        /// Gets or sets the visible condition.
        /// </summary>
        public GUIVisibleCondition VisibleCondition { get; set; }

        /// <summary>
        /// Gets or sets the animations.
        /// </summary>
        public AnimationCollection Animations { get; set; } 

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the specified skin XML.
        /// </summary>
        /// <param name="skinXml">The skin XML.</param>
        public virtual void Initialize(XmlDialog skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            VisibleCondition = new GUIVisibleCondition(this);
            Animations = new AnimationCollection(this, BaseXml.Animations, (condition) => OnAnimationStarted(condition), (condition) => OnAnimationCompleted(condition));
            CreateControls();
            NotifyPropertyChanged("Controls");
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
        /// Opens the Dialog.
        /// </summary>
        /// <returns></returns>
        public virtual Task DialogOpen()
        {
            return Dispatcher.InvokeAsync(() =>
            {
                GUIVisibilityManager.RegisterControlVisibility(this);
                GUIActionManager.RegisterAction(XmlActionType.ControlVisible, ToggleControlVisibility);
                GUIVisibilityManager.RegisterMessage(VisibleMessageType.ControlVisibilityChanged, UpdateControlVisibility);
                InfoRepository.RegisterMessage<int>(InfoMessageType.FocusedDialogControlId, OnMediaPortalFocusedControlChanged);
                foreach (var control in Controls.GetControls())
                {
                    control.OnWindowOpen();
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
                OnMediaPortalFocusedControlChanged(InfoRepository.Instance.FocusedDialogControlId);
            }).Task;
        }

        /// <summary>
        /// Closes the Dialog.
        /// </summary>
        /// <returns></returns>
        public virtual Task DialogClose()
        {
            return Dispatcher.InvokeAsync(() =>
            {
                GUIVisibilityManager.DeregisterControlVisibility(this);
                GUIVisibilityManager.DeregisterMessage(VisibleMessageType.ControlVisibilityChanged, this);
                GUIActionManager.DeregisterAction(XmlActionType.ControlVisible, this);
                InfoRepository.DeregisterMessage(InfoMessageType.FocusedDialogControlId, this);

                foreach (var control in Controls.GetControls())
                {
                    control.OnWindowClose();
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowClose);
            }).Task;
        }

        /// <summary>
        /// Called when a animation starts.
        /// </summary>
        /// <param name="condition">The animation condition</param>
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

        /// <summary>
        /// Called when a animation completes.
        /// </summary>
        /// <param name="condition">The animation condition</param>
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
        /// Called when MediaPortal focused control changes.
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

        #endregion
    }
}
