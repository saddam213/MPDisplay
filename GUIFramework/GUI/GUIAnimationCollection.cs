using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using GUIFramework.Managers;
using GUISkinFramework.Animations;

namespace GUIFramework.GUI
{
    public class AnimationCollection
    {
        private Dictionary<XmlAnimationCondition, GUIStoryboard> _animations = new Dictionary<XmlAnimationCondition, GUIStoryboard>();
        private FrameworkElement _element;
        private Action<XmlAnimationCondition> _startedCallback;
        private Action<XmlAnimationCondition> _completedCallback;

        public AnimationCollection(FrameworkElement element, IEnumerable<XmlAnimation> animations, Action<XmlAnimationCondition> animationStartedCallback, Action<XmlAnimationCondition> animationCompletedCallback)
        {
            _element = element;
            _startedCallback = animationStartedCallback;
            _completedCallback = animationCompletedCallback;
            if (_element != null)
            {
                foreach (XmlAnimationCondition condition in Enum.GetValues(typeof(XmlAnimationCondition)))
                {
                    if (condition != XmlAnimationCondition.None && !_animations.ContainsKey(condition))
                    {
                        _animations.Add(condition, GUIAnimationFactory.CreateAnimation(condition, _element, animations.Where(a => a.Condition == condition).OrderBy(x => x.Delay)));
                    }
                }
            }
        }

        public void StartAnimation(XmlAnimationCondition condition)
        {
            if (_element != null && _animations.ContainsKey(condition))
            {
                if (_animations[GetDependantAnimation(condition)] != null)
                {
                    _animations[GetDependantAnimation(condition)].OnAnimationComplete -= OnAnimationComplete;
                }

                if (_animations[condition] != null)
                {
                    _animations[condition].OnAnimationComplete -= OnAnimationComplete;
                }
              
                _startedCallback(condition);

                if (_animations[condition] != null)
                {
                    _animations[condition].OnAnimationComplete += OnAnimationComplete;
                    _animations[condition].Begin(_element, HandoffBehavior.SnapshotAndReplace, true);
                }
                else
                {
                    if (_completedCallback != null)
                    {
                        _completedCallback.Invoke(condition);
                    }
                }
            }
        }

        private void OnAnimationComplete(XmlAnimationCondition condition)
        {
            if (_element != null && _completedCallback != null)
            {
                if (_animations.ContainsKey(condition))
                {
                    _completedCallback.Invoke(condition);
                }
            }
        }


        private XmlAnimationCondition GetDependantAnimation(XmlAnimationCondition condition)
        {
            switch (condition)
            {
                case XmlAnimationCondition.WindowOpen:
                    return  XmlAnimationCondition.WindowClose;
                case XmlAnimationCondition.WindowClose:
                    return  XmlAnimationCondition.WindowOpen;
                case XmlAnimationCondition.TouchDown:
                    return  XmlAnimationCondition.TouchUp;
                case XmlAnimationCondition.TouchUp:
                    return  XmlAnimationCondition.TouchDown;
                case XmlAnimationCondition.VisibleTrue:
                    return  XmlAnimationCondition.VisibleFalse;
                case XmlAnimationCondition.VisibleFalse:
                    return  XmlAnimationCondition.VisibleTrue;
                case XmlAnimationCondition.FocusTrue:
                    return  XmlAnimationCondition.FocusFalse;
                case XmlAnimationCondition.FocusFalse:
                    return  XmlAnimationCondition.FocusTrue;
                case XmlAnimationCondition.PropertyChanging:
                    return  XmlAnimationCondition.PropertyChanged;
                case XmlAnimationCondition.PropertyChanged:
                    return  XmlAnimationCondition.PropertyChanging;
                default:
                    break;
            }
            return condition;
        }
    }

    public class GUIStoryboard : Storyboard
    {
        public XmlAnimationCondition Condition { get; set; }

        public delegate void AnimationComplete(XmlAnimationCondition condition);
        public event AnimationComplete OnAnimationComplete;

        public GUIStoryboard()
        {
            this.Completed += (s, e) =>
            {
                if (OnAnimationComplete != null)
                {
                    OnAnimationComplete(Condition);
                }
            };
        }
    }
}
