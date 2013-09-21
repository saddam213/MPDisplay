﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GUISkinFramework.Animations;
using GUISkinFramework.Windows;
using GUIFramework.Managers;
using System.Windows.Data;
using System.ComponentModel;
using MPDisplay.Common.Controls;
using System.Collections.ObjectModel;
using GUISkinFramework.Common;
using MPDisplay.Common.Log;
using MessageFramework.DataObjects;
using GUIFramework.GUI.Controls;

namespace GUIFramework.GUI
{
    [XmlSkinType(typeof(XmlWindow))]
    public class GUIWindow : GUISurfaceElement
    {
        private XmlWindow _baseXml;
        protected Log Log = LoggingManager.GetLog(typeof(GUIWindow));

        public GUIWindow()
        {
            Visibility = System.Windows.Visibility.Collapsed;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
            DataContext = this;
        }

     

      

        public XmlWindow BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); } 
        }
        
        public bool IsDefault { get; private set; }
        public AnimationCollection Animations { get; set; }
        public GUIVisibleCondition VisibleCondition { get; set; }

        public virtual void Initialize(XmlWindow skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            IsDefault = BaseXml.IsDefault;
            VisibleCondition = new GUIVisibleCondition(this);
            CreateAnimations();
            CreateControls();
        }

        public override void CreateControls()
        {
            foreach (var xmlControl in BaseXml.Controls)
            {
                var control = GUIElementFactory.CreateControl(Id, xmlControl);
                control.ParentId = Id;
                Controls.Add(control);
            }
        }
        
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
                    OnMediaPortalFocusedControlChanged(InfoRepository.Instance.FocusedWindowControlId);
                }).Task;
        }



    
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

        public void OnMediaPortalFocusedControlChanged(int controlId)
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.SetFocusedControlId(controlId)));
        }

        private void UpdateControlVisibility()
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.UpdateControlVisibility()));
        }

        private void ToggleControlVisibility(XmlAction obj)
        {
            Dispatcher.InvokeAsync(() => Controls.ForAllControls(c => c.ToggleControlVisibility(obj)));
        }


        #region Animations

        private void CreateAnimations()
        {
            Animations = new AnimationCollection(this, BaseXml.Animations, OnAnimationStarted, OnAnimationCompleted);
        }

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
