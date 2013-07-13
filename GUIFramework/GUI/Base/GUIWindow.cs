using System;
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
            DataContext = this;
            Visibility = System.Windows.Visibility.Hidden;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

        #region Animations

        public virtual void Initialize(XmlWindow skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            IsDefault = BaseXml.IsDefault;
            Animations = new AnimationCollection(this, BaseXml.Animations, (condition) => OnAnimationStarted(condition), (condition) => OnAnimationCompleted(condition)); 
            CreateControls();
        }

        private void OnAnimationCompleted(XmlAnimationCondition condition)
        {
            switch (condition)
            {
                case XmlAnimationCondition.WindowOpen:
                    break;
                case XmlAnimationCondition.WindowClose:
                    Visibility = System.Windows.Visibility.Hidden;
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

        public XmlWindow BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); } 
        }
        
        public bool IsDefault { get; private set; }
    
        public AnimationCollection Animations { get; set; }

      


        public override void CreateControls()
        {
            foreach (var xmlControl in BaseXml.Controls)
            {
                if (Controls.Any(c => c.Id == xmlControl.Id))
                {
                    Log.Message(LogLevel.Warn, "Duplicate Control - Window '{0}' already contains a control with Id:{1}, Duplicate control '{2}' id cannot be added to window.",BaseXml.Name, xmlControl.Id, xmlControl.Name);
                    continue;
                }
                var control = GUIElementFactory.CreateControl(Id, xmlControl);
                control.ParentId = Id;
                Controls.Add(control);
            }
        }

        

        public async Task WindowOpen()
        {
           // await this.RegisterWindowInfo();
            await Dispatcher.InvokeAsync(() =>
            {
                GUIDataRepository.RegisterWindowData(this);
                this.RegisterAction(XmlActionType.ControlVisible, action => this.ToggleControlVisibility(action.GetParam1As<int>(-1)));
                InfoRepository.RegisterMessage<int>(InfoMessageType.FocusedWindowControlId, OnMediaPortalFocusedControlChanged);

                this.RegisterControlVisibility();
                foreach (var control in Controls.GetControls())
                {
                    if (control is IPropertyControl)
                    {
                        (control as IPropertyControl).RegisterProperties();
                    }
                    control.OnWindowOpen();
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
                GUIVisibilityManager.RegisterMessage(VisibleMessageType.ControlVisibilityChanged, this.RefreshControlVisibility);
            });
        }

        public async Task WindowClose()
        {
           await Dispatcher.InvokeAsync(() =>
           {
               this.DeregisterAction(XmlActionType.ControlVisible);
               InfoRepository.DeregisterMessage(InfoMessageType.FocusedWindowControlId, this);
               GUIVisibilityManager.DeregisterMessage(VisibleMessageType.ControlVisibilityChanged,this);

               this.DeregisterControlVisibility();
               foreach (var control in Controls.GetControls())
               {
                   if (control is IPropertyControl)
                   {
                       (control as IPropertyControl).DergisterProperties();
                   }
                   control.OnWindowClose();
               }
               Animations.StartAnimation(XmlAnimationCondition.WindowClose);
           });
        }

  



        public void OnMediaPortalFocusedControlChanged(int controlId)
        {
            foreach (var control in Controls.GetControls())
            {
                control.SetFocusedControlId(controlId);
            }
        }

     


    }
}
