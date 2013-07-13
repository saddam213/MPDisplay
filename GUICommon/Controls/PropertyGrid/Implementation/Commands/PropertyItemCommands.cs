﻿using System;
using System.Windows.Input;

namespace MPDisplay.Common.Controls.PropertyGrid.Commands
{
    public static class PropertyItemCommands
    {
        private static RoutedCommand _resetValueCommand = new RoutedCommand();
        public static RoutedCommand ResetValue
        {
            get
            {
                return _resetValueCommand;
            }
        }
    }
}
