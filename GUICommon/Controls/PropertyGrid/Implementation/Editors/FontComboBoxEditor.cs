﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class FontComboBoxEditor : ComboBoxEditor
    {
        protected override IList<object> CreateItemsSource(PropertyItem propertyItem)
        {
            if (propertyItem.PropertyType == typeof(FontFamily) || propertyItem.Name.Contains("FontType"))
                return GetFontFamilies();
            if (propertyItem.PropertyType == typeof(FontWeight) || propertyItem.Name.Contains("FontWeight"))
                return GetFontWeights();
            if (propertyItem.PropertyType == typeof(FontStyle))
                return GetFontStyles();
            return propertyItem.PropertyType == typeof(FontStretch) ? GetFontStretches() : null;
        }

        private static IList<object> GetFontFamilies()
        {

            return Fonts.SystemFontFamilies.Select(x => x.Source).Cast<object>().ToList();

        }

        private static IList<object> GetFontWeights()
        {
            return new List<object>
            {
                FontWeights.Black.ToString(), 
                FontWeights.Bold.ToString(), 
                FontWeights.ExtraBlack.ToString(), 
                FontWeights.ExtraBold.ToString(),
                FontWeights.ExtraLight.ToString(), 
                FontWeights.Light.ToString(), 
                FontWeights.Medium.ToString(), 
                FontWeights.Normal.ToString(), 
                FontWeights.SemiBold.ToString(),
                FontWeights.Thin.ToString()
            };
        }

        private static IList<object> GetFontStyles()
        {
            return new List<object>
            {
                FontStyles.Italic,
                FontStyles.Normal
            };
        }

        private static IList<object> GetFontStretches()
        {
            return new List<object>
            {
                FontStretches.Condensed,
                FontStretches.Expanded,
                FontStretches.ExtraCondensed,
                FontStretches.ExtraExpanded,
                FontStretches.Normal,
                FontStretches.SemiCondensed,
                FontStretches.SemiExpanded,
                FontStretches.UltraCondensed,
                FontStretches.UltraExpanded
            };
        }
    }
}
