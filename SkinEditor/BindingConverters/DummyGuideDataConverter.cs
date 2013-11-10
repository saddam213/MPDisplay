using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Common.Helpers;
using GUIFramework.Managers;
using GUISkinFramework;

namespace SkinEditor.BindingConverters
{
    public class DummyGuideDataConverter : IValueConverter
    {

        private double GetItemWidth(DateTime startTime, DateTime endTime, double multi)
        {
            return (endTime - startTime).TotalMinutes * multi;
        }

        private double GetStartPoint(DateTime startTime, DateTime guideStart, double multi)
        {
            return ((startTime - guideStart).TotalMinutes * multi);
        }

        private static List<TvGuideChannel> _guideData;


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_guideData == null)
            {
                  var dummyItemPath = Environment.CurrentDirectory + "\\Data\\GuideData.xml";
                  _guideData = SerializationHelper.Deserialize<List<TvGuideChannel>>(dummyItemPath);
            }

        
            if (parameter.ToString() == "Timeline")
            {
              
            }
            if (parameter.ToString() == "TimelineInfo")
            {
            
            }
            if (parameter.ToString() == "TimelineLength")
            {
             
            }
            if (parameter.ToString() == "TimelineCenterPosition")
            {
              
            }

            if (parameter.ToString() == "ChannelData")
            {
                return _guideData;
            }


            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



     

    }


    public class DummyGuideDataValueConverter : IMultiValueConverter
    {

        private double GetItemWidth(DateTime startTime, DateTime endTime, double multi)
        {
            return (endTime - startTime).TotalMinutes * multi;
        }

        private double GetStartPoint(DateTime startTime, DateTime guideStart, double multi)
        {
            return ((startTime - guideStart).TotalMinutes * multi);
        }

        private static List<TvGuideChannel> _guideData;


        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_guideData == null)
            {
                var dummyItemPath = Environment.CurrentDirectory + "\\Data\\GuideData.xml";
                _guideData = SerializationHelper.Deserialize<List<TvGuideChannel>>(dummyItemPath);
            }


            if (parameter.ToString() == "ProgramWidth")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetItemWidth(program.StartTime, program.EndTime, (double)values[2]);
                    //}
                }
            }
            if (parameter.ToString() == "ProgramPosition")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    //}
                }
            }
            if (parameter.ToString() == "Timeline")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    //}
                }
            }
            if (parameter.ToString() == "TimelineInfo")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    //}
                }
            }
            if (parameter.ToString() == "TimelineLength")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    //}
                }
            }
            if (parameter.ToString() == "TimelineCenterPosition")
            {
                if (values != null && values.Count() == 3)
                {

                    //if (values[0] is TvGuideProgram)
                    //{
                    //    var program = values[0] as TvGuideProgram;
                    //    return GetStartPoint(program.StartTime, (DateTime)values[1], (double)values[2]);
                    //}
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
