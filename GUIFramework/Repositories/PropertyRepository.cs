using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common.Log;
using Common.MessengerService;
using Common.Settings;
using Common.Status;
using GUIFramework.GUI;
using GUIFramework.Utils;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using MessageFramework.Messages;

namespace GUIFramework.Repositories
{
    public class PropertyRepository : IRepository
    {
        #region Singleton Implementation

        private PropertyRepository()
        {
            _log = LoggingManager.GetLog(typeof (PropertyRepository));
        }

        private static PropertyRepository _instance;
        public static PropertyRepository Instance
        {
            get { return _instance ?? (_instance = new PropertyRepository()); }
        }

        public static void RegisterPropertyMessage(GUIControl control, string property)
        {
            Instance.RegisterProperty(control, property);
        }

        public static void DeregisterPropertyMessage(GUIControl control, string property)
        {
            Instance.DeregisterProperty(control, property);
        }

        public static async Task<T> GetProperty<T>(string property, string format)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await Instance.GetControlLabelValue(property, format);
            }
            if (typeof(T) == typeof(double))
            {
                return (T)(object)await Instance.GetControlNumberValue(property);
            }
            if (typeof(T) == typeof(byte[]))
            {
                return (T)(object)await Instance.GetControlImageValue(property);
            }
            return default(T);
        }

        public static List<XmlProperty> GetRegisteredProperties(GUIControl control, string xmlstring)
        {
            return Instance.GetControlProperties(control, xmlstring);
        }

        public static List<APIPropertyMessage> GetRegisteredProperties(IControlHost controlHost)
        {
            return Instance.GetControlHostProperties(controlHost);
        }

        public static void AddNumberProperty(string tag, double tagValue)
        {
            Instance.AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Number = tagValue,
                PropertyType = APIPropertyType.Number
            });
        }

        public static void AddLabelProperty(string tag, string tagValue)
        {
            Instance.AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Label = tagValue,
                PropertyType = APIPropertyType.Label
            });
        }

        public static void AddImageProperty(string tag, byte[] tagValue)
        {
            Instance.AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Image = new APIImage(tagValue),
                PropertyType = APIPropertyType.Image
            });
        }


        #endregion


        private List<APIPropertyMessage> _skinProperties = new List<APIPropertyMessage>();
        private MessengerService<string> _propertyService = new MessengerService<string>();
        private DataRepository<string, APIPropertyMessage> _propertyRepository;
        private SystemStatusInfo _systemInfo;
        private Log _log;

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _propertyRepository = new DataRepository<string, APIPropertyMessage>();
            _skinProperties = GetSkinProperties();
            StartSystemInfoMonitoring();
        }

        public void ClearRepository()
        {
            _propertyRepository.ClearRepository();
        }

        public void ResetRepository()
        {
            StopSystemInfoMonitoring();
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }


        /// <summary>
        /// Adds an APIProperty property.
        /// </summary>
        /// <param name="propertyMessage">The property message.</param>
        public void AddProperty(APIPropertyMessage propertyMessage)
        {
            if (propertyMessage == null) return;

            _log.Message(LogLevel.Verbose, "AddProperty: SkinTag: {0}, Label: {1}", propertyMessage.SkinTag, propertyMessage.Label );

            if (!_propertyRepository.AddOrUpdate(propertyMessage.SkinTag, propertyMessage)) return;

            NotifyPropertyChanged(propertyMessage.SkinTag);
        }

        public void AddProperty(string tag, double tagValue)
        {
            AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Number = tagValue,
                PropertyType = APIPropertyType.Number
            });
        }

        public void AddProperty(string tag, string tagValue)
        {
            AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Label = tagValue,
                PropertyType = APIPropertyType.Label
            });
        }

        public void AddProperty(string tag, byte[] tagValue)
        {
            AddProperty(new APIPropertyMessage
            {
                SkinTag = tag,
                Image = new APIImage(tagValue),
                PropertyType = APIPropertyType.Image
            });
        }

        public void DeregisterProperty(GUIControl control, string propertyString)
        {
            if (!HasProperties(propertyString)) return;

            foreach (var item in GetPropertiesFromXmlString(propertyString))
            {
                _propertyService.Deregister(item, control);
            }
        }

        public void RegisterProperty(GUIControl control, string propertyString)
        {
            if (!HasProperties(propertyString)) return;

            foreach (var property in GetPropertiesFromXmlString(propertyString))
            {
                _propertyService.Register(property, control.OnPropertyChanged);
            }
        }

        public Task<string> GetControlLabelValue(string xmlstring, string format)
        {
            return Task.Factory.StartNew(() =>
            {
                var returnValue = string.Empty;
                if (!string.IsNullOrEmpty(xmlstring))
                {
                    var parts = xmlstring.Contains("+") ? xmlstring.Split('+').ToList() : new List<string> { xmlstring };
                    foreach (var part in parts)
                    {
                        // ReSharper disable once InvertIf
                        if (part.StartsWith("@"))
                        {
                            returnValue += SkinInfo.GetLanguageValue(part);
                            continue;
                        }
                        // ReSharper disable once InvertIf
                        if (part.StartsWith("#"))
                        {
                            if (string.IsNullOrEmpty(format))
                            {
                                returnValue += GetPropertyLabelValue(part);
                            }
                            else
                            {
                                returnValue += GetPropertyNumberValue(part).ToString(format);
                            }
                            continue;
                        }
                        returnValue += part;
                    }
                }
                return returnValue;
            });
        }

        public Task<byte[]> GetControlImageValue(string xmlstring)
        {
            return Task.Factory.StartNew(() =>
              {
                  if (!string.IsNullOrEmpty(xmlstring))
                  {
                      if (xmlstring.StartsWith("#") && !xmlstring.Contains("+"))
                      {
                          return GetPropertyImageValue(xmlstring);
                      }
                      var filename = GetControlLabelValue(xmlstring, null).Result;
                      return SkinInfo.GetImageValue(filename);
                  }
                  return null;
              });
        }

        public Task<double> GetControlNumberValue(string xmlstring)
        {
            return Task.Factory.StartNew(() => GetPropertyNumberValue(xmlstring));
        }

        public List<XmlProperty> GetControlProperties(GUIControl control, string xmlstring)
        {
            var returnValue = new List<XmlProperty>();
            if (HasProperties(xmlstring))
            {
                returnValue.AddRange(GetPropertiesFromXmlString(xmlstring).Select(property => SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == property)));
            }
            return returnValue;
        }

        public List<APIPropertyMessage> GetControlHostProperties(IControlHost controlHost)
        {
            if (controlHost == null) return new List<APIPropertyMessage>();

            var skinTags = new List<string>();
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var control in controlHost.Controls.GetControls())
            {
                if (control.RegisteredProperties == null || !control.RegisteredProperties.Any()) continue;

                foreach (var property in control.RegisteredProperties.Where(property => property != null && !string.IsNullOrEmpty(property.SkinTag) && !skinTags.Contains(property.SkinTag)))
                {
                    skinTags.Add(property.SkinTag);
                }
            }
            return _skinProperties.Where(p => p != null && skinTags.Contains(p.SkinTag)).ToList();
        }

        private string GetPropertyLabelValue(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            return result != null ? result.Label : string.Empty;
        }

        private byte[] GetPropertyImageValue(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null && result.Image != null)
            {
                return result.Image.ToImageBytes();
            }
            return null;
        }

        private double GetPropertyNumberValue(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            return result != null ? result.Number : 0;
        }

        private static bool HasProperties(string xmlString)
        {
            return !string.IsNullOrEmpty(xmlString) && xmlString.Contains("#");
        }

        private static IEnumerable<string> GetPropertiesFromXmlString(string xmlString)
        {
            if (xmlString.Contains("+"))
            {
                foreach (var property in xmlString.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Trim().StartsWith("#"))
                    .Select(x => x.Trim()))
                {
                    yield return property;
                }
            }
            else
            {
                yield return xmlString.Trim();
            }
        }

        private List<APIPropertyMessage> GetSkinProperties()
        {
            var properties = new List<APIPropertyMessage>();
            if (SkinInfo == null) return properties;

            foreach (var xmlProperty in SkinInfo.Properties.Where(xmlProperty => properties.All(x => x.SkinTag != xmlProperty.SkinTag)))
            {
                properties.Add(new APIPropertyMessage
                {
                    SkinTag = xmlProperty.SkinTag,
                    Tags = xmlProperty.MediaPortalTags.Select(x => x.Tag).ToList(),
                    PropertyType = xmlProperty.PropertyType.ToAPIType()
                });
            }
            return properties;
        }

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="tag">The tag.</param>
        private void NotifyPropertyChanged(string tag)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                _propertyService.NotifyListeners(tag);
            });
        }

        private void StartSystemInfoMonitoring()
        {
            if (!Settings.IsSystemInfoEnabled) return;

            _systemInfo = new SystemStatusInfo {TagPrefix = "MPD"};
            _systemInfo.OnTextDataChanged += SystemInfo_OnTextDataChanged;
            _systemInfo.OnNumberDataChanged += SystemInfo_OnNumberDataChanged;
            _systemInfo.StartMonitoring();
        }

        private void StopSystemInfoMonitoring()
        {
            if (!Settings.IsSystemInfoEnabled) return;
            _systemInfo.OnTextDataChanged -= SystemInfo_OnTextDataChanged;
            _systemInfo.OnNumberDataChanged -= SystemInfo_OnNumberDataChanged;
            _systemInfo.StopMonitoring();
            _systemInfo = null;
        }


        private void SystemInfo_OnNumberDataChanged(string tag, double tagValue)
        {
            AddProperty(tag, tagValue);
        }

        private void SystemInfo_OnTextDataChanged(string tag, string tagValue)
        {
            AddProperty(tag, tagValue);
        }
    }
}
