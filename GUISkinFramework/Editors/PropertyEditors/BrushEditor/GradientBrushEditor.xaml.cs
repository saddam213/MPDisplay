using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GUISkinFramework.Skin;

namespace GUISkinFramework.Editors
{
    /// <summary>
    /// Interaction logic for GradientBrushEditor.xaml
    /// </summary>
    public partial class GradientBrushEditor : INotifyPropertyChanged
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
    DependencyProperty.Register("GradientBrush", typeof(XmlGradientBrush), typeof(GradientBrushEditor), new PropertyMetadata(new XmlGradientBrush(), OnBrushChanged));

        private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = e.NewValue as XmlGradientBrush;
            var _this = d as GradientBrushEditor;
            if (_this != null) _this.SetBackgroundValues(value);
        }

        private void SetBackgroundValues(XmlGradientBrush value)
        {
            if (value != null)
            {
                NotifyPropertyChanged("SelectedGradientAngle");
            }
        }

        public XmlGradientStop SelectedGradientStop  
        {
            get { return _selectedGradientStop; }
            set { _selectedGradientStop = value; NotifyPropertyChanged("SelectedGradientStop"); }
        }

        public XmlGradientAngle GradientBrushAngle
        {
            get { return GradientBrush.Angle; }
            set { GradientBrush.Angle = value; NotifyPropertyChanged("GradientBrushAngle"); UpdateDisplayBrush(); }
        }
        

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
