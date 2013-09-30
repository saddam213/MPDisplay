using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Status;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using MPDisplay.Common.Log;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin.InfoManagers
{
    public class PropertyManager
    {
        #region Singleton Implementation

        private static PropertyManager instance;

        private PropertyManager()
        {
            Log = MPDisplay.Common.Log.LoggingManager.GetLog(typeof(PropertyManager));
        }

        public static PropertyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PropertyManager();
                }
                return instance;
            }
        }

        #endregion

        private MPDisplay.Common.Log.Log Log;

        private List<APIPropertyMessage> _properties = new List<APIPropertyMessage>();
        private List<string> _registeredProperties = new List<string>();
        private PluginSettings _settings;
        private bool _suspended;
        private ServerChecker _systemInfo = new ServerChecker();

        //public bool Suspend { get; set; }

        public void Initialize(PluginSettings settings)
        {
            _settings = settings;
            _systemInfo.OnTextDataChanged += SystemInfo_OnTextDataChanged;
            _systemInfo.OnNumberDataChanged += SystemInfo_OnNumberDataChanged;
            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;

            if (_settings.IsSystemInfoEnabled)
            {
                _systemInfo.StartMonitoring();
            }
        }

     

        public void Shutdown()
        {

            if (_settings.IsSystemInfoEnabled)
            {
                _systemInfo.StopMonitoring();
            }

            _systemInfo.OnTextDataChanged -= SystemInfo_OnTextDataChanged;
            _systemInfo.OnNumberDataChanged -= SystemInfo_OnNumberDataChanged;
            GUIPropertyManager.OnPropertyChanged -= GUIPropertyManager_OnPropertyChanged;
        }

        public void RegisterWindowProperties(List<APIPropertyMessage> properties)
        {
            Suspend(true);
            if (properties != null && properties.Any())
            {
                lock (_properties)
                {
                    _properties.Clear();
                    _properties.AddRange(properties);
                }

                lock (_registeredProperties)
                {
                    Log.Message(LogLevel.Verbose, "[RegisterWindowProperties] - Registering MPDisplay skin property tags...");
                    _registeredProperties.Clear();
                    _registeredProperties.AddRange(properties.SelectMany(x => x.Tags).Distinct());
                    foreach (var prop in _registeredProperties)
                    {
                          Log.Message(LogLevel.Verbose, "[RegisterWindowProperties] - Registering property tag: {0}", prop);
                    }
                    Log.Message(LogLevel.Verbose, "[RegisterWindowProperties] - Registering MPDisplay skin property tags complete.");
                }
            }
            else
            {
                lock (_registeredProperties)
                {
                    _registeredProperties.Clear();
                }
            }
            Suspend(false);
        }

        public void Suspend(bool suspend)
        {
            _suspended = suspend;
            if (!_suspended)
            {
                _registeredProperties.ForEach(prop => ProcessProperty(prop, GUIPropertyManager.GetProperty(prop)));
            }
        }

        private void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (!_suspended)
            {
                ThreadPool.QueueUserWorkItem((o) => ProcessProperty(tag, tagValue));
            }
        }

        private void ProcessProperty(string tag, string tagValue)
        {
            if (_registeredProperties.Contains(tag))
            {
                try
                {
                    foreach (var property in _properties.Where(x => x.Tags.Contains(tag)).ToList())
                    {
                        switch (property.PropertyType)
                        {
                            case APIPropertyType.Label:
                                SendLabelProperty(property.SkinTag, tagValue);
                                break;
                            case APIPropertyType.Image:
                                SendImageProperty(property.SkinTag, tagValue);
                                break;
                            case APIPropertyType.Number:
                                SendNumberProperty(property.SkinTag, tagValue);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch { }
            }
        }

        public void SendLabelProperty(string tag, string tagValue)
        {
            MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
            {
                SkinTag = tag,
                PropertyType = APIPropertyType.Label,
                Label = tagValue
            });
        }

        private void SendImageProperty(string tag, string tagValue)
        {
            //if (!string.IsNullOrEmpty(tagValue))
            //{
                MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
                {
                    SkinTag = tag,
                    PropertyType = APIPropertyType.Image,
                    Image = new APIImage
                    {
                        FileName = tagValue,
                        Image = GetImageBytes(tagValue)
                    }
                });
           // }
        }

        private void SendNumberProperty(string tag, string tagValue)
        {
            double value = 0;
            if (!string.IsNullOrEmpty(tagValue) && double.TryParse(tagValue, out value))
            {
                MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
                {
                    SkinTag = tag,
                    PropertyType = APIPropertyType.Number,
                    Number = value
                });
            }
        }

        private void SendNumberProperty(string tag, double tagValue)
        {
            MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
            {
                SkinTag = tag,
                PropertyType = APIPropertyType.Number,
                Number = tagValue
            });
        }



        private byte[] GetImageBytes(string tagValue)
        {
            if (!string.IsNullOrEmpty(tagValue) && File.Exists(tagValue))
            {
                try
                {
                    return File.ReadAllBytes(tagValue);
                }
                catch (Exception ex)
                {
                    Log.Message(LogLevel.Error, "An exception occured processing property image, FileName: {0}{1}Excption:{1}{2}", tagValue, Environment.NewLine, ex.ToString());
                }
            }
            return null;
        }


        private void SystemInfo_OnNumberDataChanged(string tag, double tagValue)
        {
            SendNumberProperty(tag, tagValue);
        }

        private void SystemInfo_OnTextDataChanged(string tag, string tagValue)
        {
            SendLabelProperty(tag, tagValue);
        }
    }
}
