using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GUIFramework.GUI;
using GUISkinFramework;
using GUISkinFramework.Property;
using MessageFramework.DataObjects;
using MPDisplay.Common;
using MPDisplay.Common.Settings;

namespace GUIFramework.Managers
{
    public class PropertyRepository : IRepository
    {
        #region Singleton Implementation

        private PropertyRepository() { }
        private static PropertyRepository _instance;
        public static PropertyRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PropertyRepository();
                }
                return _instance;
            }
        }

        public static void RegisterPropertyMessage(GUIControl control, string property)
        {
            Instance.RegisterProperty(control, property);
        }

        public static void DeregisterPropertyMessage(GUIControl control, string property)
        {
            Instance.DeregisterProperty(control, property);
        }

        public static async Task<T> GetProperty<T>(string property)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await Instance.GetControlLabelValue(property);
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

        #endregion




        private List<APIPropertyMessage> _skinProperties = new List<APIPropertyMessage>();
        private MessengerService<string> _propertyService = new MessengerService<string>();
        private DataRepository<string, APIPropertyMessage> _propertyRepository;
        private DataRepository<string, string> _propertyDefaults;

        public GUISettings Settings { get; set; }
        public XmlSkinInfo SkinInfo { get; set; }

        public void Initialize(GUISettings settings, XmlSkinInfo skininfo)
        {
            Settings = settings;
            SkinInfo = skininfo;
            _propertyRepository = new DataRepository<string, APIPropertyMessage>();
            _propertyDefaults = new DataRepository<string, string>();
            _skinProperties = GetSkinProperties();
            if (SkinInfo != null)
            {
                foreach (var property in SkinInfo.Properties)
                {
                    _propertyDefaults.AddOrUpdate(property.SkinTag, property.DefaultValue);
                }
            }
        }

        public void ClearRepository()
        {
            _propertyRepository.ClearRepository();
        }

        public void ResetRepository()
        {
            ClearRepository();
            Settings = null;
            SkinInfo = null;
        }


        /// <summary>
        /// Adds an APIProperty property.
        /// </summary>
        /// <param name="tag">The property tag.</param>
        /// <param name="tagValue">The property value.</param>
        public void AddProperty(APIPropertyMessage propertyMessage)
        {
            if (propertyMessage != null)
            {
                if (propertyMessage.SkinTag == "#selectedthumb")
                {

                }
                if (_propertyRepository.AddOrUpdate(propertyMessage.SkinTag, propertyMessage))
                {
                   NotifyPropertyChanged(propertyMessage.SkinTag);
                }
            }
        }

        public void DeregisterProperty(GUIControl control, string propertyString)
        {
            if (HasProperties(propertyString))
            {
                foreach (var item in GetPropertiesFromXmlString(propertyString))
                {
                    _propertyService.Deregister(item, control);
                }
            }
        }

        public void RegisterProperty(GUIControl control, string propertyString)
        {
            if (HasProperties(propertyString))
            {
                foreach (var property in GetPropertiesFromXmlString(propertyString))
                {
                    _propertyService.Register(property, control.OnPropertyChanged);
                }
            }
        }

        public Task<string> GetControlLabelValue(string xmlstring)
        {
            return Task.Factory.StartNew<string>(() =>
            {
                string returnValue = string.Empty;
                if (!string.IsNullOrEmpty(xmlstring))
                {
                    var parts = xmlstring.Contains("+") ? xmlstring.Split('+').ToList() : new List<string> { xmlstring };
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("@"))
                        {
                            returnValue += SkinInfo.GetLanguageValue(part);
                            continue;
                        }
                        if (part.StartsWith("#"))
                        {
                            returnValue += GetPropertyLabelValueOrDefault(part);
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
            return Task.Factory.StartNew<byte[]>(() =>
              {
                  if (xmlstring.StartsWith("#") && !xmlstring.Contains("+"))
                  {
                      return GetPropertyImageValueOrDefault(xmlstring);
                  }
                  else
                  {
                      string filename = GetControlLabelValue(xmlstring).Result;
                      return SkinInfo.GetImageValue(filename);
                  }
              });
        }

        public Task<double> GetControlNumberValue(string xmlstring)
        {
            return Task.Factory.StartNew<double>(() =>
            {
                return GetPropertyNumberValueOrDefault(xmlstring);
            });
        }

        public List<XmlProperty> GetControlProperties(GUIControl control, string xmlstring)
        {
            var returnValue = new List<XmlProperty>();
            if (HasProperties(xmlstring))
            {
                foreach (var property in GetPropertiesFromXmlString(xmlstring))
                {
                    returnValue.Add(SkinInfo.Properties.FirstOrDefault(x => x.SkinTag == property));
                }
            }
            return returnValue;
        }

        public List<APIPropertyMessage> GetControlHostProperties(IControlHost controlHost)
        {
            if (controlHost != null)
            {
                var skinTags = new List<string>();
                foreach (var control in controlHost.Controls.GetControls())
                {
                    if (control.RegisteredProperties != null && control.RegisteredProperties.Any())
                    {
                        foreach (var property in control.RegisteredProperties)
                        {
                            if (property != null && !string.IsNullOrEmpty(property.SkinTag) && !skinTags.Contains(property.SkinTag))
                            {
                                skinTags.Add(property.SkinTag);
                            }
                        }
                    }
                }
                return _skinProperties.Where(p => p != null && skinTags.Contains(p.SkinTag)).ToList();
            }
            return new List<APIPropertyMessage>();
        }

        private string GetPropertyLabelValueOrDefault(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null && !string.IsNullOrEmpty(result.Label))
            {
                return result.Label;
            }
            return _propertyDefaults.GetValueOrDefault(tag, string.Empty);
        }

        private byte[] GetPropertyImageValueOrDefault(string tag)
        {
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null && result.Image != null)
            {
                var img = result.Image.ToImageBytes();
                if (img != null)
                {
                    return img;
                }
            }
            return SkinInfo.GetImageValue(_propertyDefaults.GetValueOrDefault(tag, string.Empty));
        }

        private double GetPropertyNumberValueOrDefault(string tag)
        {
            double returnValue = 0;
            var result = _propertyRepository.GetValueOrDefault(tag, null);
            if (result != null)
            {
                return result.Number;
            }

            if (double.TryParse(_propertyDefaults.GetValueOrDefault(tag, "0"), out returnValue))
            {
                return returnValue;
            }
            return 0;
        }

        private bool HasProperties(string xmlString)
        {
            return !string.IsNullOrEmpty(xmlString) && xmlString.Contains("#");
        }

        private IEnumerable<string> GetPropertiesFromXmlString(string xmlString)
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
            if (SkinInfo != null)
            {
                foreach (var xmlProperty in SkinInfo.Properties)
                {
                    if (!properties.Any(x => x.SkinTag == xmlProperty.SkinTag))
                    {
                        properties.Add(new APIPropertyMessage
                        {
                            SkinTag = xmlProperty.SkinTag,
                            Tags = xmlProperty.MediaPortalTags.Select(x => x.Tag).ToList(),
                            DefaultValue = xmlProperty.DefaultValue,
                            PropertyType = xmlProperty.PropertyType.ToAPIType()
                        });
                    }
                }
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
    }
}
