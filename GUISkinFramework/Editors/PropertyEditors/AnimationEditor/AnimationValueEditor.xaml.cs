using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GUISkinFramework.Skin;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class AnimationValueEditor : UserControl, ITypeEditor
    {
        private PropertyItem _propertyItem;

        public AnimationValueEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(int), typeof(AnimationValueEditor)
            ,new PropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
           DependencyProperty.Register("MinValue", typeof(int), typeof(AnimationValueEditor), new PropertyMetadata(int.MinValue));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
           DependencyProperty.Register("MaxValue", typeof(int), typeof(AnimationValueEditor), new PropertyMetadata(int.MaxValue));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty IsSetValueVisibleProperty =
         DependencyProperty.Register("IsSetValueVisible", typeof(bool), typeof(AnimationValueEditor), new PropertyMetadata(false));

        public bool IsSetValueVisible
        {
            get { return (bool)GetValue(IsSetValueVisibleProperty); }
            set { SetValue(IsSetValueVisibleProperty, value); }
        }
     
      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var animationEditorDialog = _propertyItem.PropertyGrid.DataContext as AnimationEditorDialog;
            if (animationEditorDialog != null)
            {
                var animatedElement = animationEditorDialog.AnimatedElement as XmlControl;
           
                if (animatedElement != null)
                {
                    if (_propertyItem.Instance is XmlSlideAnimation)
                    {
                        var animation = _propertyItem.Instance is XmlSlideAnimation;
                        if (_propertyItem.Name.Equals("StartX") || _propertyItem.Name.Equals("EndX"))
                        {
                            Value = animatedElement.PosX;
                        }

                        if (_propertyItem.Name.Equals("StartY") || _propertyItem.Name.Equals("EndY"))
                        {
                            Value = animatedElement.PosY;
                        }

                        if (_propertyItem.Name.Equals("StartZ") || _propertyItem.Name.Equals("EndZ"))
                        {
                            Value = animatedElement.PosZ;
                        }
                    }

                    if (_propertyItem.Instance is XmlRotateAnimation)
                    {
                        if (_propertyItem.Name.Equals("Pos3DXFrom") || _propertyItem.Name.Equals("Pos3DXTo"))
                        {
                            Value = animatedElement.Pos3DX;
                        }

                        if (_propertyItem.Name.Equals("Pos3DYFrom") || _propertyItem.Name.Equals("Pos3DYTo"))
                        {
                            Value = animatedElement.Pos3DY;
                        }

                        if (_propertyItem.Name.Equals("Pos3DZFrom") || _propertyItem.Name.Equals("Pos3DZTo"))
                        {
                            Value = animatedElement.Pos3DZ;
                        }

                        if (_propertyItem.Name.Equals("Pos3DCenterXFrom") || _propertyItem.Name.Equals("Pos3DCenterXTo"))
                        {
                            Value = animatedElement.Center3DX;
                        }

                        if (_propertyItem.Name.Equals("Pos3DCenterYFrom") || _propertyItem.Name.Equals("Pos3DCenterYTo"))
                        {
                            Value = animatedElement.Center3DY;
                        }

                        if (_propertyItem.Name.Equals("Pos3DCenterZFrom") || _propertyItem.Name.Equals("Pos3DCenterZTo"))
                        {
                            Value = animatedElement.Center3DZ;
                        }
                    }
                }
            }
        }

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            Binding binding = new Binding("Value");
            binding.Source = propertyItem;
            binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(this, ValueProperty, binding);

            _propertyItem = propertyItem;
            if (propertyItem != null && propertyItem.Instance != null)
            {
                IsSetValueVisible = propertyItem.Instance is XmlSlideAnimation || propertyItem.Instance is XmlRotateAnimation;

                var propertyRange = propertyItem.Instance.GetType()
                    .GetProperty(propertyItem.BindingPath)
                    .GetCustomAttributes(typeof(PropertyRangeAttribute), true)
                    .FirstOrDefault() as PropertyRangeAttribute;
                if (propertyRange != null)
                {
                    MinValue = propertyRange.Min;
                    MaxValue = propertyRange.Max;
                }
            }
            return this;
        }


   
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PropertyRangeAttribute : Attribute
    {
        protected int _min;
        protected int _max;
      
        public PropertyRangeAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public int Min
        {
            get { return _min; }
        }

        public int Max
        {
            get { return _max; }
        }
    }
}
