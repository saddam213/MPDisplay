﻿using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GUISkinFramework.ExtensionMethods;
using MPDisplay.Common.Controls.PropertyGrid;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for BrushEditor.xaml
    /// </summary>
    public partial class FourPointValueEditor : ITypeEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FourPointValueEditor"/> class.
        /// </summary>
        public FourPointValueEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The value property
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(FourPointValueEditor),
                               new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Resolves the editor.
        /// </summary>
        /// <param name="propertyItem">The property item.</param>
        /// <returns></returns>
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var binding = new Binding("Value")
            {
                Source = propertyItem,
                Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, ValueProperty, binding);
            return this;
        }

        /// <summary>
        /// Called when the Editor has lost focuse to format a single value into a 4 point value just for consitancy
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ValueTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var singleValue = 0;
            if (string.IsNullOrEmpty(Value) || int.TryParse(Value, out singleValue))
            {
                Value = string.Format("{0},{0},{0},{0}", singleValue);
            }
            Value = Value.Replace(" ","");
        }
    }

    /// <summary>
    /// Validation class to give the user feedback on the data entered in the TextBox
    /// </summary>
    public class FourPointValidationRule : ValidationRule
    {
        private const string Message = "Value must declare a single numeric value or 4(Left, Top, Right, Bottom) numeric values in format: X,X,X,X";

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var fourPointString = value as string;
            if (string.IsNullOrEmpty(fourPointString)) return new ValidationResult(false, Message);

            if (fourPointString.Contains(','))
            {
                if (fourPointString.Count(c => c == ',') != 3 || !fourPointString.Split(',').All(s => s.IsNumber()))
                {
                    return new ValidationResult(false, Message);
                }
            }
            else if (!fourPointString.IsNumber())
            {
                return new ValidationResult(false, Message);
            }
            return new ValidationResult(true, null);
        }
    }

}
