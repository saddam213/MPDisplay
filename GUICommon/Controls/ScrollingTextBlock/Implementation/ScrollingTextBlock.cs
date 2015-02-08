using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace MPDisplay.Common.Controls
{
    public class ScrollingTextBlock : Control
    {
        private bool _isReady = false;
        private TextBlock _mainTextBlock;
        private TextBlock _measureLabel;

        static ScrollingTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollingTextBlock), new FrameworkPropertyMetadata(typeof(ScrollingTextBlock)));
        }

        public ScrollingTextBlock()
        {
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ScrollingTextBlock), new PropertyMetadata(string.Empty, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty IsScrollingEnabledProperty = DependencyProperty.Register("IsScrollingEnabled", typeof(bool), typeof(ScrollingTextBlock), new PropertyMetadata(false, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty IsVerticalProperty = DependencyProperty.Register("IsVertical", typeof(bool), typeof(ScrollingTextBlock), new PropertyMetadata(false, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty ScrollDelayProperty = DependencyProperty.Register("ScrollDelay", typeof(int), typeof(ScrollingTextBlock), new PropertyMetadata(3, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty ScrollSpeedProperty = DependencyProperty.Register("ScrollSpeed", typeof(int), typeof(ScrollingTextBlock), new PropertyMetadata(5, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register("IsScrolling", typeof(bool), typeof(ScrollingTextBlock), new PropertyMetadata(false));
        public static readonly DependencyProperty IsWrapEnabledProperty = DependencyProperty.Register("IsWrapEnabled", typeof(bool), typeof(ScrollingTextBlock), new PropertyMetadata(true, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty ScrollSeperatorProperty = DependencyProperty.Register("ScrollSeperator", typeof(string), typeof(ScrollingTextBlock), new PropertyMetadata(string.Empty, (d, e) => (d as ScrollingTextBlock).Reset()));
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(ScrollingTextBlock), new PropertyMetadata(TextAlignment.Left, (d, e) => (d as ScrollingTextBlock).Reset()));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsScrollingEnabled
        {
            get { return (bool)GetValue(IsScrollingEnabledProperty); }
            set { SetValue(IsScrollingEnabledProperty, value); }
        }

        public bool IsVertical
        {
            get { return (bool)GetValue(IsVerticalProperty); }
            set { SetValue(IsVerticalProperty, value); }
        }
        
        public int ScrollDelay
        {
            get { return (int)GetValue(ScrollDelayProperty); }
            set { SetValue(ScrollDelayProperty, value); }
        }

        public int ScrollSpeed
        {
            get { return (int)GetValue(ScrollSpeedProperty); }
            set { SetValue(ScrollSpeedProperty, value); }
        }

        public bool IsScrolling
        {
            get { return (bool)GetValue(IsScrollingProperty); }
            set { SetValue(IsScrollingProperty, value); }
        }

        public bool IsWrapEnabled
        {
            get { return (bool)GetValue(IsWrapEnabledProperty); }
            set { SetValue(IsWrapEnabledProperty, value); }
        }

        public string ScrollSeperator
        {
            get { return (string)GetValue(ScrollSeperatorProperty); }
            set { SetValue(ScrollSeperatorProperty, value); } 
        }

        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }
    
        public override void OnApplyTemplate()
        {
            _mainTextBlock = GetTemplateChild("PART_mainTextBlock") as TextBlock;
            _measureLabel = GetTemplateChild("PART_measureTextBlock") as TextBlock;
            this.SizeChanged += (s, e) => { Reset(); };
            _isReady = true;
            Reset();
        }

        private void SetText(string text)
        {
            if (_mainTextBlock != null)
            {
                _mainTextBlock.BeginAnimation(Canvas.LeftProperty, null);
                _mainTextBlock.BeginAnimation(Canvas.TopProperty, null);
                _mainTextBlock.Width = Width;
                _mainTextBlock.Height = Height;
                if (IsScrollingEnabled)
                {
                    if (!IsVertical)
                    {
                       
                        var textWidth = GetTextWidth(text);
                        if (textWidth > Width)
                        {
                            IsScrolling = true;
                            _mainTextBlock.Width = double.NaN;
                            _mainTextBlock.TextAlignment = TextAlignment.Left;
                            _mainTextBlock.Text = GetScrollingText();
                            var textLength = GetScrollingLength();
                            var animation = new DoubleAnimation(0.0, -textLength, new Duration(TimeSpan.FromSeconds((textLength / 5))));
                            animation.SpeedRatio = ScrollSpeed;
                            animation.BeginTime = TimeSpan.FromSeconds(ScrollDelay);
                            animation.FillBehavior = FillBehavior.Stop;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            if (!IsWrapEnabled)
                            {
                                animation.RepeatBehavior = new RepeatBehavior(1);
                                animation.Completed += (s, e) => { SetText(Text); };
                            }
                            _mainTextBlock.BeginAnimation(Canvas.LeftProperty, animation);
                            return;
                        }
                    }
                    else
                    {
                     
                        var textHeight = GetTextHeight(text);
                        if (textHeight > Height)
                        {
                            IsScrolling = true;
                            _mainTextBlock.Height = double.NaN;
                            _mainTextBlock.TextAlignment = TextAlignment;
                            _mainTextBlock.Text = GetScrollingText();
                            var textLength = GetScrollingLength();
                            var animation = new DoubleAnimation(0.0, -textLength, new Duration(TimeSpan.FromSeconds((textLength / 5))));
                            animation.SpeedRatio = ScrollSpeed;
                            animation.BeginTime = TimeSpan.FromSeconds(ScrollDelay);
                            animation.FillBehavior = FillBehavior.Stop;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            if (!IsWrapEnabled)
                            {
                                animation.RepeatBehavior = new RepeatBehavior(1);
                                animation.Completed += (s, e) => { SetText(Text); };
                            }
                            _mainTextBlock.BeginAnimation(Canvas.TopProperty, animation);
                            return;
                        }
                    }
                }
              
                _mainTextBlock.TextAlignment = TextAlignment;
                _mainTextBlock.Text = Text;
                IsScrolling = false;
            }
        }

        private string GetScrollingText()
        {
            if (IsWrapEnabled)
            {
                if (IsVertical)
                {
                    return string.Concat(Text, Environment.NewLine, ScrollSeperator, Environment.NewLine, Text);
                }
                return string.Concat(Text, ScrollSeperator, Text);
            }
            return Text;
        }

        private double GetScrollingLength()
        {
            if (IsWrapEnabled)
            {
                if (IsVertical)
                {
                    return GetTextHeight(string.Concat(Text, Environment.NewLine, ScrollSeperator));
                }
                return GetTextWidth(Text + ScrollSeperator);
            }

            if (IsVertical)
            {
                return GetTextHeight(Text);
            }

            return GetTextWidth(Text);
        }

     

        private void Reset()
        {
            if (_isReady)
            {
                _mainTextBlock.TextAlignment = TextAlignment;
                _mainTextBlock.MaxWidth = double.PositiveInfinity;
                _measureLabel.MaxWidth = double.PositiveInfinity;
                _mainTextBlock.TextWrapping = TextWrapping.NoWrap;
                _measureLabel.TextWrapping = TextWrapping.NoWrap;
                if (IsVertical)
                {
                    _mainTextBlock.TextWrapping = TextWrapping.Wrap;
                    _measureLabel.TextWrapping = TextWrapping.Wrap;
                    _mainTextBlock.MaxWidth = ActualWidth;
                    _measureLabel.MaxWidth = ActualWidth;
                }
                SetText(Text);
            }
        }

        private double GetTextWidth(string text)
        {
            try
            {
                _measureLabel.Text = text;
                double value = 0.0;
                Dispatcher.Invoke((Action)delegate { value = _measureLabel.ActualWidth; }, DispatcherPriority.Background);
                return value;
            }
            catch { }
            return 0.0;
        }

        private double GetTextHeight(string text)
        {
            try
            {
                _measureLabel.Text = text;
                double value = 0.0;
                Dispatcher.Invoke((Action)delegate { value = _measureLabel.ActualHeight; }, DispatcherPriority.Background);
                return value;
            }
            catch { }
            return 0.0;
        }
    }
}
