using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUIFramework.GUI;
using GUIFramework.GUI.Controls;
using GUISkinFramework;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{
    public class TVGuideRepository : IRepository
    {
        #region Singleton Implementation

        private TVGuideRepository() { }
        private static TVGuideRepository _instance;
        public static TVGuideRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TVGuideRepository();
                }
                return _instance;
            }
        }


        public static void RegisterTvGuideData(Action callback)
        {
            Instance.TVGuideService.Register(TVGuideMessageType.TVGuideData, callback);
        }

        public static void RegisterRecordingData(Action callback)
        {
            Instance.TVGuideService.Register(TVGuideMessageType.RecordingData, callback);
        }

        public static void DeregisterMessage(TVGuideMessageType message, object owner)
        {
            Instance.TVGuideService.Deregister(message, owner);
        }

        #endregion

        public MessengerService<TVGuideMessageType> TVGuideService
        {
            get { return _tvGuideService; }
        }

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }
        private MessengerService<TVGuideMessageType> _tvGuideService = new MessengerService<TVGuideMessageType>();

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
        }

        public void ClearRepository()
        {

        }

        public void ResetRepository()
        {
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }

        public List<TvGuideChannel> GuideData
        {
            get { return _guideData; }
        }

        public List<APIRecording> RecordingData
        {
            get { return _recordings; }
        }

        private List<TvGuideChannel> _guideData = new List<TvGuideChannel>();
        private List<APIRecording> _recordings = new List<APIRecording>();

        public void AddDataMessage(APITVGuide message)
        {
            if (message != null)
            {
                switch (message.MessageType)
                {
                    case APITVGuideMessageType.TvGuide:
                       ProcessBatch(message.TvGuideMessage);
                       break;
                    case APITVGuideMessageType.Recordings:
                       ProcessRecordings( message.RecordingMessage);
                        break;
                    default:
                        break;
                }
            }
        }

        private SortedDictionary<int, APIChannel> _data = new SortedDictionary<int, APIChannel>();
        private int _currentBatchId = -1;
        private object _syncObject = new object();

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
                    }

                    if (message.BatchId == _currentBatchId)
                    {
                        _data.Add(message.BatchNumber, message.Channel);
                        Console.WriteLine(string.Format("BatchId: {0}, BatchNumber: {1}, Count: {2}", message.BatchId, message.BatchNumber, _data.Count));
                        if (_data.Count == message.BatchCount)
                        {
                            _guideData = _data.Values.OrderBy(v => v.SortOrder).Select(x => new TvGuideChannel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                SortOrder = x.SortOrder,
                                IsRadio = x.IsRadio,
                                Groups = x.Groups,
                                Programs = x.Programs.Select(p => new TvGuideProgram
                                {
                                    ChannelId = x.Id,
                                    Id = p.Id,
                                    Title = p.Title,
                                    Description = p.Description,
                                    IsRecording = p.IsRecording,
                                    StartTime = p.StartTime,
                                    EndTime = p.EndTime,
                                    IsScheduled = p.IsScheduled,
                                }).ToList()
                            }).ToList();

                            _guideData.ForEach(c => c.UpdateCurrentProgram());

                            TVGuideService.NotifyListeners(TVGuideMessageType.TVGuideData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              
            }
        }

       private void   ProcessRecordings(List<APIRecording> recordings)
       {
           lock (_recordings)
           {
               _recordings.Clear();
               _recordings = recordings;
               TVGuideService.NotifyListeners(TVGuideMessageType.RecordingData);
           }
       }
    }

    public enum TVGuideMessageType
    {
        TVGuideData,
        RecordingData
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

    

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
        }

        public void UpdateCurrentProgram(DateTime? date = null)
        {
            if (Programs != null && Programs.Any())
            {
                foreach (var program in Programs)
                {
                    if (program.IsCurrent != program.IsProgramCurrent(date))
                    {
                        program.IsCurrent = program.IsProgramCurrent(date);
                    }
                }
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
    }

    public class TvGuideProgram : INotifyPropertyChanged
    {
        private bool _isSelected;
        private bool _isRecording;
        private bool _isScheduled;

        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        private bool _isCurrent;

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
            var baseline = date == null ? DateTime.Now : date.Value;
            return baseline > StartTime && baseline < EndTime;
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
