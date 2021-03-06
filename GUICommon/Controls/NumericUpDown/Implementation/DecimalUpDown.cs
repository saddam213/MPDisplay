﻿using System;
using System.Windows;

namespace MPDisplay.Common.Controls
{
    public class DecimalUpDown : NumericUpDown<decimal?>
    {
        #region Constructors

        static DecimalUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(typeof(DecimalUpDown)));
            DefaultValueProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(default(decimal)));
            IncrementProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(1m));
            MaximumProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(decimal.MaxValue));
            MinimumProperty.OverrideMetadata(typeof(DecimalUpDown), new FrameworkPropertyMetadata(decimal.MinValue));
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override decimal? CoerceValue(decimal? value)
        {
            if (value < Minimum)
                return Minimum;
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

        protected override decimal? ConvertTextToValue(string text)
        {
            decimal? result;

            if (String.IsNullOrEmpty(text))
                return null;

            try
            {
                result = FormatString.Contains("P") ? ParsePercent(text, CultureInfo) : ParseDecimal(text, CultureInfo);
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

        protected override void ValidateValue(decimal? value)
        {
            if (value < Minimum)
                Value = Minimum;
            else if (value > Maximum)
                Value = Maximum;
        }

        #endregion //Base Class Overrides
    }
}
