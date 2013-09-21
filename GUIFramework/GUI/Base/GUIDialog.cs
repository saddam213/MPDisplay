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
using GUISkinFramework.Dialogs;
using MPDisplay.Common.Controls;
using System.Collections.ObjectModel;
using GUISkinFramework.Common;
using MPDisplay.Common.Log;


namespace GUIFramework.GUI
{
    [XmlSkinType(typeof(XmlDialog))]
    public class GUIDialog : GUISurfaceElement
    {

          private XmlDialog _baseXml;
          protected Log Log = LoggingManager.GetLog(typeof(GUIDialog));
          private bool _closeOnWindowChanged = false;
       

        public GUIDialog()
        {
          
            Visibility = System.Windows.Visibility.Collapsed;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
            DataContext = this;
        }

      


        #region Animations

        public virtual void Initialize(XmlDialog skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            VisibleCondition = new GUIVisibleCondition(this);
            Animations = new AnimationCollection(this, BaseXml.Animations, (condition) => OnAnimationStarted(condition), (condition) => OnAnimationCompleted(condition)); 
            CreateControls();
            NotifyPropertyChanged("Controls");
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

        public XmlDialog BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); } 
        }

        public bool CloseOnWindowChanged
        {
            get { return _closeOnWindowChanged; }
        }

        public GUIVisibleCondition VisibleCondition { get; set; }

        public AnimationCollection Animations { get; set; }


        public override void CreateControls()
        {
            foreach (var xmlControl in BaseXml.Controls)
            {
                var control = GUIElementFactory.CreateControl(Id, xmlControl);
                control.ParentId = Id;
                Controls.Add(control);
            }
        }

        

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






    }
}
