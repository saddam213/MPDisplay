using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Holds a collection of animations
    /// </summary>
    public class AnimationCollection
    {
        #region Fields

        private Dictionary<XmlAnimationCondition, GUIStoryboard> _animations = new Dictionary<XmlAnimationCondition, GUIStoryboard>();
        private FrameworkElement _element;
        private Action<XmlAnimationCondition> _startedCallback;
        private Action<XmlAnimationCondition> _completedCallback; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationCollection"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="animations">The animations.</param>
        /// <param name="animationStartedCallback">The animation started callback.</param>
        /// <param name="animationCompletedCallback">The animation completed callback.</param>
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
                        var xmlAnimations = animations as IList<XmlAnimation> ?? animations.ToList();
                        _animations.Add(condition, GUIAnimationFactory.CreateAnimation(condition, _element, xmlAnimations.Where(a => a.Condition == condition).OrderBy(x => x.Delay)));
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="condition">The condition.</param>
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
                        _completedCallback(condition);
                    }
                }
            }
        }

        /// <summary>
        /// Called when animation completes.
        /// </summary>
        /// <param name="condition">The condition.</param>
        private void OnAnimationComplete(XmlAnimationCondition condition)
        {
            if (_element != null && _completedCallback != null)
            {
                if (_animations.ContainsKey(condition))
                {
                    _completedCallback(condition);
                }
            }
        }

        /// <summary>
        /// Gets the dependant animation.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        private XmlAnimationCondition GetDependantAnimation(XmlAnimationCondition condition)
        {
            switch (condition)
            {
                case XmlAnimationCondition.WindowOpen:
                    return XmlAnimationCondition.WindowClose;
                case XmlAnimationCondition.WindowClose:
                    return XmlAnimationCondition.WindowOpen;
                case XmlAnimationCondition.TouchDown:
                    return XmlAnimationCondition.TouchUp;
                case XmlAnimationCondition.TouchUp:
                    return XmlAnimationCondition.TouchDown;
                case XmlAnimationCondition.VisibleTrue:
                    return XmlAnimationCondition.VisibleFalse;
                case XmlAnimationCondition.VisibleFalse:
                    return XmlAnimationCondition.VisibleTrue;
                case XmlAnimationCondition.FocusTrue:
                    return XmlAnimationCondition.FocusFalse;
                case XmlAnimationCondition.FocusFalse:
                    return XmlAnimationCondition.FocusTrue;
                case XmlAnimationCondition.PropertyChanging:
                    return XmlAnimationCondition.PropertyChanged;
                case XmlAnimationCondition.PropertyChanged:
                    return XmlAnimationCondition.PropertyChanging;
            }
            return condition;
        } 

        #endregion
    }
}
