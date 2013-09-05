﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Styles;
using GUISkinFramework;
using GUIFramework.Managers;
using System.Windows;

namespace GUIFramework.Converters
{
    public class GUIBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is XmlColorBrush)
            {
                return new SolidColorBrush((value as XmlColorBrush).Color.ToColor());
            }
            else if (value is XmlGradientBrush)
            {
                var background = value as XmlGradientBrush;
                var gradient = new LinearGradientBrush();
                //gradient.StartPoint = background.StartPoint.ToPoint();
                //gradient.EndPoint = background.EndPoint.ToPoint();
                switch (background.Angle)
                {
                    case XmlGradientAngle.Vertical:
                        gradient.StartPoint = new Point(0.5, 0);
                        gradient.EndPoint = new Point(0.5, 1);
                        break;
                    case XmlGradientAngle.Horizontal:
                        gradient.StartPoint = new Point(0, 0.5);
                        gradient.EndPoint = new Point(1, 0.5);
                        break;
                    default:
                        break;
                }
            

                foreach (var stop in background.GradientStops)
                {
                    gradient.GradientStops.Add(new GradientStop(stop.Color.ToColor(), stop.Offset));
                }
                return gradient;
            }
            else if (value is XmlImageBrush)
            {
                var background = value as XmlImageBrush;
            
                    return GUIImageManager.GetImage(background);
             
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}