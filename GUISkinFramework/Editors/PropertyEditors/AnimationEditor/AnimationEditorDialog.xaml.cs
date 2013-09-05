﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GUISkinFramework.Animations;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Windows;
using MPDisplay.Common.Controls;

namespace GUISkinFramework.Editor.PropertyEditors
{
    /// <summary>
    /// Interaction logic for VisibleConditionEditorDialog.xaml
    /// </summary>
    public partial class AnimationEditorDialog : Window, INotifyPropertyChanged
    {
        private object _instance;
        private object _animatedElement;
        private AnimationType _selectedAnimationType;
        private XmlAnimationCondition _selectedAnimationCondition;
        private XmlAnimation _selectedAnimation;
        private ObservableCollection<XmlAnimation> _xmlAnimations = new ObservableCollection<XmlAnimation>();

        public AnimationEditorDialog(object instance)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            if (instance is XmlControl)
            {
                _instance = (instance as XmlControl).CreateCopy<XmlControl>();
                AnimatedElement = (instance as XmlControl).CreateCopy<XmlControl>();
            }

            if (instance is XmlWindow)
            {
                _instance = (instance as XmlWindow).CreateCopy<XmlWindow>();
                AnimatedElement = (instance as XmlWindow).CreateCopy<XmlWindow>();
            }

            if (instance is XmlDialog)
            {
                _instance = (instance as XmlDialog).CreateCopy<XmlDialog>();
                AnimatedElement = (instance as XmlDialog).CreateCopy<XmlDialog>();
            }

            NotifyPropertyChanged("AnimationConditions");
        }

        public object AnimatedElement
        {
            get { return _animatedElement; }
            set { _animatedElement = value; NotifyPropertyChanged("AnimatedElement"); }
        }

        public List<XmlAnimationCondition> AnimationConditions
        {
            get
            {
                if (_instance is XmlWindow || _instance is XmlDialog)
                {
                    return new List<XmlAnimationCondition> {XmlAnimationCondition.None, XmlAnimationCondition.WindowClose, XmlAnimationCondition.WindowOpen };
                }
                return Enum.GetValues(typeof(XmlAnimationCondition))
                       .OfType<XmlAnimationCondition>().ToList();
            }
        }

        public void SetItems(ObservableCollection<XmlAnimation> items)
        {
            if (items != null)
            {
                foreach (var animation in items)
                {
                    XmlAnimations.Add(animation.CreateCopy());
                }
            }
        }

        public ObservableCollection<XmlAnimation> GetItems()
        {
            return new ObservableCollection<XmlAnimation>(XmlAnimations.Where(a => a.Condition != XmlAnimationCondition.None).ToList());
        }

        public ObservableCollection<XmlAnimation> XmlAnimations
        {
            get { return _xmlAnimations; }
            set { _xmlAnimations = value; NotifyPropertyChanged("XmlAnimations"); }
        }

        public XmlAnimation SelectedAnimation
        {
            get { return _selectedAnimation; }
            set { _selectedAnimation = value; NotifyPropertyChanged("SelectedAnimation"); }
        }

        public XmlAnimationCondition SelectedAnimationCondition
        {
            get { return _selectedAnimationCondition; }
            set
            {
                _selectedAnimationCondition = value; 
                NotifyPropertyChanged("SelectedAnimationCondition");
                NotifyPropertyChanged("FilteredList"); 
            }
        }

        public AnimationType SelectedAnimationType
        {
            get { return _selectedAnimationType; }
            set { _selectedAnimationType = value; }
        }

        public List<XmlAnimation> FilteredList
        {
            get { return _xmlAnimations.Where(x => x.Condition == _selectedAnimationCondition).ToList(); }
        }
      
        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            XmlAnimation animation = null;
            switch (_selectedAnimationType)
            {
                case AnimationType.Slide:
                    animation = new XmlSlideAnimation { Condition = _selectedAnimationCondition };
                    break;
                case AnimationType.Fade:
                    animation = new XmlFadeAnimation { Condition = _selectedAnimationCondition };
                    break;
                case AnimationType.Zoom:
                    animation = new XmlZoomAnimation { Condition = _selectedAnimationCondition };
                    break;
                case AnimationType.Rotate:
                    animation = new XmlRotateAnimation { Condition = _selectedAnimationCondition };
                    break;
                default:
                    break;
            }
            if (animation != null)
            {
                XmlAnimations.Add(animation);  
                SelectedAnimation = animation;
                NotifyPropertyChanged("FilteredList");
            }
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            XmlAnimations.Remove(SelectedAnimation);
            SelectedAnimation = FilteredList.FirstOrDefault();
            NotifyPropertyChanged("FilteredList");
        }

        private void Button_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAnimation != null)
            {
                int currentIndex = XmlAnimations.IndexOf(_selectedAnimation);
                int newIndex = Math.Max(0, currentIndex - 1);
                XmlAnimations.Move(currentIndex, newIndex);
                NotifyPropertyChanged("FilteredList");
            }
        }

        private void Button_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAnimation != null)
            {
                int currentIndex = XmlAnimations.IndexOf(_selectedAnimation);
                int newIndex = Math.Min(XmlAnimations.Count - 1, currentIndex + 1);
                XmlAnimations.Move(currentIndex, newIndex);
                NotifyPropertyChanged("FilteredList");
            }
        }


        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
    
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAnimation != null)
            {
                var storyboard = CreateAnimation(animatedControl, new List<XmlAnimation> { SelectedAnimation });
                storyboard.Begin(animatedControl, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void Button_PlayAll_Click(object sender, RoutedEventArgs e)
        {
            if (FilteredList.Count() > 0)
            {
                var storyboard = CreateAnimation(animatedControl, FilteredList);
                storyboard.Begin(animatedControl, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void Button_Reset_Click(object sender, RoutedEventArgs e)
        {
            animatedControl.BeginAnimation(ContentControl.OpacityProperty, null);
            animatedControl.BeginAnimation(Canvas.LeftProperty, null);
            animatedControl.BeginAnimation(Canvas.TopProperty, null);
            animatedControl.BeginAnimation(ContentControl.WidthProperty, null);
            animatedControl.BeginAnimation(ContentControl.HeightProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationXProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationYProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationXProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationCenterXProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationCenterYProperty, null);
            animatedControl.BeginAnimation(Surface3D.RotationCenterZProperty, null);
            animatedControl.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            animatedControl.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            animatedControl.RenderTransform = new ScaleTransform(1, 1);
            animatedControl.RenderTransformOrigin = new Point(0.5, 0.5);
        }


        public Storyboard CreateAnimation(Surface3D element, IEnumerable<XmlAnimation> animations)
        {
            if (animations != null && animations.Any())
            {
                var storyboard = new Storyboard();
                foreach (var animation in animations.OfType<XmlSlideAnimation>())
                {
                    Add(storyboard,element, CreateDoubleAnimation(animation.StartX, animation.EndX, animation), new PropertyPath(Canvas.LeftProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.StartY, animation.EndY, animation), new PropertyPath(Canvas.TopProperty));
                    Add(storyboard, element, CreateIntAnimation(animation.StartZ, animation.EndZ, animation), new PropertyPath(Canvas.ZIndexProperty));
                }
                foreach (var animation in animations.OfType<XmlFadeAnimation>())
                {
                    Add(storyboard, element, CreateDoubleAnimation(GetOpacity(animation.From), GetOpacity(animation.To), animation), new PropertyPath(FrameworkElement.OpacityProperty));
                }
                foreach (var animation in animations.OfType<XmlZoomAnimation>())
                {
                    Add(storyboard, element, CreateDoubleAnimation(GetZoom(animation.From), GetZoom(animation.To), animation), new PropertyPath("RenderTransform.ScaleX"));
                    Add(storyboard, element, CreateDoubleAnimation(GetZoom(animation.From), GetZoom(animation.To), animation), new PropertyPath("RenderTransform.ScaleY"));
                }
                foreach (var animation in animations.OfType<XmlRotateAnimation>())
                {
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DXFrom, animation.Pos3DXTo, animation), new PropertyPath(Surface3D.RotationXProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DYFrom, animation.Pos3DYTo, animation), new PropertyPath(Surface3D.RotationYProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DZFrom, animation.Pos3DZTo, animation), new PropertyPath(Surface3D.RotationZProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DCenterXFrom, animation.Pos3DCenterXTo, animation), new PropertyPath(Surface3D.RotationCenterXProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DCenterYFrom, animation.Pos3DCenterYTo, animation), new PropertyPath(Surface3D.RotationCenterYProperty));
                    Add(storyboard, element, CreateDoubleAnimation(animation.Pos3DCenterZFrom, animation.Pos3DCenterZTo, animation), new PropertyPath(Surface3D.RotationCenterZProperty));
                }
                return storyboard;
            }
            return null;
        }


        private void Add(Storyboard storyboard, FrameworkElement element, Timeline animation, PropertyPath propertyTarget)
        {
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, propertyTarget);
        }


        private DoubleAnimationUsingKeyFrames CreateDoubleAnimation(double from, double to, XmlAnimation xmlAnimation)
        {
            var doubleanimation = new DoubleAnimationUsingKeyFrames();
            doubleanimation.AutoReverse = xmlAnimation.Reverse;
            doubleanimation.IsAdditive = false;
            Storyboard.SetDesiredFrameRate(doubleanimation, 30);
            doubleanimation.IsCumulative = false;
            doubleanimation.BeginTime = new TimeSpan(0, 0, 0, 0, xmlAnimation.Delay);

            doubleanimation.Duration = new Duration(TimeSpan.FromMilliseconds(xmlAnimation.Duration));
            doubleanimation.RepeatBehavior = xmlAnimation.Repeat == -1 ? RepeatBehavior.Forever : new RepeatBehavior(xmlAnimation.Repeat);
            EasingDoubleKeyFrame start = new EasingDoubleKeyFrame();
            EasingDoubleKeyFrame end = new EasingDoubleKeyFrame();
            EasingMode easingmode = EasingMode.EaseInOut;
            if (Enum.TryParse<EasingMode>(xmlAnimation.Easing.ToString(), out easingmode))
            {
                var easingFunction = new QuadraticEase();
                easingFunction.EasingMode = easingmode;
                start = new EasingDoubleKeyFrame(from, KeyTime.FromPercent(0), easingFunction);
                end = new EasingDoubleKeyFrame(to, KeyTime.FromPercent(1.0), easingFunction);
            }
            else
            {
                start = new EasingDoubleKeyFrame(from, KeyTime.FromPercent(0));
                end = new EasingDoubleKeyFrame(to, KeyTime.FromPercent(1.0));
            }
            doubleanimation.KeyFrames.Add(start);
            doubleanimation.KeyFrames.Add(end);
            //doubleanimation.Freeze();
            return doubleanimation;
        }

        private Int32AnimationUsingKeyFrames CreateIntAnimation(int from, int to, XmlAnimation xmlAnimation)
        {
            var intanimation = new Int32AnimationUsingKeyFrames();
            intanimation.AutoReverse = xmlAnimation.Reverse;
            intanimation.IsAdditive = false;
            Storyboard.SetDesiredFrameRate(intanimation, 30);
            intanimation.IsCumulative = false;
            intanimation.BeginTime = new TimeSpan(0, 0, 0, 0, xmlAnimation.Delay);

            intanimation.Duration = new Duration(TimeSpan.FromMilliseconds(xmlAnimation.Duration));
            intanimation.RepeatBehavior = xmlAnimation.Repeat == -1 ? RepeatBehavior.Forever : new RepeatBehavior(xmlAnimation.Repeat);
            EasingInt32KeyFrame start = new EasingInt32KeyFrame();
            EasingInt32KeyFrame end = new EasingInt32KeyFrame();
            EasingMode easingmode = EasingMode.EaseInOut;
            if (Enum.TryParse<EasingMode>(xmlAnimation.Easing.ToString(), out easingmode))
            {
                var easingFunction = new QuadraticEase();
                easingFunction.EasingMode = easingmode;
                start = new EasingInt32KeyFrame(from, KeyTime.FromPercent(0), easingFunction);
                end = new EasingInt32KeyFrame(to, KeyTime.FromPercent(1.0), easingFunction);
            }
            else
            {
                start = new EasingInt32KeyFrame(from, KeyTime.FromPercent(0));
                end = new EasingInt32KeyFrame(to, KeyTime.FromPercent(1.0));
            }
            intanimation.KeyFrames.Add(start);
            intanimation.KeyFrames.Add(end);
            //doubleanimation.Freeze();
            return intanimation;
        }

        public double GetZoom(int value)
        {
            return Math.Max(0.0, ((double)value) / 100.0);
        }

        public double GetOpacity(int value)
        {
            return Math.Max(0.0, Math.Min(1.0, ((double)value) / 100.0));
        }

  

      

    }
   
    public enum AnimationType 
    { 
        Slide,
        Fade,
        Zoom,
        Rotate
    }
   
}