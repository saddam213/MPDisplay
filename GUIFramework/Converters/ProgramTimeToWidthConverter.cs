using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GUIFramework.Managers;

namespace GUIFramework.Converters
{
    public class ProgramTimeToWidthConverter : IMultiValueConverter
    {
        private double GetItemWidth(DateTime startTime, DateTime endTime, double multi)
        {
            return (endTime - startTime).TotalMinutes * multi;
        }

        private double GetStartPoint(DateTime startTime, DateTime guideStart, double multi)
        {
            return ((startTime - guideStart).TotalMinutes * multi);
        }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter.ToString() == "ProgramWidth")
            {
                if (values != null && values.Count(d => d != DependencyProperty.UnsetValue) == 3)
                {

                    if (values[0] is TvGuideProgram)
                    {
                        var program = values[0] as TvGuideProgram;
                        return GetItemWidth(program.StartTime, program.EndTime, (double)values[2]);
                    }
                }
            }
            if (parameter.ToString() == "ProgramPosition")
            {
                if (values != null && values.Count(d => d != DependencyProperty.UnsetValue) == 3)
                {

                    if (values[0] is TvGuideProgram)
                    {
                        var program = values[0] as TvGuideProgram;
                        return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    }
                }
            }
            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
