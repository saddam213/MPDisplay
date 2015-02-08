using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Common.Helpers;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Util;
using MessageFramework.DataObjects;
using Common.Settings;
using Common.Logging;
using System.Diagnostics;
using Common;

namespace MediaPortalPlugin.InfoManagers
{
    public class TVServerManager
    {
        #region Singleton Implementation

        private static TVServerManager instance;

        private TVServerManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(TVServerManager));
        }

        public static TVServerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TVServerManager();
                }
                return instance;
            }
        }

        #endregion


        private Common.Logging.Log Log;
        private PluginSettings _settings;
        private Func<List<APIChannel>> _getGuide;
        private Func<List<APIRecording>> _getRecordings;
        private Func<int, string, DateTime, DateTime, bool, bool> _setRecording;
        private string _channelGroup;
        private List<APIRecording> _lastRecordingMessage = new List<APIRecording>();
        private int _batchId = 0;
        private Timer _updateTimer;
        private int _preRecordingInterval;
        private int _postRecordingInterval;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            SetupTVServerInterface();
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;
        
           _channelGroup = MPSettings.Instance.GetValue("mytv", "group");

           if (_getRecordings != null)
           {
               if (_updateTimer == null)
               {
                   _updateTimer = new Timer((o) => UpdateGuideData(), null, 5000, -1);
               }
           }
        }

   
        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (!string.IsNullOrEmpty(tag) && tag == "#TV.Guide.Group")
            {
                _channelGroup = tagValue;
                SendChannelGroup();                         // send new group
                SendTvGuide();                              // and send TVGuide again because it contaisn only the channels of the group
            }
        }

        public void Shutdown()
        {
            GUIPropertyManager.OnPropertyChanged -= GUIPropertyManager_OnPropertyChanged;
            if (_updateTimer != null)
            {
                _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _updateTimer = null;
            }
        }


        private void UpdateGuideData()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
                SendRecordings();
                _updateTimer.Change(5000, -1);
            }
        }

        public void SendTvGuide()
        {
            if (_getGuide != null)
            {
                _batchId++;
                var channels = _getGuide();
                for (int i = 0; i < channels.Count; i++)
                {
                    MessageService.Instance.SendListMessage(new APIListMessage
                    {
                        MessageType = APIListMessageType.TVGuide,
                        TvGuide = new APITVGuide
                        {
                            MessageType = APITVGuideMessageType.TvGuide,
                            TvGuideMessage = new APITvGuideMessage
                            {
                                BatchNumber = i,
                                BatchCount = channels.Count,
                                BatchId = _batchId,
                                Channel = channels[i]
                            }
                        }
                    });
                    _lastRecordingMessage = null;           // make sure recordings are sent again 
                }
            }
        }

        public void SendRecordings()
        {
            if (_getRecordings != null)
            {
                var recordings = _getRecordings();
                if (_lastRecordingMessage == null || !recordings.AreUnorderedEqualBy(_lastRecordingMessage, x => x.ProgramId))
                {
                    _lastRecordingMessage = recordings;
                    MessageService.Instance.SendListMessage(new APIListMessage
                    {
                        MessageType = APIListMessageType.TVGuide,
                        TvGuide = new APITVGuide
                        {
                            MessageType = APITVGuideMessageType.Recordings,
                            RecordingMessage = recordings
                        }
                    });
                }
            }
        }

        public void SendChannelGroup()
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.TVGuide,
                TvGuide = new APITVGuide
                {
                    MessageType = APITVGuideMessageType.TvGuideGroup,
                    GuideGroup = _channelGroup
                }
            });
        }

         private void SetupTVServerInterface()
        {
            string tvBusinessLayerFile = Config.GetFolder(Config.Dir.Base) + @"\TvBusinessLayer.dll";
            string tvDatabaseFile = Config.GetFolder(Config.Dir.Base) + @"\TVDatabase.dll";
            string tvControlFile = Config.GetFolder(Config.Dir.Base) + @"\TvControl.dll";

            if (File.Exists(tvBusinessLayerFile) && File.Exists(tvBusinessLayerFile))
            {
                try
                {
                    var tvServerAssembly = Assembly.LoadFrom(tvDatabaseFile);
                    var tvBusinessLayerAssembly = Assembly.LoadFrom(tvBusinessLayerFile);
                    var tvControlAssembly = Assembly.LoadFrom(tvControlFile);

                    if (tvBusinessLayerAssembly != null && tvServerAssembly != null && tvControlAssembly != null)
                    {
                        Type scheduleType = tvServerAssembly.GetType("TvDatabase.Schedule");
                        Type virtualCardType = tvControlAssembly.GetType("TvControl.VirtualCard");

                        var tvBusinessLayer = Activator.CreateInstance(tvBusinessLayerAssembly.GetType("TvDatabase.TvBusinessLayer"));
                        var tvServer = Activator.CreateInstance(tvControlAssembly.GetType("TvControl.TvServer"));

                        if (tvBusinessLayer != null && scheduleType != null && tvServer != null)
                        {
                            var getGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetGroupByName") && m.GetParameters().Count() == 1);
                            var getChannelsInGroup = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetTVGuideChannelsForGroup") && m.GetParameters().Count() == 1);
                            var getProgramsInChannel = tvBusinessLayer.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 3 && m.GetParameters()[0].ParameterType != typeof(DateTime));

                            var getSchedule = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("AddSchedule") && m.GetParameters().Count() == 5);
                            var stopRecording = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("StopRecordingSchedule") && m.GetParameters().Count() == 1);
                            var onNewSchedule = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("OnNewSchedule") && m.GetParameters().Count() == 0);
                            var isRecordingSchedule = tvServer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("IsRecordingSchedule") && m.GetParameters().Count() == 2);


                            var getSetting = tvBusinessLayer.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetSetting") && m.GetParameters().Count() == 2);

                            _preRecordingInterval = 5;                          // set default in case the settings cannot be retrieved
                            _postRecordingInterval = 10;

                            if (getSetting != null)                             // get pre/post interval from MP settings
                            {
                                var preRecordingProperty = getSetting.Invoke(tvBusinessLayer, new object[] { "preRecordInterval", "5" });
                                var postRecordingProperty = getSetting.Invoke(tvBusinessLayer, new object[] { "postRecordInterval", "10" });

                                _preRecordingInterval = Int32.Parse(ReflectionHelper.GetPropertyValue<string>(preRecordingProperty, "Value", "5", null));
                                _postRecordingInterval = Int32.Parse(ReflectionHelper.GetPropertyValue<string>(postRecordingProperty, "Value", "10", null));
                            }

                            if (getGroup != null && getChannelsInGroup != null && getProgramsInChannel != null && getSchedule != null && stopRecording != null && onNewSchedule != null)
                            {
                                _getGuide = () =>
                                {
                                    Stopwatch sp = new Stopwatch();
                                    sp.Start();
                                    var returnValue = new List<APIChannel>();
                                    try
                                    {
                                         var group = getGroup.Invoke(tvBusinessLayer, new object[] { _channelGroup });
                                         int groupID = ReflectionHelper.GetPropertyValue<int>(group, "IdGroup", -1);
                                         var allChannels = (IEnumerable)getChannelsInGroup.Invoke(tvBusinessLayer, new object[] { groupID });
                                         int iSort = 0;
                                         foreach (var channel in allChannels)
                                        {
                                            var programs = (IEnumerable)getProgramsInChannel.Invoke(tvBusinessLayer, new object[] { channel, DateTime.Now.Date, DateTime.Now.AddDays(_settings.EPGDays).Date });
                                            if (programs != null)
                                            {
                                                int channelId = ReflectionHelper.GetPropertyValue<int>(channel, "IdChannel", -1);
                                                string channelName = ReflectionHelper.GetPropertyValue<string>(channel, "DisplayName", string.Empty);
                                                bool isRadio = ReflectionHelper.GetPropertyValue<bool>(channel, "IsRadio", false);

                                                var newPrograms = new List<APIProgram>();
                                                foreach (var program in programs)
                                                {
                                                    newPrograms.Add(new APIProgram
                                                    {
                                                        Id = ReflectionHelper.GetPropertyValue<int>(program, "IdProgram", -1),
                                                        ChannelId = channelId,
                                                        Title = ReflectionHelper.GetPropertyValue<string>(program, "Title", string.Empty),
                                                        Description = ReflectionHelper.GetPropertyValue<string>(program, "Description", string.Empty),
                                                        StartTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "StartTime", DateTime.MinValue),
                                                        EndTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "EndTime", DateTime.MinValue),
                                                        IsScheduled = ReflectionHelper.GetPropertyValue<bool>(program, "IsRecording", false)
                                                                   || ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingOncePending", false)
                                                                   || ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingSeriesPending", false),
                                                    });
                                                }

                                                returnValue.Add(new APIChannel
                                                {
                                                    Id = channelId,
                                                    Logo =  GetChannelLogo(channelName, isRadio),
                                                    Name = ReflectionHelper.GetPropertyValue<string>(channel, "DisplayName", string.Empty),
                                                    SortOrder = iSort++,
                                                    IsRadio = ReflectionHelper.GetPropertyValue<bool>(channel, "IsRadio", false),
                                                    Groups = ReflectionHelper.GetPropertyValue<List<string>>(channel, "GroupNames", new List<string>()),
                                                    Programs = newPrograms
                                                });
                                            }
                                        }
                                       
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Exception("[GetGuide] - An exception occured fetching guide data", ex);
                                    }
                                  
                                    return returnValue;
                                };

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
                                                var preRecordInterval = ReflectionHelper.GetPropertyValue<int>(schedule, "PreRecordInterval", 0);
                                                var postRecordInterval = ReflectionHelper.GetPropertyValue<int>(schedule, "PostRecordInterval", 0);
                                                var programs = ReflectionHelper.InvokeStaticMethod<IEnumerable>(scheduleType, "GetProgramsForSchedule", null, schedule);
                                                if (programs != null)
                                                {
                                                    foreach (var program in programs)
                                                    {
                                                        recordings.Add(new APIRecording
                                                        {
                                                            ChannelId = ReflectionHelper.GetPropertyValue<int>(program, "IdChannel", -1),
                                                            ProgramId = ReflectionHelper.GetPropertyValue<int>(program, "IdProgram", -1),
                                                            StartTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "StartTime", DateTime.MinValue),
                                                            EndTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "EndTime", DateTime.MinValue),
                                                            RecordPaddingStart = preRecordInterval,
                                                            RecordPaddingEnd = postRecordInterval
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Exception("[GetRecordings] - An exception occured fetching recordings", ex);
                                    }
                                    return recordings;
                                };

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
                                                var scheduleId = ReflectionHelper.GetPropertyValue<int>(schedule, "IdSchedule", -1);
                                                //var card = Activator.CreateInstance(virtualCardType);
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
                                                ReflectionHelper.SetPropertyValue<int>(schedule, "PreRecordInterval", _preRecordingInterval);
                                                ReflectionHelper.SetPropertyValue<int>(schedule, "PostRecordInterval", _postRecordingInterval);
                                                ReflectionHelper.InvokeMethod(schedule, "Persist", null);
                                            }
                                            onNewSchedule.Invoke(tvServer, null);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Exception("[SetRecording] - An exception occured scheduling a recording", ex);
                                    }
                                    return true;
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception("[SetupTVServerInterface] - An exception occured seting up interface between MPDisplay plugin and TvBusinessLayer", ex);
                }
            }
        }



        private APIImage GetChannelLogo(string channelName, bool isRadio)
        {
            string filename = Utils.GetCoverArt(isRadio ? Thumbs.Radio : Thumbs.TVChannel, channelName);
          
            return ImageHelper.CreateImage(filename);
        }

        // Message from MPD-EPG: Program was selected:
        // cancel = false: create a schedule
        // cancel = true: cancel existing schedule
        private void SelectEPGItem(int channelId, string title, DateTime startTime, DateTime endTime,  bool cancel)
        {

            if( title == null || title.Length == 0 || channelId == -1 ) return;
            if (_setRecording != null)
            {
                _setRecording(channelId, title, startTime, endTime, cancel);
            }
          }

        public void OnActionMessageReceived(APIActionMessage message)
        {
            if (message != null && message.GuideAction != null)
            {
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
                    SelectEPGItem(message.GuideAction.ChannelId, message.GuideAction.Title, message.GuideAction.StartTime, message.GuideAction.EndTime, message.GuideAction.Cancel);
                }
            }
        }
    }
}
