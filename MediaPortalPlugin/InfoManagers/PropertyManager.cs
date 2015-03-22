using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Helpers;
using Common.Logging;
using Common.Settings;
using Common.Status;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.InfoManagers
{
    public class PropertyManager
    {
        #region Singleton Implementation

        private static PropertyManager instance;

        private PropertyManager()
        {
            Log = Common.Logging.LoggingManager.GetLog(typeof(PropertyManager));
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

        private Common.Logging.Log Log;

        private List<APIPropertyMessage> _properties = new List<APIPropertyMessage>();
        private List<string> _registeredProperties = new List<string>();
        private PluginSettings _settings;
        private AddImageSettings _addImageSettings;
        private bool _suspended;
        private SystemStatusInfo _systemInfo;

        //public bool Suspend { get; set; }

        public void Initialize(PluginSettings settings, AddImageSettings addImageSettings)
        {
            _settings = settings;
            _addImageSettings = addImageSettings;

            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;

            if (RegistrySettings.InstallType == MPDisplayInstallType.Plugin && _settings.IsSystemInfoEnabled)
            {
                _systemInfo = new SystemStatusInfo();
                _systemInfo.TagPrefix = "MP";
                _systemInfo.OnTextDataChanged += SystemInfo_OnTextDataChanged;
                _systemInfo.OnNumberDataChanged += SystemInfo_OnNumberDataChanged;
                _systemInfo.StartMonitoring();
            }
        }

        public void Shutdown()
        {

            if (RegistrySettings.InstallType == MPDisplayInstallType.Plugin && _settings.IsSystemInfoEnabled)
            {
                _systemInfo.OnTextDataChanged -= SystemInfo_OnTextDataChanged;
                _systemInfo.OnNumberDataChanged -= SystemInfo_OnNumberDataChanged;
                _systemInfo.StopMonitoring();
                _systemInfo = null;
            }

         
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
                    Log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering MPDisplay skin property tags...");
                    _registeredProperties.Clear();
                    _registeredProperties.AddRange(properties.SelectMany(x => x.Tags).Distinct());
                    foreach (var prop in _registeredProperties)
                    {
                        Log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering property tag: {0}", prop);
                    }
                    Log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering MPDisplay skin property tags complete.");
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
                _registeredProperties.ForEach(prop => ProcessAddProperty(_addImageSettings.AddImagePropertySettings.Where(x => x.PathExists && x.MPProperties.Contains(prop)).ToList()));
            }
        }

        private void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (!_suspended)
            {
                ThreadPool.QueueUserWorkItem((o) => ProcessProperty(tag, tagValue));
                ThreadPool.QueueUserWorkItem((o) => ProcessAddProperty(_addImageSettings.AddImagePropertySettings.Where(x => x.PathExists && x.MPProperties.Contains(tag)).ToList()));
            }
        }

        private void ProcessProperty(string tag, string tagValue)
        {
            if (_registeredProperties.Contains(tag) && !_addImageSettings.AddImagePropertySettings.Where(x => x.MPDSkinTag.Equals(tag)).Any())
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

            if (MessageService.Instance.IsSkinEditorConnected)
            {
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(tagValue))
                {
                    MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                    {
                        DataType = APISkinEditorDataType.Property,
                        PropertyData = new string[] { tag, tagValue }
                    });
               }
            }
        }

        // send the additional images (ClearArt etc.)
        private void ProcessAddProperty(List<AddImagePropertySettings> pList)
        {
            if (pList != null && pList.Any())
            {
                // make sure to send only the first image found per MPD skin tag
                Dictionary<string, bool> imageFound = new Dictionary<string, bool>();
                foreach (var p in pList)
                {
                    if (!imageFound.ContainsKey(p.MPDSkinTag))
                    {
                        imageFound.Add(p.MPDSkinTag, false);
                    }

                    if (!imageFound[p.MPDSkinTag] && _registeredProperties.Contains(p.MPDSkinTag))
                    {
                        try
                        {
                            imageFound[p.MPDSkinTag] = SendAddImage(p, false);
                        }
                        catch { }
                    }


                    if (MessageService.Instance.IsSkinEditorConnected)
                    {
                        SendAddSkinEditorTag(p);
                    }

                }

                foreach (var p in pList)                                // check if any images were not found
                {
                    if (!imageFound[p.MPDSkinTag] && _registeredProperties.Contains(p.MPDSkinTag))
                    {
                        Log.Message(LogLevel.Verbose, "[ProcessAddProperty] - No image file found for tag {0}.", p.MPDSkinTag);
                        SendAddImage(p, true);                          // send an empty image now
                        imageFound[p.MPDSkinTag] = true;                // to avoid sending multiple empty images
                    }
                }
                imageFound = null;
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

        public void SendImageProperty(string tag, string tagValue)
        {
            if (string.IsNullOrEmpty(tagValue) || File.Exists(tagValue))
            {
                MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
                {
                    SkinTag = tag,
                    PropertyType = APIPropertyType.Image,
                    Image = ImageHelper.CreateImage(tagValue)
                });
            }
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

        private void SystemInfo_OnNumberDataChanged(string tag, double tagValue)
        {
            SendNumberProperty(tag, tagValue);
        }

        private void SystemInfo_OnTextDataChanged(string tag, string tagValue)
        {
            SendLabelProperty(tag, tagValue);
        }

        #region additional properties

        public bool SendAddImage(AddImagePropertySettings p, bool sendEmpty)
        {
            bool result = false;
            APIImage image = null;
            if (!sendEmpty)
            {
                string tagvalue = getAddImageTagValue(p);
                foreach (var extension in p.Extensions)
                {
                    var filename = System.IO.Path.Combine(p.FullPath, tagvalue) + extension;
                    if (File.Exists(filename))
                    {
                        image = ImageHelper.CreateImage(filename);
                        result = true;
                        break;
                    }

                }
            }
            MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
            {
                SkinTag = p.MPDSkinTag,
                PropertyType = APIPropertyType.Image,
                Image = image
            });
            return result;
        }

        public void SendAddSkinEditorTag(AddImagePropertySettings p)
        {
           string message = "";
           string tagvalue = getAddImageTagValue(p);
           foreach (var extension in p.Extensions)
           {
                var filename = System.IO.Path.Combine(p.FullPath, tagvalue) + extension;
                if (File.Exists(filename))
                {
                    message = filename;
                    break;
                }
            }
           if (string.IsNullOrEmpty(message))
           {
               message = System.IO.Path.Combine(p.FullPath, tagvalue) + " - <no image file found>";
           }

            MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
            {
                DataType = APISkinEditorDataType.Property,
                PropertyData = new string[] { p.MPDSkinTag, message }
            });
 
           return;
        }

        private string getAddImageTagValue(AddImagePropertySettings p)
        {
            string returnValue = string.Empty;
            if (!string.IsNullOrEmpty(p.PropertyString))
            {
                var parts = p.PropertyString.Contains("+") ? p.PropertyString.Split('+').ToList() : new List<string> { p.PropertyString };
                foreach (var part in parts)
                {
                    if (part.StartsWith("#"))
                    {
                        returnValue += GUIPropertyManager.GetProperty(part);
                        continue;
                    }
                    returnValue += part;
                }
            }
            return returnValue;
        }

        #endregion
    }
}
