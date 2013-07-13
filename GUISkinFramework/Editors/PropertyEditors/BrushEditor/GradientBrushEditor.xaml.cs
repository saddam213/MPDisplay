using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Editor.PropertyEditors;

namespace GUISkinFramework.Editors.PropertyEditors
{
    /// <summary>
    /// Interaction logic for GradientBrushEditor.xaml
    /// </summary>
    public partial class GradientBrushEditor : UserControl, INotifyPropertyChanged
    {
        public GradientBrushEditor()
        {
            InitializeComponent();
        }

        public delegate void GradientBrushChangedEvent(XmlGradientBrush brush);
        public event GradientBrushChangedEvent OnGradientBrushChanged;

      
        private XmlGradientStop _selectedGradientStop;
       




public XmlGradientBrush GradientBrush
{
    get { return (XmlGradientBrush)GetValue(GradientBrushProperty); }
    set { SetValue(GradientBrushProperty, value); }
} 
         
// Using a DependencyProperty as the backing store for GradientBrush.  This enables animation, styling, binding, etc...
public static readonly DependencyProperty GradientBrushProperty = 
    DependencyProperty.Register("GradientBrush", typeof(XmlGradientBrush), typeof(GradientBrushEditor), new PropertyMetadata(new XmlGradientBrush(), new PropertyChangedCallback(OnBrushChanged)));

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var _value = e.NewValue as XmlGradientBrush;
            var _this = d as GradientBrushEditor;
            _this.SetBackgroundValues(_value);
        }

        private void SetBackgroundValues(XmlGradientBrush value)
        {
            if (value != null)
            {
                //NotifyPropertyChanged("GradientStartPoint");
                //NotifyPropertyChanged("GradientEndPoint");
                NotifyPropertyChanged("SelectedGradientAngle");
            }
        }

        public XmlGradientStop SelectedGradientStop  
        {
            get { return _selectedGradientStop; }
            set { _selectedGradientStop = value; NotifyPropertyChanged("SelectedGradientStop"); }
        }

       // private XmlGradientAngle _gradientBrushAngle;

        public XmlGradientAngle GradientBrushAngle
        {
            get { return GradientBrush.Angle; }
            set { GradientBrush.Angle = value; NotifyPropertyChanged("GradientBrushAngle"); UpdateDisplayBrush(); }
        }
        

        //public GradientAngle SelectedGradientAngle
        //{
        //    get
        //    {
        //        if (GradientBrush.StartPoint.ToPoint() == new Point(0, 0.5) && GradientBrush.EndPoint.ToPoint() == new Point(1, 0.5))
        //        {
        //            return GradientAngle.Horizontal;
        //        }
        //        else if (GradientBrush.StartPoint.ToPoint() == new Point(0.5, 0) && GradientBrush.EndPoint.ToPoint() == new Point(0.5, 1))
        //        {
        //            return GradientAngle.Vertical;
        //        }
        //        return GradientAngle.Custom;
        //    }
        //    set
        //    {
        //        switch (value)
        //        {
        //            case GradientAngle.Vertical:
        //                GradientBrush.StartPoint = new Point(0.5, 0).ToXmlString();
        //                GradientBrush.EndPoint = new Point(0.5, 1).ToXmlString();
        //                break;
        //            case GradientAngle.Horizontal:
        //                GradientBrush.StartPoint = new Point(0, 0.5).ToXmlString();
        //                GradientBrush.EndPoint = new Point(1, 0.5).ToXmlString();
        //                break;
        //            case GradientAngle.Custom:
        //                GradientBrush.StartPoint = new Point().ToXmlString();
        //                GradientBrush.EndPoint = new Point().ToXmlString();
        //                break;
        //            default:
        //                break;
        //        }
        //        NotifyPropertyChanged("SelectedGradientAngle");
        //        NotifyPropertyChanged("GradientStartPoint");
        //        NotifyPropertyChanged("GradientEndPoint");
        //        UpdateDisplayBrush();
        //    }
        //}


        //public Point GradientStartPoint
        //{
        //    get { return GradientBrush.StartPoint.ToPoint(); }
        //    set
        //    {
        //        GradientBrush.StartPoint =  value.ToXmlString();
        //        NotifyPropertyChanged("GradientStartPoint");
        //    }
        //}

        //public Point GradientEndPoint
        //{
        //    get { return GradientBrush.EndPoint.ToPoint(); }
        //    set
        //    {
        //        GradientBrush.EndPoint = value.ToXmlString();
        //        NotifyPropertyChanged("GradientEndPoint");
        //    }
        //}

        //private Point GetStartPoint()
        //{
        //    switch (SelectedGradientAngle)
        //    {
        //        case GradientAngle.Vertical:
        //            return new Point(0.5, 0);
        //        case GradientAngle.Horizontal:
        //            return new Point(0, 0.5);
        //        case GradientAngle.Custom:
        //            return GradientStartPoint;
        //        default:
        //            break;
        //    }
        //    return new Point();
        //}

        //private Point GetEndPoint()
        //{
        //    switch (SelectedGradientAngle)
        //    {
        //        case GradientAngle.Vertical:
        //            return new Point(0.5, 1);
        //        case GradientAngle.Horizontal:
        //            return new Point(1, 0.5);
        //        case GradientAngle.Custom:
        //            return GradientEndPoint;
        //        default:
        //            break;
        //    }
        //    return new Point();
        //}

        //private GradientAngle GetGradientAngle(string points)
        //{

        //    if (points == "0,0.51,0.5")
        //    {
        //        return GradientAngle.Horizontal;
        //    }
        //    else if (points == "0.5,00.5,1")
        //    {
        //        return GradientAngle.Vertical;
        //    }
        //    return GradientAngle.Custom;
        //}

        private void GradientStopAdd_Click(object sender, RoutedEventArgs e)
        {
            GradientBrush.GradientStops.Add(new XmlGradientStop{ Color = "Transparent", Offset = 1});
            SelectedGradientStop = GradientBrush.GradientStops.Last();
            UpdateDisplayBrush();
        }

        private void GradientStopRemove_Click(object sender, RoutedEventArgs e)
        {
            GradientBrush.GradientStops.Remove(SelectedGradientStop);
            SelectedGradientStop = GradientBrush.GradientStops.Count > 0 ? GradientBrush.GradientStops.Last() : null;
            UpdateDisplayBrush();
        }

        private void GradientStopOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateDisplayBrush();
        }

      

        private void UpdateDisplayBrush()
        {
            if (OnGradientBrushChanged != null)
            {
                OnGradientBrushChanged(GradientBrush);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void ColorPicker_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            UpdateDisplayBrush();
        }
      
    }
}
