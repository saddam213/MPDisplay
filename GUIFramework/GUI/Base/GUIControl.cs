using System.Collections.Generic;
using System.Linq;
using GUIFramework.GUI.Controls;
using GUIFramework.Managers;
using GUIFramework.Utils;
using GUISkinFramework.Animations;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Property;
using MPDisplay.Common.Controls;
using MPDisplay.Common.Log;


namespace GUIFramework.GUI
{
    /// <summary>
    /// 
    /// </summary>
    [XmlSkinType(typeof(XmlControl))]
    public class GUIControl : Surface3D
    {
        #region Fields

        protected Log Log = LoggingManager.GetLog(typeof(GUIControl));
        private XmlControl _baseXml;
        private bool _isWindowOpenVisible = true;
        private bool _isControlVisible = false;
        private bool _isVisibleToggled = false;
        private bool _isControlfocused = false;
        private GUIVisibleCondition _visibleCondition;
        private AnimationCollection _animations;
        private List<int> _focusedControlIds;
        private bool _isDataRegistered = false;
     
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIControl"/> class.
        /// </summary>
        public GUIControl()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            DataContext = this;
            ClipToBounds = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the base XML.
        /// </summary>
        public XmlControl BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the registered properties.
        /// </summary>
        public List<XmlProperty> RegisteredProperties { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is control visible.
        /// </summary>
        public bool IsControlVisible
        {
            get { return _isControlVisible; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is window open visible.
        /// </summary>
        public bool IsWindowOpenVisible
        {
            get { return _isWindowOpenVisible; }
        }

        /// <summary>
        /// Gets the animations.
        /// </summary>
        public AnimationCollection Animations
        {
            get { return _animations; }
        }

     
        public bool IsControlFocused
        {
            get { return _isControlfocused; }
            set { _isControlfocused = value; NotifyPropertyChanged("IsControlFocused"); }
        }
        

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes the control.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="skinXml">The skin XML.</param>
        public void Initialize(int parentId, XmlControl skinXml)
        {
            Is3DControl = skinXml.Pos3DX != 0 || skinXml.Pos3DY != 0 || skinXml.Pos3DZ != 0 || skinXml.Animations.OfType<XmlRotateAnimation>().Any();
            BaseXml = skinXml;
            Id = BaseXml.Id;
            ParentId = parentId;
            _isWindowOpenVisible = BaseXml.IsWindowOpenVisible;
            _focusedControlIds = skinXml.MediaPortalFocusControls.ToList();
            CreateControl();
            _visibleCondition = new GUIVisibleCondition(this);
            CreateAnimations();
            OnInitialized();
        }

        /// <summary>
        /// Creates the control.
        /// </summary>
        public virtual void CreateControl()
        {
        }

        /// <summary>
        /// Called when [initialized].
        /// </summary>
        public virtual void OnInitialized()
        {
        }

        /// <summary>
        /// Called when [window close].
        /// </summary>
        public virtual void OnWindowClose()
        {
            Animations.StartAnimation(XmlAnimationCondition.WindowClose);
            DeregisterInfoData();
        }

        /// <summary>
        /// Called when [window open].
        /// </summary>
        public virtual void OnWindowOpen()
        {
            _isVisibleToggled = false;
            _isControlVisible = _isWindowOpenVisible;
            NotifyPropertyChanged("IsControlVisible");

            if (_isControlVisible)
            {
                Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
                RegisterInfoData();
                UpdateInfoData(); 
            }
         
        } 

        #endregion

        #region Animations

        /// <summary>
        /// Creates the animations.
        /// </summary>
        public void CreateAnimations()
        {
            _animations = new AnimationCollection(this, BaseXml.Animations, AnimationStarted, AnimationComplete);
        }


        private void AnimationStarted(XmlAnimationCondition condition)
        {
            Dispatcher.Invoke(() =>
            {
                OnAnimationStarted(condition);
            });
        }

        private void AnimationComplete(XmlAnimationCondition condition)
        {
            Dispatcher.Invoke(() =>
            {
                OnAnimationCompleted(condition);
            });
        }

        /// <summary>
        /// Called when [animation started].
        /// </summary>
        /// <param name="condition">The condition.</param>
        public virtual void OnAnimationStarted(XmlAnimationCondition condition)
        {
           // Log.Message(LogLevel.Verbose, "OnAnimationStarted({0}), WindowId: {1}, ControlName: {2}, ControlId {3}", condition, ParentId, Name, Id);
            switch (condition)
            {
                case XmlAnimationCondition.VisibleTrue:
                    NotifyPropertyChanged("IsControlVisible");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when [animation completed].
        /// </summary>
        /// <param name="condition">The condition.</param>
        public virtual void OnAnimationCompleted(XmlAnimationCondition condition)
        {
            //Log.Message(LogLevel.Verbose, "OnAnimationCompleted({0}), WindowId: {1}, ControlName: {2}, ControlId {3}", condition, ParentId, Name, Id);
            switch (condition)
            {
                case XmlAnimationCondition.VisibleFalse:
                    NotifyPropertyChanged("IsControlVisible");
                    ClearInfoData();
                    break;
                case XmlAnimationCondition.WindowClose:
                    ClearInfoData();
                    break;
                case XmlAnimationCondition.PropertyChanging:
                    OnPropertyChanged();
                    break;
                case XmlAnimationCondition.WindowOpen:
                    SetWindowOpenVisibility();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Info/Property Data

        private void RegisterInfoData()
        {
            if (!_isDataRegistered)
            {
                _isDataRegistered = true;
                OnRegisterInfoData();
            }
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        private void DeregisterInfoData()
        {
            if (_isDataRegistered)
            {
                _isDataRegistered = false;
                OnDeregisterInfoData();
            }
        }


        /// <summary>
        /// Called when [property changing].
        /// </summary>
        public void OnPropertyChanging()
        {
            Animations.StartAnimation(XmlAnimationCondition.PropertyChanging);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        public void OnPropertyChanged()
        {
            Animations.StartAnimation(XmlAnimationCondition.PropertyChanged);
            UpdateInfoData();
        }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public virtual void OnRegisterInfoData()
        {
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public virtual void OnDeregisterInfoData()
        {
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public virtual void UpdateInfoData()
        {
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public virtual void ClearInfoData()
        {
        }

        #endregion

        #region Visibility

        /// <summary>
        /// Called when [visible false].
        /// </summary>
        private void OnVisibleFalse()
        {
            Animations.StartAnimation(XmlAnimationCondition.VisibleFalse);
            DeregisterInfoData();
        }

        /// <summary>
        /// Called when [visible true].
        /// </summary>
        private void OnVisibleTrue()
        {
            Animations.StartAnimation(XmlAnimationCondition.VisibleTrue);
            RegisterInfoData();
            UpdateInfoData();
        }

        /// <summary>
        /// Toggles the control visibility.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ToggleControlVisibility(XmlAction action)
        {
            if (action != null && action.GetParam1As<int>(-1) == Id)
            {
                _isVisibleToggled = !_isVisibleToggled;
                SetControlVisibility(!_isControlVisible);
            }
        }

        /// <summary>
        /// Updates the control visibility.
        /// </summary>
        public void UpdateControlVisibility()
        {
            if (!_isVisibleToggled)
            {
                if (_visibleCondition.HasCondition)
                {
                    SetControlVisibility(_visibleCondition.ShouldBeVisible());
                }
                else
                {
                    SetControlVisibility(_isWindowOpenVisible);
                }
            }
        }

        /// <summary>
        /// Sets the control visibility.
        /// </summary>
        /// <param name="visible">if set to <c>true</c> [visible].</param>
        private void SetControlVisibility(bool visible)
        {
            if (visible != _isControlVisible)
            {
                _isControlVisible = visible;
                GUIVisibilityManager.UpdateControlVisibility(this);
                if (_isControlVisible)
                {
                    OnVisibleTrue();
                }
                else
                {
                    OnVisibleFalse();
                }
            }
        }

        private void SetWindowOpenVisibility()
        {
            _isControlfocused = false;
            _isVisibleToggled = false;
            _isControlVisible = _visibleCondition.HasCondition
                    ? _visibleCondition.ShouldBeVisible()
                    : _isWindowOpenVisible;
            GUIVisibilityManager.UpdateControlVisibility(this);
            if (_isControlVisible)
            {
                OnVisibleTrue();
            }
            else
            {
                OnVisibleFalse();
            }
        }

        #endregion

        #region Focus

        /// <summary>
        /// Called when [focus false].
        /// </summary>
        private void OnFocusFalse()
        {
            Animations.StartAnimation(XmlAnimationCondition.FocusFalse);
        }

        /// <summary>
        /// Called when [focus true].
        /// </summary>
        private void OnFocusTrue()
        {
            Animations.StartAnimation(XmlAnimationCondition.FocusTrue);
        }

        /// <summary>
        /// Sets the focused control id.
        /// </summary>
        /// <param name="focusedControlId">The focused control id.</param>
        public void SetFocusedControlId(int focusedControlId)
        {
            if (_focusedControlIds != null && _focusedControlIds.Any())
            {
                bool shouldFocus = _focusedControlIds.Contains(focusedControlId);
                if (shouldFocus != _isControlfocused)
                {
                    if (_isControlfocused)
                    {
                        OnFocusFalse();
                    }
                    else
                    {
                        OnFocusTrue();
                    }
                    IsControlFocused = shouldFocus;
                }
            }
        }

        #endregion

        #region Touch Interaction

        /// <summary>
        /// Called when [touch up].
        /// </summary>
        public virtual void OnTouchUp()
        {
            Animations.StartAnimation(XmlAnimationCondition.TouchUp);
        }

        /// <summary>
        /// Called when [touch down].
        /// </summary>
        public virtual void OnTouchDown()
        {
            Animations.StartAnimation(XmlAnimationCondition.TouchDown);
        }

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(this is GUIList))
            {
                OnTouchDown();
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(this is GUIList || this is GUIGuide))
            {
                OnTouchUp();
            }
            base.OnPreviewMouseUp(e);
        }

        #endregion
    }
}
