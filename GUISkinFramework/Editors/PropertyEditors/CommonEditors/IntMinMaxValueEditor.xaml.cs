﻿using System.Linq;
using System.Windows;
using System.Windows.Data;
using Common.Helpers;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class IntMinMaxValueEditor : ITypeEditor
    {
        private PropertyItem _propertyItem;

        public IntMinMaxValueEditor()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(IntMinMaxValueEditor), new PropertyMetadata(0, Test));

        private static void Test(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
           DependencyProperty.Register("MinValue", typeof(int), typeof(IntMinMaxValueEditor), new PropertyMetadata(int.MinValue));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
           DependencyProperty.Register("MaxValue", typeof(int), typeof(IntMinMaxValueEditor), new PropertyMetadata(int.MaxValue));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }


        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _propertyItem = propertyItem;

            if (_propertyItem?.Instance == null) return this;
            var propertyRange = ReflectionHelper.GetPropertyPath(_propertyItem.Instance, _propertyItem.BindingPath)
                .GetCustomAttributes(typeof(PropertyRangeAttribute), true)
                .FirstOrDefault() as PropertyRangeAttribute;
            if (propertyRange != null)
            {
                MinValue = propertyRange.Min;
                MaxValue = propertyRange.Max;
            }

            var binding = new Binding("Value") {Source = _propertyItem, Mode = BindingMode.TwoWay};
            BindingOperations.SetBinding(this, ValueProperty, binding);

            return this;
        }
    }
}
