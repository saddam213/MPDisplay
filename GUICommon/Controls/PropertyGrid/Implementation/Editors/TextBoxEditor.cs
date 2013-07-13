﻿using System;
using System.Windows.Controls;

namespace MPDisplay.Common.Controls.PropertyGrid.Editors
{
    public class TextBoxEditor : TypeEditor<WatermarkTextBox>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }
    }
}
