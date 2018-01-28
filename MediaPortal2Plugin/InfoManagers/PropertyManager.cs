using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using Common.Status;
using MediaPortal.Common;
using MessageFramework.Messages;
using MediaPortal.Common.General;
using MediaPortal.UiComponents.SkinBase.Models;
using MediaPortal.UiComponents.Weather.Models;
using MediaPortal.UiComponents.News.Models;
using MediaPortal.UiComponents.Media.Models;
using MediaPortal.UiComponents.BackgroundManager.Models;
using MediaPortal.UiComponents.Weather;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.SkinResources;
using MediaPortal.UI.SkinEngine.SkinManagement;
using Log = Common.Log.Log;

namespace MediaPortal2Plugin.InfoManagers
{
    public class PropertyManager
    {
        #region Singleton Implementation

        private static PropertyManager _instance;

        private PropertyManager()
        {
            _log = LoggingManager.GetLog(typeof(PropertyManager));
        }

        public static PropertyManager Instance => _instance ?? (_instance = new PropertyManager());

        #endregion

        private readonly Log _log;

        private readonly List<APIPropertyMessage> _properties = new List<APIPropertyMessage>();
        private readonly List<string> _registeredProperties = new List<string>();
        private readonly Dictionary<string,object> _registeredMP2Models = new Dictionary<string, object>();
        private readonly Dictionary<object, string> _registeredMP2Properties = new Dictionary<object, string>();
        private readonly Dictionary<AbstractProperty, string> _registeredMP2TimedProperties = new Dictionary<AbstractProperty, string>();

        private readonly Dictionary<string, Func<object, bool>> _modelProcessors = new Dictionary<string, Func<object, bool>>();

        private PluginSettings _settings;
        private bool _suspended;
        private SystemStatusInfo _systemInfo;

        private TimeModel _timeModel;
        private CurrentNewsModel _currentNewsModel;
        private CurrentWeatherModel _currentWeatherModel;

        // private List<ExifProperty> _exifproperties;
        private string _pictureThumbPath = String.Empty;

        //        private Dictionary<string, PropertyTagId> _pictureTags;
        private Dictionary<string, object> _pictureTags;

        public void Initialize(PluginSettings settings)
        {
            _log.Message(LogLevel.Debug, "[Initialize] - Initializing PropertyManager...");
            _settings = settings;
            AddPictureTags();

            InitTimerModels();
            InitStaticModels();
           _log.Message(LogLevel.Debug, "[Initialize] - Initialize complete");


            if (RegistrySettings.InstallType != MPDisplayInstallType.Plugin || !_settings.IsSystemInfoEnabled) return;

            _systemInfo = new SystemStatusInfo {TagPrefix = "MP"};
            _systemInfo.OnTextDataChanged += SystemInfo_OnTextDataChanged;
            _systemInfo.OnNumberDataChanged += SystemInfo_OnNumberDataChanged;
            _systemInfo.StartMonitoring();
            _log.Message(LogLevel.Debug, "[Initialize] - SystemInfo monitoring started");

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
            DisposeTimerModels();
        }

        private void InitStaticModels()
        {
            _log.Message(LogLevel.Debug, "[InitStaticModels] - Initializing static models..");
            _modelProcessors.Add("d5b308c1-4585-4051-ab78-e10d17c3cc2d", ProcessNewsModel);
            _modelProcessors.Add("92bdb53f-4159-4dc2-b212-6083c820a214", ProcessWeatherModel);
            _modelProcessors.Add("9e9d0cd9-4fdb-4c0f-a0c4-f356e151bde0", ProcessMenuModel);
            _modelProcessors.Add("ca6428a7-a6e2-4dd3-9661-f89cebaf8e62", ProcessMouseModel);
            _modelProcessors.Add("4cdd601f-e280-43b9-ad0a-6d7b2403c856", ProcessMediaNavigationModel);
            _modelProcessors.Add("1f4caede-7108-483d-b5c8-18bec7ec58e5", ProcessBackgroundManagerModel);
            _modelProcessors.Add("854aba9a-71a1-420b-a657-9641815f9c01", ProcessHomeServerModel);
           _log.Message(LogLevel.Debug, "[InitStaticModels] - <{0}> static models were added", _modelProcessors.Count);
     }

        public void RegisterModel(object model)
        {
            var modelId = ProcessModel(model);
            if (model == null || string.IsNullOrWhiteSpace(modelId)) return;
            if (!_registeredMP2Models.ContainsKey(modelId))
            {
                _registeredMP2Models.Add(modelId, model);
            }

        }

        private string ProcessModel(object model)
        {
            var modelId = string.Empty;

            if (model == null) return modelId;

            if (model.GetType().IsSubclassOf(typeof(BaseTimerControlledModel))) return modelId;

            var s = model.GetType().GetProperty("ModelId")?.GetValue(model, null); 
            if (s != null)
            {
                modelId = s.ToString();
            }

            var modelName = model.GetType().Name;


      _modelProcessors.TryGetValue(modelId, out Func<object, bool> processor);

      if (processor == null)
            {
                 _log.Message(LogLevel.Error, "[ProcessModel] - Processing model type <{0}>, modelId <{1}> failed, no processor found", modelName, modelId);
                
            }
            else
            {
                _log.Message(LogLevel.Debug, "[ProcessModel] - Processing model type <{0}>, modelId <{1}>", modelName, modelId);
                processor(model);
            }
            return modelId;
        }


        private bool ProcessNewsModel( object modelobject)
        {
            if (!(modelobject is NewsModel model)) return false;

            if (model.SelectedFeed != null)
            {
                ListManager.Instance.SendList(model.SelectedFeed.Items,"NewsItems");
            }
            else
            {
                ListManager.Instance.SendList(model.Feeds, "NewsFeeds");
            }
            return true;
        }

        private bool ProcessWeatherModel(object modelobject)
        {
            if (!(modelobject is WeatherModel model)) return false;

            _log.Message(LogLevel.Debug, "[ProcessWeatherModel] - Processing ");

            ProcessProperty("Weather.Temperature", model.CurrentLocation.Condition.Temperature);
            ProcessProperty("Weather.Humidity", model.CurrentLocation.Condition.Humidity);
            ProcessProperty("Weather.City", model.CurrentLocation.Condition.City);
            ProcessProperty("Weather.Precipitation", model.CurrentLocation.Condition.Precipitation);
            ProcessProperty("Weather.BigIcon", model.CurrentLocation.Condition.BigIcon);
            ProcessProperty("Weather.Wind", model.CurrentLocation.Condition.Wind);
            ProcessProperty("Weather.Condition", model.CurrentLocation.Condition.Condition);
            var day = 0;
            foreach (var listItem in model.CurrentLocation.ForecastCollection)
            {
                var forecast = (DayForecast) listItem;
                ProcessProperty($"Weather.Forecast.Day{day}.TempHigh", forecast.High);                   
                ProcessProperty($"Weather.Forecast.Day{day}.TempLow", forecast.Low);                   
                ProcessProperty($"Weather.Forecast.Day{day}.BigIcon", forecast.BigIcon);                   
                ProcessProperty($"Weather.Forecast.Day{day}.SmallIcon", forecast.SmallIcon);                   
                ProcessProperty($"Weather.Forecast.Day{day}.Wind", forecast.Wind);                   
                ProcessProperty($"Weather.Forecast.Day{day}.Humidity", forecast.Humidity);                   
                ProcessProperty($"Weather.Forecast.Day{day}.Precipitation", forecast.Precipitation);                   
                day++;
            }
            return true;
        }

        private bool ProcessBackgroundManagerModel(object modelobject)
        {
          if (!(modelobject is BackgroundManagerModel model)) return false;

            _log.Message(LogLevel.Debug, "[ProcessBackgroundManagerModel] - Processing ");

            ProcessProperty("Background.Image", model.BackgroundImage);
            return true;
        }
        private bool ProcessHomeServerModel(object modelobject)
        {
          if (!(modelobject is HomeServerModel model)) return false;

            _log.Message(LogLevel.Debug, "[ProcessHomeServerModel] - Processing ");

            ProcessProperty("HomeServer", model.HomeServer);

            return true;
        }

        private bool ProcessMouseModel(object modelobject)
        {
          if (!(modelobject is MouseModel model)) return false;

            _log.Message(LogLevel.Debug, "[ProcessMouseModel] - Processing ");

            //model.CurrentLocation.LocationInfo.
            return true;
        }
        private bool ProcessMediaNavigationModel(object modelobject)
        {
          if (!(modelobject is MediaNavigationModel model)) return false;
            var navigationdata = model.NavigationData;
            var screendata = navigationdata?.CurrentScreenData;
            if (screendata == null) return false;

            _log.Message(LogLevel.Debug, "[ProcessMediaNavigationModel] - Processing {0} items", screendata.NumItems);

            ListManager.Instance.SendList(screendata.Items,"MediaNavigationList");

            return true;
        }
        private bool ProcessMenuModel(object modelobject)
        {
          if (!(modelobject is MenuModel model)) return false;

            _log.Message(LogLevel.Debug, "[ProcessMenuModel] - Processing {0} items", model.MenuItems.Count());

            ListManager.Instance.SendList(model.MenuItems,"MenuList");

            return true;
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
                    _log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering MPDisplay skin property tags...");
                    _registeredProperties.Clear();
                    _registeredProperties.AddRange(properties.SelectMany(x => x.Tags).Distinct());
                    foreach (var prop in _registeredProperties)
                    {
                        _log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering property tag: {0}", prop);
                    }
                    _log.Message(LogLevel.Debug, "[RegisterWindowProperties] - Registering MPDisplay skin property tags complete.");
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
            if (_suspended) return;

            //_registeredProperties.ForEach(prop => ProcessProperty(prop, GUIPropertyManager.GetProperty(prop)));
        }

        private void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (_suspended) return;

            ThreadPool.QueueUserWorkItem(o => ProcessProperty(tag, tagValue));
            // ReSharper disable once ImplicitlyCapturedClosure
            ThreadPool.QueueUserWorkItem(o => GetPictureInfo(tag));
        }

        #region TimerModelProperties

        private void InitTimerModels()
        {
             _log.Message(LogLevel.Debug, "[InitTimerModels] - Model: TimerModel");
             _timeModel = new TimeModel();
            RegisterTimedProperties(_timeModel?.CurrentTimeProperty, "Time");
            RegisterTimedProperties(_timeModel?.CurrentDateProperty, "Date");

            _log.Message(LogLevel.Debug, "[InitTimerModels] - Model: CurrentNewsModel");
             _currentNewsModel = new CurrentNewsModel();
            RegisterTimedProperties(_currentNewsModel?.CurrentNewsItemProperty, "CurrentNews");

            _log.Message(LogLevel.Debug, "[InitTimerModels] - Model: CurrentWeatherModel");
            _currentWeatherModel = new CurrentWeatherModel();
            RegisterTimedProperties(_currentWeatherModel?.CurrentLocationProperty, "CurrentWeather");

        }

        private void DisposeTimerModels()
        {
            _log.Message(LogLevel.Debug, "[DisposeTimerModels] - Disposing timed models");
            foreach (var property in _registeredMP2TimedProperties.Keys)
            {
                property.Detach(TimedPropertyHasChanged);
            }

            _timeModel?.Dispose();
            _currentNewsModel?.Dispose();
            _currentWeatherModel?.Dispose();
        }

        private void RegisterTimedProperties(AbstractProperty property, string tag)
        {
            if (property == null) return;

            _log.Message(LogLevel.Debug, "[RegisterTimedProperties] - Property tag <{0}>, type <{1}>", tag, property.PropertyType);

            if (property.PropertyType == typeof(string) || property.PropertyType == typeof(double) || property.PropertyType == typeof(DateTime))
            {
                if (_registeredMP2TimedProperties.ContainsKey(property))
                {
                    _registeredMP2TimedProperties[property] = tag;
                }
                else
                {
                    _log.Message(LogLevel.Debug, "[RegisterTimedProperties] - Register property tag <{0}>, current value is <{1}>", tag, property.GetValue());
                    _registeredMP2TimedProperties.Add(property, tag);
                    property.Attach(TimedPropertyHasChanged);
                }               
            }
            else
            {
                 foreach (var propertyinfo in property.PropertyType.GetProperties())
                {
                    try
                    {
                        if (propertyinfo.Name.Equals("Item")) continue;

                        var value = property.GetValue();
                        if (value == null) continue;

                        var prop = propertyinfo.GetValue(value) as AbstractProperty;
                        if (prop?.PropertyType == null) continue;

                        _log.Message(LogLevel.Debug, "[RegisterTimedProperties] - Register sub-property tag <{0}>, Type <{1}>", tag + "." + propertyinfo.Name, prop?.PropertyType);
                        RegisterTimedProperties(prop, tag + "." + propertyinfo.Name.Replace("Property",""));
                    }
                    catch( Exception ex)
                    {
                        _log.Message(LogLevel.Error, "[RegisterTimedProperties] - Cannot register sub-property tag <{0}>: {1}", tag + "." + propertyinfo.Name, ex);

                    }
                }
            }
        }

        private void TimedPropertyHasChanged(AbstractProperty property, object oldvalue)
        {
            if (_registeredMP2TimedProperties.ContainsKey(property))
            {               
                _log.Message(LogLevel.Debug, "[TimedPropertyHasChanged] - Property <{0}> new value is <{1}>", _registeredMP2TimedProperties[property], property.GetValue());
                ProcessProperty(_registeredMP2TimedProperties[property], property.GetValue().ToString() );            
            }
        }
        #endregion

        private void ProcessProperty(string tag, string tagValue)
        {
            if (string.IsNullOrWhiteSpace(tag) || tagValue == null) return;
            var value = tagValue;

            if (value.Contains("\\"))
            {
                value = ServiceRegistration.Get<ISkinResourceManager>().SkinResourceContext.GetResourceFilePath($@"{SkinResources.IMAGES_DIRECTORY}\{value}");
                if (string.IsNullOrEmpty(value))
                {
                    value = tagValue;
                }
            }
            lock (_registeredProperties)
            {
                if (_registeredProperties.Contains(tag) && !_pictureTags.ContainsKey(tag))
                {
                    try
                    {
                        lock (_properties)
                        {
                            foreach (var property in _properties.Where(x => x.Tags.Contains(tag)).ToList())
                            {
                                switch (property.PropertyType)
                                {
                                    case APIPropertyType.Label:
                                        SendLabelProperty(property.SkinTag, value);
                                        break;
                                    case APIPropertyType.Image:
                                        SendImageProperty(property.SkinTag, value);
                                        break;
                                    case APIPropertyType.Number:
                                        SendNumberProperty(property.SkinTag, value);
                                        break;
                                }
                            }
                        }
                    }
                    catch( Exception ex)
                    {
                        _log.Message(LogLevel.Error, "[ProcessProperty] - An exception occured processing property <{0}> with value <{1}>. Exception: {2}", tag, value, ex.Message);
                    }
                }
            }

            if (MessageService.Instance.IsSkinEditorConnected)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                    {
                        DataType = APISkinEditorDataType.Property,
                        PropertyData = new[] { tag, value }
                    });
               }
            }

            if( _pictureTags.ContainsKey(tag)) SendPictureTag(tag, _pictureTags[tag]);
        }


        public void SendLabelProperty(string tag, string tagValue)
        {
            MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
            {
                SkinTag = tag,
                PropertyType = APIPropertyType.Label,
                Label = tagValue ?? string.Empty
            });


        }

        public void SendImageProperty(string tag, string tagValue)
        {
		    if (!string.IsNullOrEmpty(tagValue) && ((FileHelpers.IsUrl(tagValue) && FileHelpers.ExistsUrl(tagValue)) || File.Exists(tagValue))) // check for url to prevent exception
            {
                MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
                {
                    SkinTag = tag,
                    PropertyType = APIPropertyType.Image,
                    Image = ImageHelper.CreateImage(tagValue)
                });
            }
        }

        private static void SendNumberProperty(string tag, string tagValue)
        {
            if (tagValue != null && double.TryParse(tagValue, out double value))
            {
                MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
                {
                    SkinTag = tag,
                    PropertyType = APIPropertyType.Number,
                    Number = value
                });
            }
        }

        private static void SendNumberProperty(string tag, double tagValue)
        {
            MessageService.Instance.SendPropertyMessage(new APIPropertyMessage
            {
                SkinTag = tag,
                PropertyType = APIPropertyType.Number,
                Number = tagValue
            });
        }

        private static void SystemInfo_OnNumberDataChanged(string tag, double tagValue)
        {
            SendNumberProperty(tag, tagValue);
        }

        private void SystemInfo_OnTextDataChanged(string tag, string tagValue)
        {
            SendLabelProperty(tag, tagValue);
        }

       
        #region Pictures

        private void GetPictureInfo(string tag)
        {
            //if (tag != "#selecteditem" || WindowManager.Instance.CurrentWindow == null || WindowManager.Instance.CurrentWindow.GetID != 2007) return;

            //_exifproperties = null;
            //_pictureThumbPath = null;

            //Thread.Sleep(500);

            //var slidelist = new List<string>();
            //SupportedPluginManager.GuiSafeInvoke(() =>
            //{
            //    slidelist = ReflectionHelper.GetFieldValue<List<string>>(WindowManager.Instance.CurrentWindow, "_slideList", null, BindingFlags.Instance | BindingFlags.Public);
            //});

            //if (slidelist == null || !slidelist.Any()) return;

            //int slideindex = -1;

            //SupportedPluginManager.GuiSafeInvoke(() =>
            //{
            //    slideindex = ReflectionHelper.GetFieldValue(WindowManager.Instance.CurrentWindow, "_currentSlideIndex", 0, BindingFlags.Instance | BindingFlags.Public);
            //});

            //var filename = string.Empty;
            //if (slideindex >= 0 && slideindex < slidelist.Count)
            //{
            //    filename = slidelist[slideindex];
            //}
            //if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) return;

            // var exifreader = new ExifReader.ExifReader(filename);

            //_exifproperties = exifreader.GetExifProperties() != null ? exifreader.GetExifProperties().ToList() : null;
            //_pictureThumbPath = Utils.IsVideo(filename) ? Utils.GetVideosLargeThumbPathname(filename) : Utils.GetPicturesLargeThumbPathname(filename);

            //foreach (var t in _pictureTags)
            //{
            //    SendPictureTag(t.Key, t.Value);
            //}          

         }

        private void SendPictureTag(string tag, object exiftag)
        {
            string tagvalue;
            var sendimage = false;

         
            switch (tag)
            {
                case "#Picture.Slideshow.Thumb":
                    tagvalue = _pictureThumbPath;
                    sendimage = true;
                    break;

                case "#Picture.Exif.MapData":
                    tagvalue = "---";
                    //if (_exifproperties != null)
                    //{
                    //    var exifLat = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLatitude);
                    //    var exifLng = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLongitude);
                    //    var exifLatRef = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLatitudeRef);
                    //    var exifLngRef = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLongitudeRef);
                    //    if (exifLat != null && exifLng != null)
                    //    {
                    //        tagvalue = exifLatRef + "," + exifLat + "," + exifLngRef + "," + exifLng;
                    //    }
                    //}
                    break;

                default:
                    tagvalue = "---";
                    //if (_exifproperties != null && _exifproperties.Any(x => x.ExifTag == exiftag))
                    //{
                    //    var firstOrDefault = _exifproperties.FirstOrDefault(x => x.ExifTag == exiftag);
                    //    if (firstOrDefault != null)
                    //        tagvalue = firstOrDefault.ToString();
                    //}
                    break;

            }

            if (sendimage)
            {
                SendImageProperty(tag, tagvalue);
            }
            else
            {
                SendLabelProperty(tag, tagvalue);
            }

            if (!MessageService.Instance.IsSkinEditorConnected) return;

            if (!string.IsNullOrEmpty(tag) && tagvalue != null)
            {
                MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                {
                    DataType = APISkinEditorDataType.Property,
                    PropertyData = new[] { tag, tagvalue }
                });
            }
        }

        private void AddPictureTags()
        {
            _pictureTags = new Dictionary<string, object>
            {
                //    { "#Picture.Slideshow.Thumb", PropertyTagId.UnknownExifTag },
                //    { "#Picture.Exif.MapData", PropertyTagId.UnknownExifTag},
                //    { "#Picture.Exif.DatePictureTaken", PropertyTagId.EXIF_DT_ORIG},
                //    { "#Picture.Exif.CameraModel" , PropertyTagId.EquipModel},
                //    { "#Picture.Exif.EquipmentMake", PropertyTagId.EquipMake},
                //    { "#Picture.Exif.ExposureBias", PropertyTagId.ExifExposureBias},
                //    { "#Picture.Exif.ExposureTime", PropertyTagId.ExifExposureTime},
                //    { "#Picture.Exif.ExposureMode", PropertyTagId.ExifExposureMode},
                //    { "#Picture.Exif.Flash", PropertyTagId.ExifFlash},
                //    { "#Picture.Exif.Fstop", PropertyTagId.ExifFNumber},
                //    { "#Picture.Exif.ImageDimensionsX", PropertyTagId.ExifPixXDim },
                //    { "#Picture.Exif.ImageDimensionsY", PropertyTagId.ExifPixYDim },
                //    { "#Picture.Exif.MeteringMode", PropertyTagId.ExifMeteringMode },
                //    { "#Picture.Exif.ResolutionX", PropertyTagId.XResolution },
                //    { "#Picture.Exif.ResolutionY", PropertyTagId.YResolution },
                //    { "#Picture.Exif.ShutterSpeed", PropertyTagId.ExifShutterSpeed },
                //    { "#Picture.Exif.ISOSpeed", PropertyTagId.ExifISOSpeed },
                //    { "#Picture.Exif.FocalLength", PropertyTagId.ExifFocalLength },
                //    { "#Picture.Exif.FocalLengthIn35mm", PropertyTagId.EXIF_FOCAL_LENGTH_IN35_MM_FILM },
                //    { "#Picture.Exif.GPSLatidude", PropertyTagId.GpsLatitude },
                //    { "#Picture.Exif.GPSLongitude", PropertyTagId.GpsLongitude },
                //    { "#Picture.Exif.GPSLatidudeRef", PropertyTagId.GpsLatitudeRef },
                //    { "#Picture.Exif.GPSLongitudeRef", PropertyTagId.GpsLongitudeRef }
            };
        }
        #endregion
    }
}
