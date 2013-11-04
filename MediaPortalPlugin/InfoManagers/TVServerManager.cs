using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Common.Helpers;
using MediaPortal.Configuration;
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
        private Func<List<APIChannel>> _getGuideForGroup;
        private Func<List<APIRecording>> _getRecordings;

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            SetupTVServerInterface();
            
        }

        public void Shutdown()
        {
           
        }

        private int _batchId = 0;

        public void SendTvGuide()
        {
            if (_getGuideForGroup != null)
            {
                _batchId++;
                var channels = _getGuideForGroup();
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
                }
            }
        }

        public void SendRecordings()
        {
            MessageService.Instance.SendListMessage(new APIListMessage
            {
                MessageType = APIListMessageType.TVGuide,
                TvGuide = new APITVGuide
                {
                    MessageType = APITVGuideMessageType.Recordings,
                    RecordingMessage = _getRecordings()
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
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 2 && m.GetParameters()[0].ParameterType != typeof(DateTime));

                            var getAllPrograms = tvBusinessLayer.GetType().GetMethods()
                                .FirstOrDefault(m => m.Name.Equals("GetPrograms") && m.GetParameters().Count() == 2 && m.GetParameters()[0].ParameterType == typeof(DateTime));

                            if (allChannels != null && getProgramsInChannel != null)
                            {
                                _getGuideForGroup = () =>
                                {
                                    var returnValue = new List<APIChannel>();
                                    try
                                    {
                                        foreach (var channel in allChannels)
                                        {
                                            var programs = (IEnumerable)getProgramsInChannel.Invoke(tvBusinessLayer, new object[] { channel, DateTime.MinValue });
                                            if (programs != null)
                                            {
                                                int channelId = ReflectionHelper.GetPropertyValue<int>(channel, "IdChannel", -1);

                                                var newPrograms = new List<APIProgram>();
                                                foreach (var program in programs)
                                                {
                                                    newPrograms.Add(new APIProgram
                                                    {
                                                        Id = ReflectionHelper.GetPropertyValue<int>(program, "IdProgram", -1),
                                                        ChannelId = channelId,
                                                        Title = ReflectionHelper.GetPropertyValue<string>(program, "Title", string.Empty),
                                                        StartTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "StartTime", DateTime.MinValue),
                                                        EndTime = ReflectionHelper.GetPropertyValue<DateTime>(program, "EndTime", DateTime.MinValue),
                                                        IsRecording = ReflectionHelper.GetPropertyValue<bool>(program, "IsRecording", false),
                                                        IsScheduled = ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingOncePending", false)
                                                                   || ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingSeriesPending", false),
                                                    });
                                                }

                                                returnValue.Add(new APIChannel
                                                {
                                                    Id = channelId,
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
                                            var programs = (IEnumerable)getAllPrograms.Invoke(tvBusinessLayer, new object[] { DateTime.Now.AddDays(-2), DateTime.Now.AddDays(10) });
                                            if (programs != null)
                                            {
                                                foreach (var program in programs)
                                                {
                                                    var isrecording = ReflectionHelper.GetPropertyValue<bool>(program, "IsRecording", false)
                                                                   || ReflectionHelper.GetPropertyValue<bool>(program, "IsRecordingOncePending", false)
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


   
       

    }
}
