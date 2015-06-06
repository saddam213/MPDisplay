using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Util;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using Log = Common.Log.Log;

namespace MediaPortalPlugin.InfoManagers
{
    public class TvServerManager
    {
        #region Singleton Implementation

        private static TvServerManager _instance;

        private TvServerManager()
        {
            _log = LoggingManager.GetLog(typeof(TvServerManager));
        }

        public static TvServerManager Instance
        {
            get { return _instance ?? (_instance = new TvServerManager()); }
        }

        #endregion


        private Log _log;
        private PluginSettings _settings;
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
            SetupTvServerInterface();
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;
        
           _channelGroup = MPSettings.Instance.GetValue("mytv", "group");

            if (_getRecordings == null) return;
            if (_updateTimer == null)
            {
                _updateTimer = new Timer(o => UpdateGuideData(), null, 5000, -1);
            }
        }

   
        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (string.IsNullOrEmpty(tag) || tag != "#TV.Guide.Group") return;
            _channelGroup = tagValue;
            SendChannelGroup();                         // send new group
            SendTvGuide();                              // and send TVGuide again because it contaisn only the channels of the group
        }

        public void Shutdown()
        {
            GUIPropertyManager.OnPropertyChanged -= GUIPropertyManager_OnPropertyChanged;
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
            var tvBusinessLayerFile = Config.GetFolder(Config.Dir.Base) + @"\TvBusinessLayer.dll";
            var tvDatabaseFile = Config.GetFolder(Config.Dir.Base) + @"\TVDatabase.dll";

            if (!File.Exists(tvBusinessLayerFile) || !File.Exists(tvBusinessLayerFile))
            {
                _log.Message(LogLevel.Error, "[SetupTVServerInterface] - Dlls for interface to TVServer not found");
                return;
            }

             try
             {
                 var tvServerAssembly = Assembly.LoadFrom(tvDatabaseFile);
                 var tvBusinessLayerAssembly = Assembly.LoadFrom(tvBusinessLayerFile);

                 if (tvBusinessLayerAssembly == null || tvServerAssembly == null)
                 {
                    _log.Message(LogLevel.Error, "[SetupTVServerInterface] - Assemblys for TVServer cannot be loaded");
                     return;
                 }

                 var tvBusinessLayer = Activator.CreateInstance(tvBusinessLayerAssembly.GetType("TvDatabase.TvBusinessLayer"));

                 if (tvBusinessLayer == null)
                 {
                    _log.Message(LogLevel.Error, "[SetupTVServerInterface] - TVBusinessLayer cannot be instantiated");
                     return;
                 }

                 var getSetting = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetSetting") && m.GetParameters().Count() == 2);

                 _preRecordingInterval = 5;                 // set default in case the settings cannot be retrieved
                 _postRecordingInterval = 10;

                 if (getSetting != null)                    // get pre/post interval from MP settings
                 {
                     var preRecordingProperty = getSetting.Invoke(tvBusinessLayer, new object[] {"preRecordInterval", "5"});
                     var postRecordingProperty = getSetting.Invoke(tvBusinessLayer, new object[] {"postRecordInterval", "10"});

                     _preRecordingInterval = Int32.Parse(ReflectionHelper.GetPropertyValue(preRecordingProperty, "Value", "5"));
                     _postRecordingInterval = Int32.Parse(ReflectionHelper.GetPropertyValue(postRecordingProperty, "Value", "10"));
                 }

                 SetupGetGuide(tvBusinessLayer);            // setup method to retrieve TV guide
                 SetupGetRecordings(tvServerAssembly);      // setup method to retrieve scheduled recordings
                 SetupSetRecording(tvBusinessLayer);        // setup method to schedule recordings

             }
             catch (Exception ex)
             {
                 _log.Exception("[SetupTVServerInterface] - An exception occured seting up interface between MPDisplay plugin and TvBusinessLayer", ex);
             }
        }

         private void SetupGetGuide(object tvBusinessLayer)
        {
            var getGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetGroupByName") && m.GetParameters().Count() == 1);
            var getChannelsInGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetTVGuideChannelsForGroup") && m.GetParameters().Count() == 1);
            var getProgramsInChannel = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 3 &&
                        m.GetParameters()[0].ParameterType != typeof(DateTime));

             if (getGroup == null || getChannelsInGroup == null || getProgramsInChannel == null)
             {
                 _log.Message(LogLevel.Error, "[SetupGetGuide] - Interface to TVServer cannot be initialized");
                 return;
             }

             _getGuide = () =>
            {
                var sp = new Stopwatch();
                sp.Start();
                var returnValue = new List<APIChannel>();
                try
                {
                    var group = getGroup.Invoke(tvBusinessLayer, new object[] { _channelGroup });
                    var groupId = ReflectionHelper.GetPropertyValue(@group, "IdGroup", -1);
                    var allChannels = (IEnumerable)getChannelsInGroup.Invoke(tvBusinessLayer, new object[] { groupId });
                    var iSort = 0;
                    returnValue.AddRange(from object channel in allChannels
                        let programs = (IEnumerable) getProgramsInChannel.Invoke(tvBusinessLayer, new[] {channel, DateTime.Now.Date, DateTime.Now.AddDays(_settings.EPGDays).Date})
                        where programs != null
                        let channelId = ReflectionHelper.GetPropertyValue(channel, "IdChannel", -1)
                        let channelName = ReflectionHelper.GetPropertyValue(channel, "DisplayName", string.Empty)
                        let isRadio = ReflectionHelper.GetPropertyValue(channel, "IsRadio", false)
                        let newPrograms = (from object program in programs select new APIProgram
                            {
                                Id = ReflectionHelper.GetPropertyValue(program, "IdProgram", -1), ChannelId = channelId, Title = ReflectionHelper.GetPropertyValue(program, "Title", string.Empty), Description = ReflectionHelper.GetPropertyValue(program, "Description", string.Empty), StartTime = ReflectionHelper.GetPropertyValue(program, "StartTime", DateTime.MinValue), EndTime = ReflectionHelper.GetPropertyValue(program, "EndTime", DateTime.MinValue), IsScheduled = ReflectionHelper.GetPropertyValue(program, "IsRecording", false) || ReflectionHelper.GetPropertyValue(program, "IsRecordingOncePending", false) || ReflectionHelper.GetPropertyValue(program, "IsRecordingSeriesPending", false)
                            }).ToList()
                        select new APIChannel
                        {
                            Id = channelId, Logo = GetChannelLogo(channelName, isRadio), Name = ReflectionHelper.GetPropertyValue(channel, "DisplayName", string.Empty), SortOrder = iSort++, IsRadio = ReflectionHelper.GetPropertyValue(channel, "IsRadio", false), Groups = ReflectionHelper.GetPropertyValue(channel, "GroupNames", new List<string>()), Programs = newPrograms
                        });
                }
                catch (Exception ex)
                {
                    _log.Exception("[GetGuide] - An exception occured fetching guide data", ex);
                }
                                  
                return returnValue;
            };
        }

        private void SetupGetRecordings(Assembly tvServerAssembly)
        {
            var scheduleType = tvServerAssembly.GetType("TvDatabase.Schedule");

            if (scheduleType == null)
            {
                _log.Message(LogLevel.Error, "[SetupGetRecording] - Type Schedule cannot be retrieved from TVServer assembly");
                return;
            }

            _getRecordings = () =>
            {
                var recordings = new List<APIRecording>();
                try
                {
                    var schedules = ReflectionHelper.InvokeStaticMethod<IEnumerable>(scheduleType, "ListAll", null);
                    if (schedules != null)
                    {

                        foreach (var schedule in schedules)
                        {
                            var preRecordInterval = ReflectionHelper.GetPropertyValue(schedule, "PreRecordInterval", 0);
                            var postRecordInterval = ReflectionHelper.GetPropertyValue(schedule, "PostRecordInterval", 0);
                            var programs = ReflectionHelper.InvokeStaticMethod<IEnumerable>(scheduleType, "GetProgramsForSchedule", null, schedule);
                            if (programs != null)
                            {
                                recordings.AddRange(from object program in programs
                                    select new APIRecording
                                    {
                                        ChannelId = ReflectionHelper.GetPropertyValue(program, "IdChannel", -1), 
                                        ProgramId = ReflectionHelper.GetPropertyValue(program, "IdProgram", -1), 
                                        StartTime = ReflectionHelper.GetPropertyValue(program, "StartTime", DateTime.MinValue),
                                        EndTime = ReflectionHelper.GetPropertyValue(program, "EndTime", DateTime.MinValue),
                                        RecordPaddingStart = preRecordInterval, RecordPaddingEnd = postRecordInterval
                                    });
                            }
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

        private void SetupSetRecording(object tvBusinessLayer)
        {
            var tvControlFile = Config.GetFolder(Config.Dir.Base) + @"\TvControl.dll";
            var tvControlAssembly = Assembly.LoadFrom(tvControlFile);
            if (tvControlAssembly == null)
            {
                _log.Message(LogLevel.Error, "[SetupSetRecording] - TVControl assemby cannot be loaded");
                return;
            }

            var tvServer = Activator.CreateInstance(tvControlAssembly.GetType("TvControl.TvServer"));
            if (tvServer == null)
            {
                _log.Message(LogLevel.Error, "[SetupSetRecording] - TVServer remote instance cannot be initialized");
                return;
            }

            var getSchedule = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("AddSchedule") && m.GetParameters().Count() == 5);
            var stopRecording = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("StopRecordingSchedule") && m.GetParameters().Count() == 1);
            var onNewSchedule = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("OnNewSchedule") && !m.GetParameters().Any());
            var isRecordingSchedule = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("IsRecordingSchedule") && m.GetParameters().Count() == 2);

            if (getSchedule == null || stopRecording == null || onNewSchedule == null || isRecordingSchedule == null)
            {
                _log.Message(LogLevel.Error, "[SetupSetRecording] - Interface to TVServer cannot be initialized");
                return;
            }

            _setRecording = (channelId, title, startTime, endTime, cancel) =>
            {
                try
                {
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
                        onNewSchedule.Invoke(tvServer, null);
                    }
                }
                catch (Exception ex)
                {
                    _log.Exception("[SetRecording] - An exception occured scheduling a recording", ex);
                }
                return true;
            };
        }

        private static APIImage GetChannelLogo(string channelName, bool isRadio)
        {
            var filename = Utils.GetCoverArt(isRadio ? Thumbs.Radio : Thumbs.TVChannel, channelName);
          
            return ImageHelper.CreateImage(filename);
        }

        // Message from MPD-EPG: Program was selected:
        // cancel = false: create a schedule
        // cancel = true: cancel existing schedule
        private void SelectEpgItem(int channelId, string title, DateTime startTime, DateTime endTime,  bool cancel)
        {

            if( string.IsNullOrEmpty(title) || channelId == -1 ) return;
            if (_setRecording != null)
            {
                _setRecording(channelId, title, startTime, endTime, cancel);
            }
          }

        public void OnActionMessageReceived(APIActionMessage message)
        {
            if (message == null || message.GuideAction == null) return;

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
