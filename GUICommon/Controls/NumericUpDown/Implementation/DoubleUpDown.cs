using System;
using System.Windows;

namespace MPDisplay.Common.Controls
{
    public class DoubleUpDown : NumericUpDown<double?>
    {
        #region Constructors

        static DoubleUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleUpDown), new FrameworkPropertyMetadata(typeof(DoubleUpDown)));
            DefaultValueProperty.OverrideMetadata(typeof(DoubleUpDown), new FrameworkPropertyMetadata(default(double)));
            IncrementProperty.OverrideMetadata(typeof(DoubleUpDown), new FrameworkPropertyMetadata(1d));
            MaximumProperty.OverrideMetadata(typeof(DoubleUpDown), new FrameworkPropertyMetadata(double.MaxValue));
            MinimumProperty.OverrideMetadata(typeof(DoubleUpDown), new FrameworkPropertyMetadata(double.MinValue));
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override double? CoerceValue(double? value)
        {
            if (value < Minimum) return Minimum;

            return value > Maximum ? Maximum : value;
        }

        protected override void OnIncrement()
        {
            if (Value.HasValue)
                Value += Increment;
            else
                Value = DefaultValue;
        }

        protected override void OnDecrement()
        {
            if (Value.HasValue)
                Value -= Increment;
            else
                Value = DefaultValue;
        }

        protected override double? ConvertTextToValue(string text)
        {
            double? result;

            if (String.IsNullOrEmpty(text))
                return null;

            try
            {
                result = FormatString.Contains("P") ? Decimal.ToDouble(ParsePercent(text, CultureInfo)) : ParseDouble(text, CultureInfo);
                result = CoerceValue(result);
            }
            catch
            {
                Text = ConvertValueToText();
                return Value;
            }

            return result;
        }

        protected override string ConvertValueToText()
        {
            return Value?.ToString(FormatString, CultureInfo) ?? string.Empty;
        }

        protected override void SetValidSpinDirection()
        {
            var validDirections = ValidSpinDirections.None;

            if (Value < Maximum || !Value.HasValue)
                validDirections = validDirections | ValidSpinDirections.Increase;

            if (Value > Minimum || !Value.HasValue)
                validDirections = validDirections | ValidSpinDirections.Decrease;

            if (Spinner != null)
                Spinner.ValidSpinDirection = validDirections;
        }

        protected override void ValidateValue(double? value)
        {
            if (value < Minimum)
                Value = Minimum;
            else if (value > Maximum)
                Value = Maximum;
        }

        #endregion //Base Class Overrides
    }
}
