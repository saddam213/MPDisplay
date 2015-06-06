using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using Common.Log;
using Common.MessengerService;
using Common.Settings;
using GUIFramework.Utils;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIFramework.Repositories
{
    public class TVGuideRepository : IRepository
    {
        #region Singleton Implementation

        private static TVGuideRepository _instance;
        public static TVGuideRepository Instance
      {
            get { return _instance ?? (_instance = new TVGuideRepository()); }
      }

        private TVGuideRepository()
        {
            _log = LoggingManager.GetLog(typeof(TVGuideRepository));
        }

        public static void RegisterMessage(TVGuideMessageType messageType, Action callback)
        {
            Instance.TVGuideService.Register(messageType, callback);
        }

         public static void DeregisterMessage(TVGuideMessageType message, object owner)
        {
            Instance.TVGuideService.Deregister(message, owner);
        }


        public static void NotifyListeners(TVGuideMessageType message)
        {
            Instance.TVGuideService.NotifyListeners(message);
        }

        #endregion

        public MessengerService<TVGuideMessageType> TVGuideService
        {
            get { return _tvGuideService; }
        }

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }
        private MessengerService<TVGuideMessageType> _tvGuideService = new MessengerService<TVGuideMessageType>();

        private const int UpdateInterval = 5;          // update every x seconds
        private const int UpdateCounterInit = 12;      // update complete guide only every y * x seconds

        private Log _log;

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            if (_updateTimer != null) return;

            _updateCounter = UpdateCounterInit;
            _updateTimer = new Timer(o => UpdateGuideData(), null, TimeSpan.FromSeconds(UpdateInterval), TimeSpan.FromSeconds(UpdateInterval));
        }

        public DateTime FilterProgramStart { get; set; }
        public DateTime FilterProgramEnd { get; set; }

        // Set the filter for the 'visible' programs
        public void FilterPrograms(DateTime start, DateTime end)
        {
            FilterProgramStart = start;
            FilterProgramEnd = end;

            // update properties (filtered programs) of the programs, if any
            if (_guideData == null || !_guideData.Any()) return;
            foreach (var channel in _guideData )
            {
                channel.FilteredPrograms = null;   // this will notify the listeners
            }
        }

        private Timer _updateTimer;
        private int _updateCounter;

        public void ClearRepository()
        {
        }

        public void ResetRepository()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }

        public ObservableCollection<TvGuideChannel> GuideData
        {
            get { return _guideData; }
        }

        public ObservableCollection<TVRecording> RecordingData
        {
            get { return _recordingData; }
        }

        public string GuideGroup { get; private set; }

        public DateTime GuideDataStart
        {
            get { return _guideDataStart; }
        }

        public DateTime GuideDataEnd
        {
            get { return _guideDataEnd; }
        }

        public APIGuideAction CurrentGuideAction { get; set; }

        private ObservableCollection<TvGuideChannel> _guideData = new ObservableCollection<TvGuideChannel>();
        private DateTime _guideDataStart = DateTime.MinValue;
        private DateTime _guideDataEnd = DateTime.MinValue;

        private ObservableCollection<TVRecording> _recordingData = new ObservableCollection<TVRecording>();

        private SortedDictionary<int, APIChannel> _data = new SortedDictionary<int, APIChannel>();
        private int _currentBatchId = -1;
        private object _syncObject = new object();

        public void AddDataMessage(APITVGuide message)
        {
            if (message == null) return;

            switch (message.MessageType)
            {
                case APITVGuideMessageType.TvGuide:
                    ProcessBatch(message.TvGuideMessage);
                    break;
                case APITVGuideMessageType.Recordings:
                    ProcessRecordings(message.RecordingMessage);
                    break;
                case APITVGuideMessageType.TvGuideGroup:
                    ProcessGuideGroup(message.GuideGroup);
                    break;
            }
        }

        // if a CurrentGuideAction exists, this method will update the guidedata, assuming that
        // the action has been sent to MP and has been properly processed. This avoids lag in
        // the update of EPG and Recordings on MPD side
        public void CurrentGuideActionProcessed()
        {
            if (CurrentGuideAction == null) return;

            var channel = _guideData.FirstOrDefault(p => p.Id == CurrentGuideAction.ChannelId);
            if (channel == null) return;

            var program = channel.Programs.FirstOrDefault(p => p.ChannelId == CurrentGuideAction.ChannelId && p.Id == CurrentGuideAction.ProgramId);
            if (program == null) return;

            if (CurrentGuideAction.Cancel)                             // program has already been scheduled, so cancel now
            {
                program.IsScheduled = false;
                program.IsRecording = false;
            }
            else
            {
                program.IsScheduled = true;
            }
        }

        /// <summary>
        ///  Process a batch of the TVGuide message
        /// </summary>
        /// <param name="message"></param>
        private void ProcessBatch(APITvGuideMessage message)
        {
            try
            {
                lock (_syncObject)
                {
                    if (message.BatchId > _currentBatchId)
                    {
                        _data.Clear();
                        _currentBatchId = message.BatchId;
                        _log.Message(LogLevel.Verbose, string.Format("Receiving TvGuide data batch, BatchId: {0}, Count: {1}", message.BatchId, message.BatchCount));
                    }

                    if (message.BatchId != _currentBatchId) return;

                    _data.Add(message.BatchNumber, message.Channel);
                    _log.Message(LogLevel.Verbose, string.Format("BatchId: {0}, BatchNumber: {1}, ReceivedCount: {2}", message.BatchId, message.BatchNumber, _data.Count));
                    if (_data.Count != message.BatchCount) return;

                    _log.Message(LogLevel.Verbose, string.Format("Received TvGuide data batch, BatchId: {0}, Count: {1}", message.BatchId, message.BatchCount));

                    var data = _data.Values.Select(x => new TvGuideChannel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        SortOrder = x.SortOrder,
                        IsRadio = x.IsRadio,
                        Groups = x.Groups,
                        Logo = x.Logo.ToImageBytes(),
                        Programs = x.Programs.Select(p => new TvGuideProgram
                        {
                            ChannelId = x.Id,
                            Id = p.Id,
                            Title = p.Title,
                            Description = p.Description,
                            StartTime = p.StartTime,
                            EndTime = p.EndTime,
                            IsScheduled = p.IsScheduled
                        }).ToList()
                    }).ToArray();

                    var allPrograms = data.SelectMany(c => c.Programs);
                    var tvGuidePrograms = allPrograms as IList<TvGuideProgram> ?? allPrograms.ToList();
                    _guideDataStart = tvGuidePrograms.Min(p => p.StartTime);
                    _guideDataEnd = tvGuidePrograms.Max(p => p.EndTime);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _guideData.Clear();
                        foreach (var item in data)
                        {
                            _guideData.Add(item);
                        }
                        SetProgramState();
                        TVGuideService.NotifyListeners(TVGuideMessageType.TVGuideData);
                    });
                }
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Background thread to update the TVGuide and recording status
        /// </summary>
        private void UpdateGuideData()
        {
            lock (_syncObject)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetRecordingState();                            // update recording state at every call
                    _updateCounter--;                               // upadte entire guide only at defined intervals
                    if (_updateCounter > 0) return;

                    _updateCounter = UpdateCounterInit;
                    SetProgramState();
                });
            }
        }

        /// <summary>
        /// Process the TVGuideGRoup message
        /// </summary>
        /// <param name="group"></param>
        private void ProcessGuideGroup(string group)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (GuideGroup == @group) return;

                GuideGroup = @group;
                TVGuideService.NotifyListeners(TVGuideMessageType.TvGuideGroup);
            });
        }

        /// <summary>
        /// Process the recording message
        /// </summary>
        /// <param name="recordings"></param>
        private void ProcessRecordings(IEnumerable<APIRecording> recordings)
        {
            lock (_syncObject)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var anyRecording = false;

                    _recordingData.Clear();                             // copy recordings into list 
                    foreach (var recording in recordings)
                    {
                        _recordingData.Add( new TVRecording {
                            ChannelId = recording.ChannelId,
                            ProgramId = recording.ProgramId,
                            StartTime = recording.StartTime,
                            EndTime = recording.EndTime,
                            RecordPaddingStart = recording.RecordPaddingStart,
                            RecordPaddingEnd = recording.RecordPaddingEnd
                        });
                    }

                    foreach (var recording in _recordingData)          // update guide data accordingly so the EPG displays status correctly
                    {
                        if (recording.IsProgramRecording()) anyRecording = true;

                        var channel = _guideData.FirstOrDefault(p => p.Id == recording.ChannelId);
                        if (channel == null) continue;

                        var program = channel.Programs.FirstOrDefault(p => p.ChannelId == recording.ChannelId && p.Id == recording.ProgramId);
                        if (program == null) continue;

                        program.IsScheduled = true;
                        program.RecordPaddingStart = recording.RecordPaddingStart;
                        program.RecordPaddingEnd = recording.RecordPaddingEnd;
                    }

                    TVGuideService.NotifyListeners(TVGuideMessageType.RecordingData);

                    InfoRepository.Instance.IsTvRecording = anyRecording;
                });
            }
        }

        // update the status properties of the TV guide
        private void SetProgramState()
        {
            if (!_guideData.Any()) return;

            foreach (var channel in _guideData)
            {
                channel.UpdateCurrentProgram();
            }
        }

        // update recording flag from the recordings
        private void SetRecordingState()
        {
            if (!_recordingData.Any())
            {
                InfoRepository.Instance.IsTvRecording = false;
                return;
            }
            InfoRepository.Instance.IsTvRecording = _recordingData.Any(recording => recording.IsProgramRecording());
        }
    }

    public enum TVGuideMessageType
    {
        TVGuideData,
        RecordingData,
        TvGuideGroup,
        RefreshGuideData,
        RefreshRecordings,
        EPGItemSelected
    }

    public class TvGuideChannel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool IsRadio { get; set; }
        public List<TvGuideProgram> Programs { get; set; }
        public List<string> Groups { get; set; }
        public byte[] Logo { get; set; }

        public List<TvGuideProgram> FilteredPrograms
        {
            get
            {
                return Programs.Where(p => p.EndTime >= TVGuideRepository.Instance.FilterProgramStart && p.StartTime <= TVGuideRepository.Instance.FilterProgramEnd).ToList();
            }
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                NotifyPropertyChanged("FilteredPrograms");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
        }

        public bool UpdateCurrentProgram(DateTime? date = null)
        {
            if (Programs == null || !Programs.Any()) return false;

            var anyRecording = false;

            foreach (var program in Programs)
            {
                if (program.IsCurrent != program.IsProgramCurrent(date))
                {
                    program.IsCurrent = program.IsProgramCurrent(date);
                }

                if (program.IsRecording != program.IsProgramRecording())
                {
                    program.IsRecording = program.IsProgramRecording();
                }
                if ( program.IsRecording ) anyRecording = true;
                  
            }
            return anyRecording;
        }
  

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }

    public class TvGuideProgram : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isRecording;
        private bool _isScheduled;
        private bool _isCurrent;

        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RecordPaddingStart { get; set; }
        public int RecordPaddingEnd { get; set; }
       

        public bool IsRecording
        {
            get { return _isRecording; }
            set { _isRecording = value; NotifyPropertyChanged("IsRecording"); }
        }

        public bool IsScheduled
        {
            get { return _isScheduled; }
            set { _isScheduled = value; NotifyPropertyChanged("IsScheduled"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
        }

        public bool IsCurrent
        {
            get { return _isCurrent; }
            set { _isCurrent = value; NotifyPropertyChanged("IsCurrent"); }
        }


        public bool IsProgramCurrent(DateTime? date = null)
        {
            var baseline = date ?? DateTime.Now;
            return baseline > StartTime && baseline < EndTime;
        }

        public bool IsProgramRecording()
        {
            if (!IsScheduled) return false;

            return DateTime.Now > StartTime.AddMinutes(-RecordPaddingStart) && DateTime.Now < EndTime.AddMinutes(RecordPaddingEnd);
        }
     
        public event PropertyChangedEventHandler PropertyChanged;
     
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class TVRecording : INotifyPropertyChanged
    {
        public int ChannelId { get; set; }
        public int ProgramId { get; set; }
        public int RecordPaddingStart { get; set; }
        public int RecordPaddingEnd { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        private bool _isRecording;

        public bool IsRecording
        {
            get { return _isRecording; }
            set { _isRecording = value; NotifyPropertyChanged("IsRecording"); }
        }

        public bool IsProgramRecording()
        {
            return DateTime.Now > StartTime.AddMinutes(-RecordPaddingStart) && DateTime.Now < EndTime.AddMinutes(RecordPaddingEnd);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

    }

}
