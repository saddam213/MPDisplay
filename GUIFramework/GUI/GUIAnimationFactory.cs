using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GUIFramework.GUI;
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using MPDisplay.Common.Controls;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Helper class to create GUIStoryboards from XmlAnimations
    /// </summary>
    public static class GUIAnimationFactory
    {
        /// <summary>
        /// Creates the animation.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="element">The element.</param>
        /// <param name="animations">The animations.</param>
        /// <returns></returns>
        public static GUIStoryboard CreateAnimation(XmlAnimationCondition condition, FrameworkElement element, IEnumerable<XmlAnimation> animations)
        {
            var storyboard = new GUIStoryboard { Condition = condition };
            if (animations != null && animations.Any())
            {
                foreach (var animation in animations.OfType<XmlSlideAnimation>())
                {
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.StartX, animation.EndX, animation), new PropertyPath(Canvas.LeftProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.StartY, animation.EndY, animation), new PropertyPath("(Canvas.Top)"));
                    storyboard.AddAnimation(element, CreateIntAnimation(animation.StartZ, animation.EndZ, animation), new PropertyPath(Canvas.ZIndexProperty));
                }
                foreach (var animation in animations.OfType<XmlFadeAnimation>())
                {
                    storyboard.AddAnimation(element, CreateDoubleAnimation(GetOpacity(animation.From), GetOpacity( animation.To), animation), new PropertyPath(FrameworkElement.OpacityProperty));
                }
                foreach (var animation in animations.OfType<XmlZoomAnimation>())
                {
                    storyboard.AddAnimation(element, CreateDoubleAnimation(GetZoom(animation.From), GetZoom(animation.To), animation), new PropertyPath("RenderTransform.ScaleX"));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(GetZoom(animation.From), GetZoom(animation.To), animation), new PropertyPath("RenderTransform.ScaleY"));
                }
                foreach (var animation in animations.OfType<XmlRotateAnimation>())
                {
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DXFrom, animation.Pos3DXTo, animation), new PropertyPath(Surface3D.RotationXProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DYFrom, animation.Pos3DYTo, animation), new PropertyPath(Surface3D.RotationYProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DZFrom, animation.Pos3DZTo, animation), new PropertyPath(Surface3D.RotationZProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DCenterXFrom, animation.Pos3DCenterXTo, animation), new PropertyPath(Surface3D.RotationCenterXProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DCenterYFrom, animation.Pos3DCenterYTo, animation), new PropertyPath(Surface3D.RotationCenterYProperty));
                    storyboard.AddAnimation(element, CreateDoubleAnimation(animation.Pos3DCenterZFrom, animation.Pos3DCenterZTo, animation), new PropertyPath(Surface3D.RotationCenterZProperty));
                }
                return storyboard;
            }
         
            return null;
        }
       
        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="storyboard">The storyboard.</param>
        /// <param name="element">The element.</param>
        /// <param name="animation">The animation.</param>
        /// <param name="propertyTarget">The property target.</param>
        private static void AddAnimation(this GUIStoryboard storyboard, FrameworkElement element, Timeline animation, PropertyPath propertyTarget)
        {
            storyboard.Children.Add(animation);
            GUIStoryboard.SetTarget(animation, element);
            GUIStoryboard.SetTargetProperty(animation, propertyTarget);
        }

        /// <summary>
        /// Creates the double animation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="xmlAnimation">The XML animation.</param>
        /// <returns></returns>
        private static DoubleAnimationUsingKeyFrames CreateDoubleAnimation(double from, double to, XmlAnimation xmlAnimation)
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

        /// <summary>
        /// Creates the int animation.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="xmlAnimation">The XML animation.</param>
        /// <returns></returns>
        private static Int32AnimationUsingKeyFrames CreateIntAnimation(int from, int to, XmlAnimation xmlAnimation)
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

        /// <summary>
        /// Gets the zoom.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static double GetZoom(int value)
        {
            return Math.Max(0.0, ((double)value) / 100.0);
        }

        /// <summary>
        /// Gets the opacity.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static double GetOpacity(int value)
        {
            return Math.Max(0.0, Math.Min(1.0, ((double)value) / 100.0));
        } 
    }
}
