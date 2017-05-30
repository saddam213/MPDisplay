using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Plugins.SlimTv.Client.Helpers;
using MediaPortal.Plugins.SlimTv.Client.Models;
using MediaPortal.Plugins.SlimTv.Interfaces;
using MediaPortal.Plugins.SlimTv.Interfaces.Items;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using Log = Common.Log.Log;
using Timer = System.Threading.Timer;

namespace MediaPortal2Plugin.InfoManagers
{
    public class TvServerManager
    {
        #region Singleton Implementation

        private static TvServerManager _instance;

        private TvServerManager()
        {
            _log = LoggingManager.GetLog(typeof(TvServerManager));
        }

        public static TvServerManager Instance => _instance ?? (_instance = new TvServerManager());

        #endregion


        private readonly Log _log;
        private PluginSettings _settings;
        private SlimTvClientModel _tvmodel;
        private ITvHandler _tvHandler;
        private Func<List<APIChannel>> _getGuide;
        private Func<List<APIRecording>> _getRecordings;
        private Func<int, string, DateTime, DateTime, bool, bool> _setRecording;
        private string _channelGroup;
        private List<APIRecording> _lastRecordingMessage = new List<APIRecording>();
        private int _batchId;
        private Timer _updateTimer;
        private int _preRecordingInterval;
        private int _postRecordingInterval;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            _tvmodel = new SlimTvClientModel();

            SetupTvServerInterface();

            _channelGroup = _tvmodel.CurrentGroupName;
            _tvmodel.CurrentGroupNameProperty.Attach(GroupNamePropertyChanged);
            if (_getRecordings == null) return;
            if (_updateTimer == null)
            {
                _updateTimer = new Timer(o => UpdateGuideData(), null, 5000, -1);
            }
        }

   
        void GroupNamePropertyChanged(AbstractProperty property, object oldvalue)
        {
            _channelGroup = property.GetValue().ToString();
            SendChannelGroup();                         // send new group
            SendTvGuide();                              // and send TVGuide again because it contaisn only the channels of the group
        }

        public void Shutdown()
        {
            _tvmodel.CurrentGroupNameProperty.Detach(GroupNamePropertyChanged);
            _tvmodel.Dispose();

            if (_updateTimer == null) return;
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _updateTimer = null;
        }


        private void UpdateGuideData()
        {
            if (_updateTimer == null) return;
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            SendRecordings();
            _updateTimer.Change(5000, -1);
        }

        public void SendTvGuide()
        {
            if (_getGuide == null) return;
            _batchId++;
            var channels = _getGuide();
            for (var i = 0; i < channels.Count; i++)
            {
                MessageService.Instance.SendListMessage(new APIListMessage
                {
                    MessageType = APIListMessageType.TVGuide,
                    TvGuide = new APITVGuide
                    {
                        MessageType = APITVGuideMessageType.TvGuide,
                        TvGuideMessage = new APITvGuideMessage
                        {
                            BatchNumber = i, BatchCount = channels.Count, BatchId = _batchId, Channel = channels[i]
                        }
                    }
                });
                _lastRecordingMessage = null;           // make sure recordings are sent again 
            }
        }

        public void SendRecordings()
        {
            if (_getRecordings == null) return;

            var recordings = _getRecordings();

            if (_lastRecordingMessage != null && recordings.AreUnorderedEqualBy(_lastRecordingMessage, x => x.ProgramId)) return;

            _lastRecordingMessage = recordings;

            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.TVGuide, TvGuide = new APITVGuide
                {
                    MessageType = APITVGuideMessageType.Recordings, RecordingMessage = recordings
                }
            });
        }

        public void SendChannelGroup()
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.TVGuide, TvGuide = new APITVGuide
                {
                    MessageType = APITVGuideMessageType.TvGuideGroup, GuideGroup = _channelGroup
                }
            });
        }

        private void SetupTvServerInterface()
        {

            _tvHandler = ServiceRegistration.Get<ITvHandler>();
            if (_tvHandler == null)
            {
                _log.Message(LogLevel.Error, "[SetupTVServerInterface] - Cannot initialize TvHandler interface");
                return;
            }
            _tvHandler.Initialize();


            if (_tvHandler.ChannelAndGroupInfo == null) return;


             try
             {

                 _preRecordingInterval = 5;                 // set default in case the settings cannot be retrieved
                 _postRecordingInterval = 10;
                // todo: load actual settings (if available)

                 SetupGetGuide();                           // setup method to retrieve TV guide
                 SetupGetRecordings();                      // setup method to retrieve scheduled recordings
                 SetupSetRecording();                       // setup method to schedule recordings

             }
             catch (Exception ex)
             {
                 _log.Exception("[SetupTVServerInterface] - An exception occured seting up interface between MPDisplay plugin and SlimTvClient", ex);
             }
        }

         private void SetupGetGuide()
        {
            //var getGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetGroupByName") && m.GetParameters().Length == 1);
            //var getChannelsInGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetTVGuideChannelsForGroup") && m.GetParameters().Length == 1);
            //var getProgramsInChannel = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Length == 3 &&
            //            m.GetParameters()[0].ParameterType != typeof(DateTime));

            // if (getGroup == null || getChannelsInGroup == null || getProgramsInChannel == null)
            // {
            //     _log.Message(LogLevel.Error, "[SetupGetGuide] - Interface to TVServer cannot be initialized");
            //     return;
            // }

             _getGuide = () =>
            {
                var sp = new Stopwatch();
                sp.Start();
                var returnValue = new List<APIChannel>();
                try
                {
                    var allChannels = _tvmodel.CurrentGroupChannels;
                    var iSort = 0;

                    foreach (var listItem in allChannels)
                    {
                        var channel = (ChannelProgramListItem) listItem;
                        var apiPrograms = new List<APIProgram> {};
                        IList<IProgram> programs;

                        _tvHandler.ProgramInfo.GetPrograms(channel.Channel, DateTime.Now.Date,
                            DateTime.Now.AddDays(_settings.EPGDays).Date, out programs);
                        foreach (var program in programs)
                        {
                            RecordingStatus isScheduled;
                            _tvHandler.ScheduleControl.GetRecordingStatus(program, out isScheduled);
                            apiPrograms.Add(new APIProgram() {Id = program.ProgramId, ChannelId = program.ChannelId, Description = program.Description, Title = program.Title,
                                StartTime = program.StartTime, EndTime = program.EndTime, IsScheduled = isScheduled != RecordingStatus.None});
                        }
                        returnValue.Add( new APIChannel
                        {
                            Id = channel.Channel.ChannelId, Name = channel.Channel.Name, Logo = GetChannelLogo(channel.ChannelLogoPath), Programs = apiPrograms,
                            SortOrder = iSort++
                        });
                    }
                }
                catch (Exception ex)
                {
                    _log.Exception("[GetGuide] - An exception occured fetching guide data", ex);
                }
                                  
                return returnValue;
            };
        }

        private void SetupGetRecordings()
        {
 
            _getRecordings = () =>
            {
                var recordings = new List<APIRecording>();
                try
                {
                    IList<ISchedule> schedules;
                    _tvHandler.ScheduleControl.GetSchedules(out schedules);
                   if (schedules != null)
                    {
                        foreach (var schedule in schedules)
                        {
                                recordings.Add(new APIRecording
                                    {
                                        ChannelId = schedule.ChannelId, 
                                        ProgramId = schedule.ScheduleId, 
                                        StartTime = schedule.StartTime, EndTime = schedule.EndTime,
                                        RecordPaddingStart = schedule.PreRecordInterval.Minutes, RecordPaddingEnd = schedule.PostRecordInterval.Minutes
                                    });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.Exception("[GetRecordings] - An exception occured fetching recordings", ex);
                }
                return recordings;
            };
        }

        private void SetupSetRecording()
        {
            _setRecording = (channelId, title, startTime, endTime, cancel) =>
            {
                try
                {
                    IList<IProgram> programs; 
                   
                    _tvHandler.ProgramInfo.GetPrograms(title, startTime, endTime, out programs);
                    foreach (var program in programs)
                    {
                        RecordingStatus recStatus;
                        _tvHandler.ScheduleControl.GetRecordingStatus(program, out recStatus);
                    }

                    var schedule = getSchedule.Invoke(tvBusinessLayer, new object[] { channelId, title, startTime, endTime, 0 });

                    if (schedule != null)
                    {
                        if (cancel)
                        {
                            var isRecording = false;
                            var scheduleId = ReflectionHelper.GetPropertyValue(schedule, "IdSchedule", -1);
                            if (scheduleId > -1)
                            {
                                isRecording = (bool)isRecordingSchedule.Invoke(tvServer, new object[] { scheduleId, null });
                            }
                            if (isRecording  && scheduleId > -1)
                            {
                                stopRecording.Invoke(tvServer, new object[] { scheduleId });
                            }
                            ReflectionHelper.InvokeMethod(schedule, "Delete", null);
                        }
                        else
                        {
                            ReflectionHelper.SetPropertyValue(schedule, "PreRecordInterval", _preRecordingInterval);
                            ReflectionHelper.SetPropertyValue(schedule, "PostRecordInterval", _postRecordingInterval);
                            ReflectionHelper.InvokeMethod(schedule, "Persist", null);
                        }
                        // onNewSchedule.Invoke(tvServer, null);
                    }
                }
                catch (Exception ex)
                {
                    _log.Exception("[SetRecording] - An exception occured scheduling a recording", ex);
                }
                return true;
            };
        }

        private static APIImage GetChannelLogo(string logoPath)
        {         
            return ImageHelper.CreateImage(logoPath);
        }

        // Message from MPD-EPG: Program was selected:
        // cancel = false: create a schedule
        // cancel = true: cancel existing schedule
        private void SelectEpgItem(int channelId, string title, DateTime startTime, DateTime endTime,  bool cancel)
        {

            if( string.IsNullOrEmpty(title) || channelId == -1 ) return;
            _setRecording?.Invoke(channelId, title, startTime, endTime, cancel);
        }

        public void OnActionMessageReceived(APIActionMessage message)
        {
            if (message?.GuideAction == null) return;

            if (message.GuideAction.ActionType == APIGuideActionType.UpdateData)
            {
                SendTvGuide();
            }

            if (message.GuideAction.ActionType == APIGuideActionType.UpdateRecordings)
            {
                SendRecordings();
            }

            if (message.GuideAction.ActionType == APIGuideActionType.EPGAction)
            {
                SelectEpgItem(message.GuideAction.ChannelId, message.GuideAction.Title, message.GuideAction.StartTime, message.GuideAction.EndTime, message.GuideAction.Cancel);
            }
        }
    }
}
