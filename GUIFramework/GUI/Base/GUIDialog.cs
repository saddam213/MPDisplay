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

        public GUIDialog()
        {
            DataContext = this;
            Visibility = System.Windows.Visibility.Hidden;
            RenderTransform = new ScaleTransform(1, 1);
            RenderTransformOrigin = new Point(0.5, 0.5);
        }

    


        #region Animations

        public virtual void Initialize(XmlDialog skinXml)
        {
            BaseXml = skinXml;
            Id = BaseXml.Id;
            VisibleCondition = new GUIVisibleCondition(this);
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

        public XmlDialog BaseXml
        {
            get { return _baseXml; }
            set { _baseXml = value; NotifyPropertyChanged("BaseXml"); } 
        }

        public bool CloseOnWindowChanged { get; set; }

        public GUIVisibleCondition VisibleCondition { get; set; }

        public AnimationCollection Animations { get; set; }


        public override void CreateControls()
        {
            Controls = new List<GUIControl>();
            foreach (var xmlControl in BaseXml.Controls)
            {
                if (Controls.Any(c => c.Id == xmlControl.Id))
                {
                    Log.Message(LogLevel.Error, "Duplicate Control - Dialog '{0}' already contains a control with Id:{1}, Duplicate control '{2}' id cannot be added to Dialog.", BaseXml.Name, xmlControl.Id, xmlControl.Name);
                    continue;
                }
                var control = GUIElementFactory.CreateControl(Id, xmlControl);
                control.ParentId = Id;
           
                Controls.Add(control);
            }
            NotifyPropertyChanged("Controls");
        }

        

        public async Task DialogOpen()
        {
            await Dispatcher.InvokeAsync(() =>
            {
             //   GUIDataRepository.RegisterDialogData(this);
              //  this.RegisterAction(XmlActionType.ControlVisible, action => this.ToggleControlVisibility(action.GetParam1As<int>(-1)));
              //  this.RegisterControlVisibility();
                foreach (var control in Controls.GetControls())
                {
                    control.Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowOpen);
            });
        }

        public async Task DialogClose()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                this.DeregisterAction(XmlActionType.ControlVisible);

              //  this.DeregisterControlVisibility();
                foreach (var control in Controls.GetControls())
                {
                    control.Animations.StartAnimation(XmlAnimationCondition.WindowClose);
                }
                Animations.StartAnimation(XmlAnimationCondition.WindowClose);
            });
        }








        public void OnMediaPortalFocusedControlChanged(int controlId)
        {
            foreach (var control in Controls.GetControls())
            {
               
            }
        }

    }
}
