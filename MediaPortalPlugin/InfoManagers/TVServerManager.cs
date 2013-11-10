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
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class TVServerManager
    {
        #region Singleton Implementation

        private static TVServerManager instance;

        private TVServerManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(TVServerManager));
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


        private MPDisplay.Common.Log.Log Log;
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
           _updateTimer = new Timer((o) => UpdateGuideData(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        private void UpdateGuideData()
        {
            SendRecordings();
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
            var recordings = _getRecordings();
            if (HasRecordingsChanged(recordings))
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
            if (File.Exists(tvBusinessLayerFile))
            {
                try
                {
                    var tvBusinessLayerAssembly = Assembly.LoadFrom(tvBusinessLayerFile);
                    if (tvBusinessLayerAssembly != null)
                    {
                        var tvBusinessLayer = Activator.CreateInstance(tvBusinessLayerAssembly.GetType("TvDatabase.TvBusinessLayer"));
                        if (tvBusinessLayer != null)
                        {
                            var allChannels = ReflectionHelper.GetPropertyValue<IEnumerable>(tvBusinessLayer, "Channels", null);
                            var getProgramsInChannel = tvBusinessLayer.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 3 && m.GetParameters()[0].ParameterType != typeof(DateTime));

                            var getAllPrograms = tvBusinessLayer.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 2 && m.GetParameters()[0].ParameterType == typeof(DateTime));

                            if (allChannels != null && getProgramsInChannel != null)
                            {
                                _getGuide = () =>
                                {
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

                                    }
                                    return returnValue;
                                };

                                _getRecordings = () =>
                                    {
                                        var recordings = new List<APIRecording>();
                                        try
                                        {
                                            var programs = (IEnumerable)getAllPrograms.Invoke(tvBusinessLayer, new object[] { DateTime.Now.Date, DateTime.Now.AddDays(_settings.EPGDays).Date });
                                            if (programs != null)
                                            {
                                                foreach (var program in programs)
                                                {
                                                    var isrecording = ReflectionHelper.GetPropertyValue<bool>(program, "IsRecording", false)
                                                      ||  ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingOncePending", false)
                                                                   || ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingSeriesPending", false);
                                                    if (isrecording)
                                                    {
                                                        recordings.Add(new APIRecording
                                                        {
                                                            ChannelId = ReflectionHelper.GetPropertyValue<int>(program, "IdChannel", -1),
                                                            ProgramId = ReflectionHelper.GetPropertyValue<int>(program, "IdProgram", -1)
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {

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

        private bool HasRecordingsChanged(IEnumerable<APIRecording> newRecordings)
        {
            lock (_lastRecordingMessage)
            {
                return !newRecordings.Select(p => p.ProgramId).OrderBy(p => p).SequenceEqual(
               _lastRecordingMessage.Select(p => p.ProgramId).OrderBy(p => p));
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
            }
        }
    }
}
