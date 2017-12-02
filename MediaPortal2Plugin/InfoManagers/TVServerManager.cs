using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.Common;
using MediaPortal.Common.Services.TaskScheduler;
using MediaPortal.Plugins.SlimTv.Interfaces;
using MediaPortal.Plugins.SlimTv.Interfaces.Items;
using MediaPortal.UI.SkinEngine.SkinManagement;
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
        private Func<List<APIChannel>> _getGuide;
        private Func<List<APIRecording>> _getRecordings;
        private Func<int, string, DateTime, DateTime, bool, bool> _setRecording;
        private int _channelGroupId;
        private IChannelGroup _channelGroup;
        private List<APIRecording> _lastRecordingMessage = new List<APIRecording>();
        private int _batchId;
        private Timer _updateTimer;

        public void Initialize(PluginSettings settings)
        {

            _log.Message(LogLevel.Debug, "[Initialize] - Initializing TVServerManager...");
            _settings = settings;
            _channelGroup = null;
            _channelGroupId = -1;
            SetupTvServerInterface();

            if (_updateTimer == null)
            {
                _updateTimer = new Timer(o => UpdateGuideData(), null, 5000, -1);
            }
            _log.Message(LogLevel.Debug, "[Initialize] - Initialize complete");
        }

  
        public void Shutdown()
        {
            if (_updateTimer == null) return;
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _updateTimer = null;
        }

         private void UpdateGuideData()
        {
            if (_updateTimer == null) return;
            _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);

             var tvHandler = ServiceRegistration.Get<ITvHandler>();
            tvHandler.Initialize();

            if (_channelGroupId != tvHandler.ChannelAndGroupInfo.SelectedChannelGroupId)
            {
                _channelGroupId = tvHandler.ChannelAndGroupInfo.SelectedChannelGroupId;
                if (tvHandler.ChannelAndGroupInfo.GetChannelGroups(out IList<IChannelGroup> grouplist))
                {
                    _channelGroup = grouplist[_channelGroupId];
                }
                SendChannelGroup();                         // send new group
                SendTvGuide();                              // and send TVGuide again because it contaisn only the channels of the group

            }
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
               _log.Message(LogLevel.Debug, "[SendTvGuide] - TV guide for {0}: {1} programs", channels[i].Name, channels[i].Programs.Count);
            }
            _log.Message(LogLevel.Debug, "[SendTvGuide] - TV guide data sent for {0} channels", channels.Count);
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
            if (_channelGroup == null) return;

            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.TVGuide, TvGuide = new APITVGuide
                {
                    MessageType = APITVGuideMessageType.TvGuideGroup, GuideGroup = _channelGroup.Name
                }
            });
        }

        private void SetupTvServerInterface()
        {

             try
             {
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
             _getGuide = () =>
            {
              var sp = new Stopwatch();
              sp.Start();
              var returnValue = new List<APIChannel>();
              var tvHandler = ServiceRegistration.Get<ITvHandler>();

              if (tvHandler == null )
              {
                return returnValue;
              }
              tvHandler.Initialize();

                try
                {
                    if (tvHandler.ChannelAndGroupInfo.GetChannels(_channelGroup, out IList<IChannel> allChannels))
                    {
                        var iSort = 0;

                        foreach (var channel in allChannels)
                        {
                            var apiPrograms = new List<APIProgram>();
                            IList<IProgram> programs = null;
                            tvHandler.ProgramInfo?.GetPrograms(channel, DateTime.Now.Date, DateTime.Now.AddDays(_settings.EPGDays).Date, out programs);
                             if (programs != null)
                                foreach (var program in programs)
                                {
                                    var isScheduled = RecordingStatus.None;
                                    tvHandler.ScheduleControl?.GetRecordingStatus(program, out isScheduled);
                                    apiPrograms.Add(new APIProgram()
                                    {
                                        Id = program.ProgramId,
                                        ChannelId = program.ChannelId,
                                        Description = program.Description,
                                        Title = program.Title,
                                        StartTime = program.StartTime,
                                        EndTime = program.EndTime,
                                        IsScheduled = isScheduled != RecordingStatus.None
                                    });
                                }
                            returnValue.Add(new APIChannel
                            {
                                Id = channel.ChannelId,
                                Name = channel.Name,
                                Logo = GetChannelLogo(channel),
                                Programs = apiPrograms,
                                SortOrder = iSort++
                            });
                        }

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
              var tvHandler = ServiceRegistration.Get<ITvHandler>();
              if ( tvHandler == null )
              {
                return recordings;
              }
              tvHandler.Initialize();

              try
                {
                    IList<ISchedule> schedules = null;
                
                    tvHandler.ScheduleControl?.GetSchedules(out schedules);
                   if (schedules != null)
                    {
                        foreach (ISchedule schedule in schedules)
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

              var tvHandler = ServiceRegistration.Get<ITvHandler>();
              if (tvHandler == null) return false;
              tvHandler.Initialize();
              try
              {
                  tvHandler.ProgramInfo.GetPrograms(title, startTime, endTime, out var programs);
                  tvHandler.ScheduleControl.GetSchedules(out var schedules);

                  if (cancel)
                  {
                      foreach (var schedule in schedules)
                      {
                          if (schedule.ChannelId == channelId && schedule.StartTime == startTime &&
                              schedule.EndTime == endTime)
                              tvHandler.ScheduleControl.RemoveSchedule(schedule);
                      }
                  }
                  else
                  {
                      var scheduled = false;
                      foreach (var schedule in schedules)
                      {
                          if (schedule.ChannelId == channelId && schedule.StartTime == startTime &&
                              schedule.EndTime == endTime)
                              scheduled = true;
                      }
                      if (!scheduled)
                      {
                          foreach( var program in programs)
                          {
                              tvHandler.ScheduleControl.CreateSchedule(program, ScheduleRecordingType.Once,
                                  out var _);
                          }
                       }
                    }
 
 
                }
                catch (Exception ex)
                {
                    _log.Exception("[SetRecording] - An exception occured scheduling a recording", ex);
                }
                return true;
            };
        }

        private static APIImage GetChannelLogo(IChannel channel)
        {         
            return ImageHelper.CreateImage(channel.Name);
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
