using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Common.Helpers;
using GUIFramework.Managers;
using GUIFramework.Repositories;
using GUISkinFramework.Skin;
using MessageFramework.Messages;
using MPDisplay.Common;
using MPDisplay.Common.ExtensionMethods;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interaction logic for GUIGuide.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlGuide))]
    public partial class GUIGuide
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
        private double _timelineLength;
        private double _timelinePosition;
        private string _timelineInfo;
        private double _timelineCenterPosition;
        private string _currentGuideGroup;
        private DateTime _lastProgramClick;
        private const int DoubleClickDelay = 500;
        private DateTime _lastMediaportalAction;
        private double _viewportHorizontalPosition;
        private const double Tolerance = 0.00001;

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

            _updateTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
            _updateTimer.Tick += (s, e) => UpdateTimeline();
            _updateTimer.Stop();

            _lastProgramClick = DateTime.MinValue;
            _lastMediaportalAction = DateTime.MinValue;

            ChannelData = new ListCollectionView(TVGuideRepository.Instance.GuideData);
            ChannelData.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
 
            TVGuideRepository.RegisterMessage(TVGuideMessageType.TVGuideData, CreateTimeline);
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
            set { _channelData = value; NotifyPropertyChanged("ChannelData");
            }
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
                if (SelectedChannel == null)
                {
                    SelectedChannel = TVGuideRepository.Instance.GuideData.FirstOrDefault(channel => channel.Id == _selectedProgram.ChannelId);
                }
                if (_selectedProgram != null && _selectedChannel != null)
                {
                    if (SelectedChannel != null && _selectedProgram.ChannelId != SelectedChannel.Id)
                    {
                        SelectedChannel = TVGuideRepository.Instance.GuideData.FirstOrDefault(channel => channel.Id == _selectedProgram.ChannelId);
                    }
                    else
                    {
                        UpdateGuideProperties();
                    }
                }
                else
                {
                    UpdateGuideProperties();
                }
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
            _updateTimer.Stop();
        }

        public override void OnWindowOpen()
        {
            base.OnWindowOpen();
            TVGuideRepository.NotifyListeners(TVGuideMessageType.RefreshRecordings);
            InfoRepository.RegisterMessage<int, int>(InfoMessageType.FocusedTVGuideId, OnFocusedTVProgramChanged);
        }

        public override void OnWindowClose()
        {
            base.OnWindowClose();
            InfoRepository.DeregisterMessage(InfoMessageType.FocusedTVGuideId, this);
        }
        #endregion

        #region Methods

         // callback when a message from Mediaportal is received from TVGuide: Set focus on selected program and scroll program
        // into viewport of EPG
        public void OnFocusedTVProgramChanged(int programId, int channelId)
        {
            if (programId <= 0 || channelId <= 0) return;

            var channel = TVGuideRepository.Instance.GuideData.FirstOrDefault(p => p.Id == channelId);
            if (channel == null) return;

            var program = channel.Programs.FirstOrDefault(p => p.ChannelId == channelId && p.Id == programId);
            if (program == null) return;

            _lastMediaportalAction = DateTime.Now;
            SelectedProgram = program;
            TimelinePosition = (TimelineLength - ((TimelineEnd - program.StartTime).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
            programListBox.Dispatcher.BeginInvoke(new Action(delegate
            {
                programListBox.ScrollIntoView(channel);
                _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
            }));
        }

        ///
        ///<summary>
        /// Updates the filter for visible program items
        ///</summary>
        private void UpdateProgramFilter()
        {
            if (!(_programScrollViewer.HorizontalOffset > 0.0) || !(Math.Abs(_viewportHorizontalPosition - _programScrollViewer.HorizontalOffset) > Tolerance)) return;

            if (
                !(_programScrollViewer.HorizontalOffset > (_viewportHorizontalPosition + (2*_programScrollViewer.ViewportWidth))) &&
                !(_programScrollViewer.HorizontalOffset < (_viewportHorizontalPosition - _programScrollViewer.ViewportWidth))) return;

            _viewportHorizontalPosition = _programScrollViewer.HorizontalOffset;
            var start = TimelineStart.AddMinutes((_viewportHorizontalPosition - (2 * _programScrollViewer.ViewportWidth)) / TimelineMultiplier);
            var end = TimelineStart.AddMinutes((_viewportHorizontalPosition + (3 *_programScrollViewer.ViewportWidth)) / TimelineMultiplier);
            TVGuideRepository.Instance.FilterPrograms(start, end);
        }

        /// <summary>
        /// Creates the timeline.
        /// </summary>
        private void CreateTimeline()
        {
             if (TimelineStart == DateTime.MinValue || TimelineEnd == DateTime.MinValue) return;

             Timeline.Clear();
             TimelineLength = (TimelineEnd - TimelineStart).TotalMinutes * TimelineMultiplier;
             TimelinePosition = (TimelineLength - ((TimelineEnd - DateTime.Now).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
             var begin = TimelineStart.Round(TimeSpan.FromMinutes(30));
             Timeline = Enumerable.Range(0, ((int)(TimelineEnd - begin).TotalHours) * 2).Select(x => new TvGuideProgram
             {
                 StartTime = begin.AddMinutes(30 * x),
                 EndTime = begin.AddMinutes((30 * x) + 30),
                 Title = begin.AddMinutes(30 * x).ToString("t")
             }).ToList();

             TimelineCenterPosition = ((DateTime.Now - TimelineStart).TotalMinutes * TimelineMultiplier);
             _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
        }

        /// <summary>
        /// Updates the timeline.
        /// </summary>
        private void UpdateTimeline()
        {
            if (ChannelData == null) return;

            if (DateTime.Now <= LastUserInteraction.AddSeconds(20) || DateTime.Now <= _lastMediaportalAction.AddSeconds(30)) return;

            TimelinePosition = (TimelineLength - ((TimelineEnd - DateTime.Now).TotalMinutes * TimelineMultiplier)) - ((_programScrollViewer.ViewportWidth / 2.0) - (15 * TimelineMultiplier));
            TimelineCenterPosition = ((DateTime.Now - TimelineStart).TotalMinutes * TimelineMultiplier);
            _programScrollViewer.ScrollToHorizontalOffset(TimelinePosition);
        }

        /// <summary>
        /// Updates the guide properties.
        /// </summary>
        private void UpdateGuideProperties()
        {
            PropertyRepository.AddLabelProperty("#TV.Guide.ChannelName", SelectedChannel != null ? SelectedChannel.Name : string.Empty);
            PropertyRepository.AddImageProperty("#TV.Guide.thumb", SelectedChannel != null ? SelectedChannel.Logo : null);
            PropertyRepository.AddLabelProperty("#TV.Guide.Title", SelectedProgram != null ? SelectedProgram.Title : string.Empty);
            PropertyRepository.AddLabelProperty("#TV.Guide.Description", SelectedProgram != null ? SelectedProgram.Description : string.Empty);
            PropertyRepository.AddLabelProperty("#TV.Guide.Time", SelectedProgram != null ? SelectedProgram.StartTime.ToString("HH:mm d/M") + " - " + SelectedProgram.EndTime.ToString("HH:mm d/M") : string.Empty);
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

            if (!Equals(sender, channelListBox))
            {
                if (Equals(sender, timelineListBox) && (int)_programScrollViewer.HorizontalOffset != hoffset)
                {
                    _programScrollViewer.ScrollToHorizontalOffset(hoffset);
                    markerScrollviewer.ScrollToHorizontalOffset(hoffset);
                    TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");
                 }

                if (Equals(sender, programListBox) && (int)_timelineScrollViewer.HorizontalOffset != hoffset)
                {
                    _timelineScrollViewer.ScrollToHorizontalOffset(hoffset);
                    markerScrollviewer.ScrollToHorizontalOffset(hoffset);
                    TimelineInfo = TimelineStart.AddMinutes(hoffset / TimelineMultiplier).ToString("dddd d/M");                       
                }
            }
            UpdateProgramFilter();
       }

        /// <summary>
        /// Called when a program item selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnProgramItemSelected(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var pg = border.Tag as TvGuideProgram;
        
            // double-click selects the program for scheduling or cancel of the schedule
            if (pg == SelectedProgram && DateTime.Now < _lastProgramClick.AddMilliseconds(DoubleClickDelay))
            {
                if (pg != null)
                {
                    var action = new APIGuideAction
                    {
                        ChannelId = pg.ChannelId,
                        ProgramId = pg.Id,
                        Title = pg.Title,
                        StartTime = pg.StartTime,
                        EndTime = pg.EndTime,
                        Cancel = pg.IsScheduled
                    };

                    TVGuideRepository.Instance.CurrentGuideAction = action;
                }

                if (pg != null && pg.IsScheduled)                             // program has already been scheduled, so cancel now
                {
                    var xmlGuide = BaseXml as XmlGuide;
                    if (xmlGuide != null && !OpenConfirmationDialog(xmlGuide.CancelDialogId))
                    {
                        TVGuideRepository.NotifyListeners(TVGuideMessageType.EPGItemSelected);
                        pg.IsScheduled = false;
                        pg.IsRecording = false;
                    }
                }
                else
                {
                    var xmlGuide = BaseXml as XmlGuide;
                    if (xmlGuide != null && !OpenConfirmationDialog(xmlGuide.CreateDialogId))
                    {
                        TVGuideRepository.NotifyListeners(TVGuideMessageType.EPGItemSelected);
                        if (pg != null) pg.IsScheduled = true;
                    }
                }
                NotifyPropertyChanged("SelectedChannel");
            }
            else if (!Equals(pg, SelectedProgram) ) 
            {
                SelectedProgram = pg;
            }
            _lastProgramClick = DateTime.Now;
        }

        // open the confirmation dialog, if any is configured
        // result is false if no dialog is configured, else true
        private static bool OpenConfirmationDialog( int dialogId )
        {
            if (dialogId <= 0) return false;

            var openaction = new XmlAction
            {
                ActionType = XmlActionType.OpenDialog,
                Param1 = dialogId.ToString()
            };
            GUIActionManager.ActionService.NotifyListeners(XmlActionType.OpenDialog, openaction);
            return true;
        }

        #endregion
    }


}
