using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Common.Helpers;
using Common.Settings;
using GUIFramework.Repositories;
using GUISkinFramework.Skin;
using MPDisplay.Common;
using MPDisplay.Common.ExtensionMethods;

namespace SkinEditor.Controls
{
    /// <summary>
    /// Interaction logic for GUIButton.xaml
    /// </summary>
    public partial class Guide : INotifyPropertyChanged
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
            if (e.NewValue == null) return;

            if (_guideData == null)
            {
                var data = Path.Combine(RegistrySettings.ProgramDataPath, "SkinEditor\\GuideData.xml");
                if (File.Exists(data))
                {
                    _guideData = SerializationHelper.Deserialize<List<TvGuideChannel>>(data);
                }
            }
            var guide = d as Guide;
            if (guide != null) guide.LoadGuideData(_guideData);
        }

        private static List<TvGuideChannel> _guideData;

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
        

        private double _timelineLength;
        private double _timelinePosition;
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
            if (e.PropertyName != "TimelineMultiplier") return;

            NotifyPropertyChanged("TimelineMultiplier");
            Dispatcher.BeginInvoke((Action)delegate
            {
                TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
                TimelinePosition = (TimelineLength - ((TimelineEnd - Now()).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
                TimelineCenterPosition = ((Now() - TimelineStart).TotalMinutes * TimelineMultiplier);
                _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
            }, DispatcherPriority.Background);
        }


        private DateTime Now()
        {
            return TimelineStart.AddHours(6).AddMinutes(13);
        }

        private void SetTimeline(IEnumerable<TvGuideChannel> data)
        {
            var tvGuideChannels = data as IList<TvGuideChannel> ?? data.ToList();
            if (data == null || !tvGuideChannels.Any()) return;

            Timeline.Clear();
            var allPrograms = tvGuideChannels.SelectMany(channel => channel.Programs);
            var tvGuidePrograms = allPrograms as IList<TvGuideProgram> ?? allPrograms.ToList();
            TimelineStart = tvGuidePrograms.Min(p => p.StartTime);
            TimelineEnd = tvGuidePrograms.Max(p => p.EndTime);
            TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
            TimelinePosition = (TimelineLength - ((TimelineEnd - Now()).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));

            var begin = TimelineStart.Round(TimeSpan.FromMinutes(30));
            Timeline = Enumerable.Range(0, ((int)(TimelineEnd - begin).TotalHours) * 2).Select(x => new TvGuideProgram
            {
                StartTime = begin.AddMinutes(30 * x),
                EndTime = begin.AddMinutes((30 * x) + 30),
                Title = begin.AddMinutes(30 * x).ToString("t")
            }).ToList();

            TimelineCenterPosition = ((Now() - TimelineStart).TotalMinutes * TimelineMultiplier);

            _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
            _timelineScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
            markerScrollviewer.ScrollToHorizontalOffset(TimelinePosition);
        }


        private void ListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var voffset = (int)e.VerticalOffset;
            var hoffset = (int)e.HorizontalOffset;

            if (!Equals(sender, timelineListBox))
            {

                if (Equals(sender, channelListBox) && (int)_programScrollViewer.VerticalOffset != voffset)
                {
                    _programScrollViewer.ScrollToVerticalOffset(voffset);
                }

                if (Equals(sender, programListBox) && (int)_channelScrollViewer.VerticalOffset != voffset)
                {
                    _channelScrollViewer.ScrollToVerticalOffset(voffset);
                }
            }

            if (Equals(sender, channelListBox)) return;

            if (Equals(sender, timelineListBox) && (int)_programScrollViewer.HorizontalOffset != hoffset)
            {
                _programScrollViewer.ScrollToHorizontalOffset(hoffset);
                markerScrollviewer.ScrollToHorizontalOffset(hoffset);
                TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");
            }

            if (!Equals(sender, programListBox) || (int) _timelineScrollViewer.HorizontalOffset == hoffset) return;

            _timelineScrollViewer.ScrollToHorizontalOffset(hoffset);
            markerScrollviewer.ScrollToHorizontalOffset(hoffset);
            TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");
        }


        private void OnProgramItemSelected(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                SelectedProgram = border.Tag as TvGuideProgram;
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

        private static double GetItemWidth(DateTime startTime, DateTime endTime, double multi)
        {
            return (endTime - startTime).TotalMinutes * multi;
        }

        private static double GetStartPoint(DateTime startTime, DateTime guideStart, double multi)
        {
            return ((startTime - guideStart).TotalMinutes * multi);
        }




        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TvGuideProgram guideProgram;

            if (parameter.ToString() == "ProgramWidth")
            {
                if (values != null && values.Count() == 3)
                {
                    guideProgram = values[0] as TvGuideProgram;
                    if (guideProgram != null)
                    {
                        var program1 = guideProgram;
                        return GetItemWidth(program1.StartTime, program1.EndTime, (double)values[2]);
                    }
                }
            }
            if (parameter.ToString() != "ProgramPosition") return 0.0;

            if (values == null || values.Count() != 3) return 0.0;

            guideProgram = values[0] as TvGuideProgram;
            if (guideProgram == null) return 0.0;

            var program2 = guideProgram;
            return GetStartPoint(program2.StartTime, (DateTime)values[1], (double)values[2]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



     

    }

  

}
