using System;
using System.Windows.Input;

namespace MPDisplay.Common.Controls.PropertyGrid.Commands
{
    public class PropertyGridCommands
    {
        private static RoutedCommand _clearFilterCommand = new RoutedCommand();
        public static RoutedCommand ClearFilter
        {
            get
            {
                return _clearFilterCommand;
            }
        }
    }
}
