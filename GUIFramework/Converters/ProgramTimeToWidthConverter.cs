using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GUIFramework.Repositories;

namespace GUIFramework.Converters
{
    public class ProgramTimeToWidthConverter : IMultiValueConverter
    {
        private static double GetItemWidth(DateTime startTime, DateTime endTime, double multi)
        {
            return (endTime - startTime).TotalMinutes * multi;
        }

        private static double GetStartPoint(DateTime startTime, DateTime guideStart, double multi)
        {
            return (startTime - guideStart).TotalMinutes * multi;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter.ToString() == "ProgramWidth")
            {
                if (values != null && values.Count(d => d != DependencyProperty.UnsetValue) == 3)
                {
                    var guideProgram = values[0] as TvGuideProgram;
                    if (guideProgram != null)
                    {
                        var program = guideProgram;
                        return GetItemWidth(program.StartTime, program.EndTime, (double)values[2]);
                    }
                }
            }
            if (parameter.ToString() != "ProgramPosition") return 0.0;

            if (values == null || values.Count(d => d != DependencyProperty.UnsetValue) != 3) return 0.0;
            var guideProgram1 = values[0] as TvGuideProgram;
            if (guideProgram1 == null) return 0.0;
            var program1 = guideProgram1;
            return GetStartPoint(program1.StartTime, (DateTime)values[1], (double)values[2]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
