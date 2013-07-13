using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GUIFramework.Managers;
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using GUISkinFramework.Property;
using MPDisplay.Common.Controls;
using MPDisplay.Common.Log;


namespace GUIFramework.GUI
{
    [XmlSkinType(typeof(XmlControl))]
    public class GUIControl : Surface3D
    {
        private XmlControl _baseXml;
        //protected Log Log = LoggingManager.GetLog(typeof(GUIControl));

        public GUIControl()
        {
            DataContext = this;
            ClipToBounds = true;
        }

        public void Initialize(int parentId, XmlControl skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            IsWindowOpenVisible = BaseXml.IsWindowOpenVisible;
            FocusedControlIds = skinXml.MediaPortalFocusControls.ToList();
            ParentId = parentId;
            CreateControl();
            CreateVisibleConditions();
            CreateAnimations();
            OnInitialized();
            SetControlVisibility(IsWindowOpenVisible);
        }

        public virtual void OnInitialized()
        {
           
        }

        public virtual void CreateControl()
        {
           
        }

     

        public XmlControl BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); }
        }

        public int ParentId { get; set; }
        public int Id { get; set; }
        public GUIVisibleCondition VisibleCondition { get; set; }
        public AnimationCollection Animations { get; set; }
       // public List<XmlProperty> RegisteredProperties { get; set; }
        public List<int> FocusedControlIds { get; set; }
        public bool IsControlVisible
        {
            get { return _isVisible; }
        }
        public bool IsWindowOpenVisible { get; set; }

        private bool _isVisible = true;
        private bool _isfocused;


        protected Log Log = LoggingManager.GetLog(typeof(GUIControl));
    
        public virtual void CreateVisibleConditions()
        {
            VisibleCondition = new GUIVisibleCondition(ParentId, BaseXml.VisibleCondition);
        }

        public virtual void CreateAnimations()
        {
            Animations = new AnimationCollection(this, BaseXml.Animations, (condition) => OnAnimationStarted(condition), (condition) => OnAnimationCompleted(condition)); 
        }

        public virtual void OnAnimationStarted(XmlAnimationCondition condition)
        {
            Log.Message(LogLevel.Verbose, "OnAnimationStarted({0}), WindowId: {1}, ControlName: {2}, ControlId {3}", condition, ParentId, Name, Id);
            switch (condition)
            {
                case XmlAnimationCondition.VisibleTrue:
                    Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }


        public virtual void OnAnimationCompleted(XmlAnimationCondition condition)
        {
            Log.Message(LogLevel.Verbose, "OnAnimationCompleted({0}), WindowId: {1}, ControlName: {2}, ControlId {3}", condition, ParentId, Name, Id);
            switch (condition)
            {
                case XmlAnimationCondition.VisibleFalse:
                    Visibility = System.Windows.Visibility.Hidden;
                    ClearInfoData();
                    break;
                case XmlAnimationCondition.WindowClose:
                    ClearInfoData();
                    break;
                case XmlAnimationCondition.PropertyChanging:
                    OnPropertyChanged();
                    break;
                default:
                    break;
            }
        }

        public virtual void OnPropertyChanging()
        {
            Animations.StartAnimation(XmlAnimationCondition.PropertyChanging);
        }

        public virtual void OnPropertyChanged()
        {
            Animations.StartAnimation(XmlAnimationCondition.PropertyChanged);
        }

        public virtual void OnFocusFalse()
        {
            Animations.StartAnimation(XmlAnimationCondition.FocusFalse);
        }

        public virtual void OnFocusTrue()
        {
            Animations.StartAnimation(XmlAnimationCondition.FocusTrue);
        }

        public virtual void OnVisibleFalse()
        {
            Animations.StartAnimation(XmlAnimationCondition.VisibleFalse);
            if (_isInfoDataRegistered)
            {
                DeregisterInfoData();
            }
        }

        public virtual void OnVisibleTrue()
        {
            Animations.StartAnimation(XmlAnimationCondition.VisibleTrue);
            if (!_isInfoDataRegistered)
            {
                RegisterInfoData();
            }
        }

        public virtual void OnTouchUp()
        {
            Animations.StartAnimation(XmlAnimationCondition.TouchUp);
        }

        public virtual void OnTouchDown()
        {
            Animations.StartAnimation(XmlAnimationCondition.TouchDown);
        }

        public virtual void OnWindowClose()
        {
            Animations.StartAnimation(XmlAnimationCondition.WindowClose);
            if (_isInfoDataRegistered)
            {
                DeregisterInfoData();
            }
        }

        public virtual void OnWindowOpen()
        {
            Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
            if (!_isInfoDataRegistered)
            {
                RegisterInfoData();
            }
        }

     


        public void SetFocusedControlId(int focusedControlId)
        {
            if (FocusedControlIds != null && FocusedControlIds.Any())
            {
                bool shouldFocus = FocusedControlIds.Contains(focusedControlId);
                if (shouldFocus != _isfocused)
                {
                    if (_isfocused)
                    {
                        OnFocusFalse();
                    }
                    else
                    {
                        OnFocusTrue();
                    }
                    _isfocused = shouldFocus;
                }
            }
        }

        public void SetControlVisibility(bool value)
        {
            if (value != _isVisible)
            {
                if (_isVisible)
                {
                    OnVisibleFalse();
                }
                else
                {
                    OnVisibleTrue();
                }
                _isVisible = value;
            }
        }


        private bool _isInfoDataRegistered = false;

        public virtual void RegisterInfoData()
        {
            _isInfoDataRegistered = true;
        }

        public virtual void DeregisterInfoData()
        {
            _isInfoDataRegistered = false;
        }


        public virtual void ClearInfoData()
        {

        }



        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            OnTouchDown();
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            OnTouchUp();
            base.OnPreviewMouseUp(e);
        }

    }
}
