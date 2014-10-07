using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using GUIFramework.Managers;
using GUISkinFramework.Controls;
using GUISkinFramework.ExtensionMethods;
using MPDisplay.Common.Utils;
using MPDisplay.Common.ExtensionMethods;
using Common;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIGuide.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlGuide))]
    public partial class GUIGuide : GUIControl
    {
        #region Fields

        private ICollectionView _channelData;
        private ScrollViewer _channelScrollViewer;
        private ScrollViewer _programScrollViewer;
        private ScrollViewer _timelineScrollViewer;
        private DispatcherTimer _updateTimer;
        private List<TvGuideProgram> _timeline = new List<TvGuideProgram>();
        private TvGuideChannel _selectedChannel;
        private TvGuideProgram _selectedProgram;
        private double _timelineLength = 0;
        private double _timelinePosition = 0;
        //private DateTime _timelineStart;
        //private DateTime _timelineEnd;
        private string _timelineInfo;
        private double _timelineCenterPosition;
        private string _currentGuideGroup; 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIGuide"/> class.
        /// </summary>
        public GUIGuide()
        {
            InitializeComponent();
            _channelScrollViewer = channelListBox.GetDescendantByType<ScrollViewer>();
            _programScrollViewer = programListBox.GetDescendantByType<ScrollViewer>();
            _timelineScrollViewer = timelineListBox.GetDescendantByType<ScrollViewer>();

            _channelScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            _programScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            _timelineScrollViewer.ManipulationBoundaryFeedback += (s, e) => e.Handled = true;
            MouseTouchDevice.RegisterEvents(channelListBox.GetDescendantByType<VirtualizingStackPanel>());
            MouseTouchDevice.RegisterEvents(programListBox.GetDescendantByType<VirtualizingStackPanel>());
            MouseTouchDevice.RegisterEvents(timelineListBox.GetDescendantByType<Canvas>());

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(5);
            _updateTimer.Tick += (s, e) => UpdateTimeline();
            _updateTimer.Stop();



            ChannelData = new ListCollectionView(TVGuideRepository.Instance.GuideData);
            ChannelData.Filter = new Predicate<object>(ChannelGroupFilter);
            ChannelData.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));

            TVGuideRepository.RegisterMessage(TVGuideMessageType.TVGuideData, CreateTimeline);
            TVGuideRepository.RegisterMessage(TVGuideMessageType.TvGuideGroup, OnChannelGroupChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlGuide SkinXml
        {
            get { return BaseXml as XmlGuide; }
        }

        /// <summary>
        /// Gets or sets the length of the timeline.
        /// </summary>
        public double TimelineLength
        {
            get { return _timelineLength; }
            set { _timelineLength = value; NotifyPropertyChanged("TimelineLength"); }
        }

        /// <summary>
        /// Gets or sets the timeline position.
        /// </summary>
        public double TimelinePosition
        {
            get { return _timelinePosition; }
            set { _timelinePosition = value; NotifyPropertyChanged("TimelinePosition"); }
        }

        /// <summary>
        /// Gets the timeline multiplier.
        /// </summary>
        public double TimelineMultiplier
        {
            get { return SkinXml.TimelineMultiplier; }
        }

        /// <summary>
        /// Gets the timeline start.
        /// </summary>
        public DateTime TimelineStart
        {
            get { return TVGuideRepository.Instance.GuideDataStart; }
        }

        /// <summary>
        /// Gets the timeline end.
        /// </summary>
        public DateTime TimelineEnd
        {
            get { return TVGuideRepository.Instance.GuideDataEnd; }
        }

        /// <summary>
        /// Gets or sets the timeline info.
        /// </summary>
        public string TimelineInfo
        {
            get { return _timelineInfo; }
            set { _timelineInfo = value; NotifyPropertyChanged("TimelineInfo"); }
        }

        /// <summary>
        /// Gets or sets the timeline center position.
        /// </summary>
        public double TimelineCenterPosition
        {
            get { return _timelineCenterPosition; }
            set { _timelineCenterPosition = value; NotifyPropertyChanged("TimelineCenterPosition"); }
        }

        /// <summary>
        /// Gets or sets the channel data.
        /// </summary>
        public ICollectionView ChannelData
        {
            get { return _channelData; }
            set { _channelData = value; NotifyPropertyChanged("ChannelData"); }
        }

        /// <summary>
        /// Gets or sets the timeline.
        /// </summary>
        public List<TvGuideProgram> Timeline
        {
            get { return _timeline; }
            set { _timeline = value; NotifyPropertyChanged("Timeline"); }
        }

        /// <summary>
        /// Gets or sets the selected channel.
        /// </summary>
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
                UpdateGuideProperties();
                NotifyPropertyChanged("SelectedChannel");
            }
        }

/// <summary>
/// Gets or sets the selected program.
/// </summary>
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
                UpdateGuideProperties();
                NotifyPropertyChanged("SelectedProgram");
            }
        }

        /// <summary>
        /// Gets or sets the current guide group.
        /// </summary>
        /// <value>
        /// The current guide group.
        /// </value>
        public string CurrentGuideGroup
        {
            get { return _currentGuideGroup; }
            set
            {
                _currentGuideGroup = value;
                UpdateGuideProperties();
                NotifyPropertyChanged("CurrentGuideGroup");
            }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
          
           
            _updateTimer.Start();
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
          //  TVGuideRepository.DeregisterMessage(TVGuideMessageType.TVGuideData, this);
       //     TVGuideRepository.DeregisterMessage(TVGuideMessageType.TvGuideGroup, this);
            _updateTimer.Stop();
        }

        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
            TVGuideRepository.NotifyListeners(TVGuideMessageType.RefreshRecordings);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the timeline.
        /// </summary>
        private void CreateTimeline()
        {
            if (TimelineStart != DateTime.MinValue && TimelineEnd != DateTime.MinValue)
            {
                Timeline.Clear();
                TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
                TimelinePosition = (TimelineLength - ((TimelineEnd - DateTime.Now).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
                var begin = TimelineStart.Round(TimeSpan.FromMinutes(30));
                Timeline = Enumerable.Range(0, ((int)(TimelineEnd - begin).TotalHours) * 2).Select(x => new TvGuideProgram
                 {
                     StartTime = begin.AddMinutes(30 * x),
                     EndTime = begin.AddMinutes((30 * x) + 30),
                     Title = begin.AddMinutes(30 * x).ToString("hh:mm")
                 }).ToList();

                TimelineCenterPosition = ((DateTime.Now - TimelineStart).TotalMinutes * TimelineMultiplier);
                _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
            }
        }

        /// <summary>
        /// Updates the timeline.
        /// </summary>
        private void UpdateTimeline()
        {
            if (ChannelData != null)
            {
                TimelinePosition = (TimelineLength - ((TimelineEnd - DateTime.Now).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
                TimelineCenterPosition = ((DateTime.Now - TimelineStart).TotalMinutes * TimelineMultiplier);

                if (DateTime.Now > LastUserInteraction.AddSeconds(20))
                {
                    _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
                }

            }
        }

        /// <summary>
        /// Called when the channel group changes.
        /// </summary>
        private void OnChannelGroupChanged()
        {
            CurrentGuideGroup = TVGuideRepository.Instance.GuideGroup;
            if (ChannelData != null)
            {
                ChannelData.Refresh();
            }
        }

        /// <summary>
        /// Filter predicate for channel group.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private bool ChannelGroupFilter(object item)
        {
            var channel = item as TvGuideChannel;
            if (channel != null)
            {
                return string.IsNullOrEmpty(CurrentGuideGroup) || channel.Groups.Any(g => g.Equals(CurrentGuideGroup, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        /// <summary>
        /// Updates the guide properties.
        /// </summary>
        private void UpdateGuideProperties()
        {
            PropertyRepository.AddLabelProperty("#MPD.Guide.Group", CurrentGuideGroup);
            PropertyRepository.AddLabelProperty("#MPD.Guide.Channel", SelectedChannel != null ? SelectedChannel.Name : string.Empty);
            PropertyRepository.AddImageProperty("#MPD.Guide.ChannelThumb", SelectedChannel != null ? SelectedChannel.Logo : null);
            PropertyRepository.AddLabelProperty("#MPD.Guide.Program", SelectedProgram != null ? SelectedProgram.Title : string.Empty);
            PropertyRepository.AddLabelProperty("#MPD.Guide.Description", SelectedProgram != null ? SelectedProgram.Description : string.Empty);
            PropertyRepository.AddLabelProperty("#MPD.Guide.EndTime", SelectedProgram != null ? SelectedProgram.EndTime.ToString("hh:mm d/M") : string.Empty);
            PropertyRepository.AddLabelProperty("#MPD.Guide.StartTime", SelectedProgram != null ? SelectedProgram.StartTime.ToString("hh:mm d/M") : string.Empty);
        }

        #endregion

        #region Control Events

        /// <summary>
        /// Handles the ScrollChanged event of the ListBoxes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Called when a program item selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnProgramItemSelected(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border)
            {
                SelectedProgram = (sender as Border).Tag as TvGuideProgram;
            }
        }

        #endregion
    }


 

  

}
