using System;
using System.Collections.Generic;
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
using GUISkinFramework.Animations;
using GUISkinFramework.Controls;
using GUIFramework.Managers;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using MPDisplay.Common.Utils;
using System.ComponentModel;
using System.Windows.Threading;
using GUISkinFramework;
using GUIFramework;
using Common.Helpers;
using MPDisplay.Common.ExtensionMethods;
using Common;

namespace SkinEditor.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    public partial class Guide : UserControl , INotifyPropertyChanged
    {
        private ObservableCollection<TvGuideChannel> _channelData = new ObservableCollection<TvGuideChannel>();
        private ScrollViewer _channelScrollViewer;
        private ScrollViewer _programScrollViewer;
        private ScrollViewer _timelineScrollViewer;
      
        public Guide()
        {
             InitializeComponent();
             DataContext = this;
            _channelScrollViewer = channelListBox.GetDescendantByType<ScrollViewer>();
            _programScrollViewer = programListBox.GetDescendantByType<ScrollViewer>();
            _timelineScrollViewer = timelineListBox.GetDescendantByType<ScrollViewer>();

            _channelScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            _programScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            _timelineScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            MouseTouchDevice.RegisterEvents(channelListBox.GetDescendantByType<VirtualizingStackPanel>());
            MouseTouchDevice.RegisterEvents(programListBox.GetDescendantByType<VirtualizingStackPanel>());
            MouseTouchDevice.RegisterEvents(timelineListBox.GetDescendantByType<Canvas>());
        }

        public XmlGuide BaseXml
        {
            get { return (XmlGuide)GetValue(BaseXmlProperty); }
            set { SetValue(BaseXmlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BaseXml.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseXmlProperty =
            DependencyProperty.Register("BaseXml", typeof(XmlGuide), typeof(Guide), new PropertyMetadata(new XmlGuide(), OnSkinChanged));

        private static void OnSkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (GuideData == null)
                {
                    GuideData = SerializationHelper.Deserialize<List<TvGuideChannel>>(Environment.CurrentDirectory + "\\Data\\GuideData.xml");
                }
                (d as Guide).LoadGuideData(GuideData);
            }
        }

        private static List<TvGuideChannel> GuideData;

        public ObservableCollection<TvGuideChannel> ChannelData
        {
            get { return _channelData; }
            set { _channelData = value; NotifyPropertyChanged("ChannelData"); }
        }

        private List<TvGuideProgram> _timeline = new List<TvGuideProgram>();

        public List<TvGuideProgram> Timeline
        {
            get { return _timeline; }
            set { _timeline = value; NotifyPropertyChanged("Timeline"); }
        }

        private TvGuideChannel _selectedChannel;

        public TvGuideChannel SelectedChannel
        {
            get { return _selectedChannel; }
            set 
            { 
                _selectedChannel = value;

                if (_selectedChannel != null && _selectedProgram != null)
                {
                    if (_selectedProgram.ChannelId != _selectedChannel.Id)
                    {
                        SelectedProgram = null;
                    }
                }

                NotifyPropertyChanged("SelectedChannel"); 
            }
        }

        private TvGuideProgram _selectedProgram;

        public TvGuideProgram SelectedProgram
        {
            get { return _selectedProgram; }
            set 
            {
                if (_selectedProgram != null)
                {
                    _selectedProgram.IsSelected = false;
                }
                _selectedProgram = value;
                if (_selectedProgram != null)
                {
                    _selectedProgram.IsSelected = true;
                }
                NotifyPropertyChanged("SelectedProgram");
            }
        }
        

        private double _timelineLength = 0;
        private double _timelinePosition = 0;
        private DateTime _timelineStart;
        private DateTime _timelineEnd;
        private string _timelineInfo;
        private double _timelineCenterPosition;

        public double TimelineLength
        {
            get { return _timelineLength; }
            set { _timelineLength = value; NotifyPropertyChanged("TimelineLength"); }
        }

        public double TimelinePosition
        {
            get { return _timelinePosition; }
            set { _timelinePosition = value; NotifyPropertyChanged("TimelinePosition"); }
        }

        public double TimelineMultiplier
        {
            get { return BaseXml.TimelineMultiplier; }
        }

        public DateTime TimelineStart
        {
            get { return _timelineStart; }
            set { _timelineStart = value; NotifyPropertyChanged("TimelineStart"); }
        }

        public DateTime TimelineEnd
        {
            get { return _timelineEnd; }
            set { _timelineEnd = value; NotifyPropertyChanged("TimelineEnd"); }
        }

        public string TimelineInfo
        {
            get { return _timelineInfo; }
            set { _timelineInfo = value; NotifyPropertyChanged("TimelineInfo"); }
        }

        public double TimelineCenterPosition
        {
            get { return _timelineCenterPosition; }
            set { _timelineCenterPosition = value; NotifyPropertyChanged("TimelineCenterPosition"); }
        }


        private void LoadGuideData(List<TvGuideChannel> data)
        {
            BaseXml.PropertyChanged -= BaseXml_PropertyChanged;
            BaseXml.PropertyChanged += BaseXml_PropertyChanged;

            Dispatcher.BeginInvoke((Action)delegate
            {
                ChannelData.Clear();
                SetTimeline(data);
                ChannelData = new ObservableCollection<TvGuideChannel>(data);
                foreach (var channel in ChannelData)
                {
                    channel.UpdateCurrentProgram(Now());
                }

              

            }, DispatcherPriority.Background);
        }

        void BaseXml_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TimelineMultiplier")
            {
                NotifyPropertyChanged("TimelineMultiplier");
                Dispatcher.BeginInvoke((Action)delegate
                {
                    TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
                    TimelinePosition = (TimelineLength - ((TimelineEnd - Now()).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
                    TimelineCenterPosition = ((Now() - TimelineStart).TotalMinutes * TimelineMultiplier);
                    _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
                }, DispatcherPriority.Background);
            }
        }


        private DateTime Now()
        {
            return TimelineStart.AddHours(6).AddMinutes(13);
        }

        private void SetTimeline(IEnumerable<TvGuideChannel> data)
        {
            if (data != null && data.Any())
            {
                Timeline.Clear();
                var allPrograms = data.SelectMany(c => c.Programs);
                TimelineStart = allPrograms.Min(p => p.StartTime);
                TimelineEnd = allPrograms.Max(p => p.EndTime);
                TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
                TimelinePosition = (TimelineLength - ((TimelineEnd - Now()).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));

                var begin = TimelineStart.Round(TimeSpan.FromMinutes(30));
                Timeline = Enumerable.Range(0, ((int)(TimelineEnd - begin).TotalHours) * 2).Select(x => new TvGuideProgram
                {
                    StartTime = begin.AddMinutes(30 * x),
                    EndTime = begin.AddMinutes((30 * x) + 30),
                    Title = begin.AddMinutes(30 * x).ToString("hh:mm")
                }).ToList();

                TimelineCenterPosition = ((Now() - TimelineStart).TotalMinutes * TimelineMultiplier);

                _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
                _timelineScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
                markerScrollviewer.ScrollToHorizontalOffset(TimelinePosition);
            }
        }


        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            int voffset = (int)e.VerticalOffset;
            int hoffset = (int)e.HorizontalOffset;

            if (sender != timelineListBox)
            {

                if (sender == channelListBox && (int)_programScrollViewer.VerticalOffset != voffset)
                {
                    _programScrollViewer.ScrollToVerticalOffset(voffset);
                }

                if (sender == programListBox && (int)_channelScrollViewer.VerticalOffset != voffset)
                {
                    _channelScrollViewer.ScrollToVerticalOffset(voffset);
                }
            }

            if (sender != channelListBox)
            {
                if (sender == timelineListBox && (int)_programScrollViewer.HorizontalOffset != hoffset)
                {
                    _programScrollViewer.ScrollToHorizontalOffset(hoffset);
                    markerScrollviewer.ScrollToHorizontalOffset(hoffset);
                    TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");
                }

                if (sender == programListBox && (int)_timelineScrollViewer.HorizontalOffset != hoffset)
                {
                    _timelineScrollViewer.ScrollToHorizontalOffset(hoffset);
                    markerScrollviewer.ScrollToHorizontalOffset(hoffset);
                    TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");
                }
            }
        }


        private void OnProgramItemSelected(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border)
            {
                SelectedProgram = (sender as Border).Tag as TvGuideProgram;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
  
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }


    public class ProgramWidthConverter : IMultiValueConverter
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
                if (values != null && values.Count() == 3)
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
                if (values != null && values.Count() == 3)
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
