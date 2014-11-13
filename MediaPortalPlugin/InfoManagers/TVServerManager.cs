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
        private string _channelGroup;
        private List<APIRecording> _lastRecordingMessage = new List<APIRecording>();
        private int _batchId = 0;
        private Timer _updateTimer;

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
                SendChannelGroup();
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

                    SendChannelGroup();
                }
            }
        }

        public void SendRecordings()
        {
            if (_getRecordings != null)
            {
                var recordings = _getRecordings();
                if (!recordings.AreUnorderedEqualBy(_lastRecordingMessage, x => x.ProgramId))
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
            string tvServerFile = Config.GetFolder(Config.Dir.Base) + @"\TVDatabase.dll";
            if (File.Exists(tvBusinessLayerFile) && File.Exists(tvBusinessLayerFile))
            {
                try
                {
                    var tvServerAssembly = Assembly.LoadFrom(tvServerFile);
                    var tvBusinessLayerAssembly = Assembly.LoadFrom(tvBusinessLayerFile);
                    if (tvBusinessLayerAssembly != null && tvServerAssembly != null)
                    {
                        Type scheduleType = tvServerAssembly.GetType("TvDatabase.Schedule");
                        var tvBusinessLayer = Activator.CreateInstance(tvBusinessLayerAssembly.GetType("TvDatabase.TvBusinessLayer"));
                        if (tvBusinessLayer != null && scheduleType != null)
                        {
                            var allChannels = ReflectionHelper.GetPropertyValue<IEnumerable>(tvBusinessLayer, "Channels", null);
                            var getProgramsInChannel = tvBusinessLayer.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 3 && m.GetParameters()[0].ParameterType != typeof(DateTime));
                        
                            if (allChannels != null && getProgramsInChannel != null)
                            {
                                _getGuide = () =>
                                {
                                    Stopwatch sp = new Stopwatch();
                                    sp.Start();
                                    var returnValue = new List<APIChannel>();
                                    try
                                    {
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
                                                    SortOrder = ReflectionHelper.GetPropertyValue<int>(channel, "SortOrder", -1),
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
            }
        }
    }
}
