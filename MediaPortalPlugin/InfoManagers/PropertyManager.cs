﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Common.Helpers;
using Common.Log;
using Common.Settings;
using Common.Status;
using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortalPlugin.ExifReader;
using MessageFramework.DataObjects;
using MessageFramework.Messages;
using Log = Common.Log.Log;

namespace MediaPortalPlugin.InfoManagers
{
    public class PropertyManager
    {
        #region Singleton Implementation

        private static PropertyManager _instance;

        private PropertyManager()
        {
            _log = LoggingManager.GetLog(typeof(PropertyManager));
        }

        public static PropertyManager Instance
        {
            get { return _instance ?? (_instance = new PropertyManager()); }
        }

        #endregion

        private Log _log;

        private List<APIPropertyMessage> _properties = new List<APIPropertyMessage>();
        private List<string> _registeredProperties = new List<string>();
        private PluginSettings _settings;
        private AddImageSettings _addImageSettings;
        private bool _suspended;
        private SystemStatusInfo _systemInfo;

        private List<ExifProperty> _exifproperties;
        private string _pictureThumbPath;

        private Dictionary<string, PropertyTagId> _pictureTags;
         
        public void Initialize(PluginSettings settings, AddImageSettings addImageSettings)
        {
            _settings = settings;
            _addImageSettings = addImageSettings;
            AddPictureTags();

            GUIPropertyManager.OnPropertyChanged += GUIPropertyManager_OnPropertyChanged;

            if (RegistrySettings.InstallType == MPDisplayInstallType.Plugin && _settings.IsSystemInfoEnabled)
            {
                _systemInfo = new SystemStatusInfo {TagPrefix = "MP"};
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
                ThreadPool.QueueUserWorkItem(o => ProcessProperty(tag, tagValue));
                ThreadPool.QueueUserWorkItem(o => ProcessAddProperty(_addImageSettings.AddImagePropertySettings.Where(x => x.PathExists && x.MPProperties.Contains(tag)).ToList()));
                ThreadPool.QueueUserWorkItem(o => GetPictureInfo(tag));
            }
        }

        private void ProcessProperty(string tag, string tagValue)
        {
            if (_registeredProperties.Contains(tag) && !_addImageSettings.AddImagePropertySettings.Any(x => x.MPDSkinTag.Equals(tag)) && !_pictureTags.ContainsKey(tag))
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
                                    SendLabelProperty(property.SkinTag, tagValue);
                                    break;
                                case APIPropertyType.Image:
                                    SendImageProperty(property.SkinTag, tagValue);
                                    break;
                                case APIPropertyType.Number:
                                    SendNumberProperty(property.SkinTag, tagValue);
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }

            if (MessageService.Instance.IsSkinEditorConnected)
            {
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(tagValue))
                {
                    MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                    {
                        DataType = APISkinEditorDataType.Property,
                        PropertyData = new[] { tag, tagValue }
                    });
               }
            }

            if( tag != null && _pictureTags.ContainsKey(tag)) SendPictureTag(tag, _pictureTags[tag]);
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
                        catch
                        {
                            // ignored
                        }
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
                        _log.Message(LogLevel.Verbose, "[ProcessAddProperty] - No image file found for tag {0}.", p.MPDSkinTag);
                        SendAddImage(p, true);                          // send an empty image now
                        imageFound[p.MPDSkinTag] = true;                // to avoid sending multiple empty images
                    }
                }
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
		    if (!string.IsNullOrEmpty(tagValue) && ((FileHelpers.IsURL(tagValue) && FileHelpers.ExistsURL(tagValue)) || File.Exists(tagValue))) // check for url to prevent exception
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
            double value;
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
                string tagvalue = GetAddImageTagValue(p);
                foreach (var extension in p.Extensions)
                {
                    var filename = Path.Combine(p.FullPath, tagvalue) + extension;
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
           string tagvalue = GetAddImageTagValue(p);
           foreach (var extension in p.Extensions)
           {
                var filename = Path.Combine(p.FullPath, tagvalue) + extension;
                if (File.Exists(filename))
                {
                    message = filename;
                    break;
                }
            }
           if (string.IsNullOrEmpty(message))
           {
               message = Path.Combine(p.FullPath, tagvalue) + " - <no image file found>";
           }

            MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
            {
                DataType = APISkinEditorDataType.Property,
                PropertyData = new[] { p.MPDSkinTag, message }
            });
        }

        private string GetAddImageTagValue(AddImagePropertySettings p)
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

        #region Pictures

        private void GetPictureInfo(string tag)
        {
            if (tag != "#selecteditem" || WindowManager.Instance.CurrentWindow == null || WindowManager.Instance.CurrentWindow.GetID != 2007) return;

            _exifproperties = null;
            _pictureThumbPath = null;

            var slidelist = ReflectionHelper.GetFieldValue<List<string>>(WindowManager.Instance.CurrentWindow, "_slideList", null, BindingFlags.Instance | BindingFlags.Public);
            if (slidelist != null && slidelist.Any())
            {
                var slideindex = ReflectionHelper.GetFieldValue(WindowManager.Instance.CurrentWindow, "_currentSlideIndex", 0, BindingFlags.Instance | BindingFlags.Public);
                var filename = slidelist[slideindex];
                if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) return;

                 var exifreader = new ExifReader.ExifReader(filename);
                 _exifproperties = exifreader.GetExifProperties().ToList();
                _pictureThumbPath = Utils.IsVideo(filename) ? Utils.GetVideosLargeThumbPathname(filename) : Utils.GetPicturesLargeThumbPathname(filename);

                foreach (var t in _pictureTags)
                {
                    SendPictureTag(t.Key, t.Value);
                }
            }
        }

        private void SendPictureTag(string tag, PropertyTagId exiftag)
        {
            string tagvalue;
            bool sendimage = false;

            switch (tag)
            {
                case "#Picture.Slideshow.Thumb":
                    tagvalue = _pictureThumbPath;
                    sendimage = true;
                    break;

                case "#Picture.Exif.MapData":
                    tagvalue = "---";
                    if (_exifproperties != null)
                    {
                        var exifLat = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLatitude);
                        var exifLng = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLongitude);
                        var exifLatRef = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLatitudeRef);
                        var exifLngRef = _exifproperties.FirstOrDefault(x => x.ExifTag == PropertyTagId.GpsLongitudeRef);
                        if (exifLat != null && exifLng != null)
                        {                            
                            tagvalue = exifLatRef + "," + exifLat + "," + exifLngRef + "," + exifLng;
                        }  
                    }
                    break;

                default:
                    tagvalue = "---";
                    if (_exifproperties != null && _exifproperties.Any(x => x.ExifTag == exiftag))
                    {
                        var firstOrDefault = _exifproperties.FirstOrDefault(x => x.ExifTag == exiftag);
                        if (firstOrDefault != null)
                            tagvalue = firstOrDefault.ToString();
                    }
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

            if (MessageService.Instance.IsSkinEditorConnected)
            {
                if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(tagvalue))
                {
                    MessageService.Instance.SendSkinEditorDataMessage(new APISkinEditorData
                    {
                        DataType = APISkinEditorDataType.Property,
                        PropertyData = new[] { tag, tagvalue }
                    });
                }
            }
        }

        private void AddPictureTags()
        {
            _pictureTags = new Dictionary<string,PropertyTagId>
            {
                { "#Picture.Slideshow.Thumb", PropertyTagId.UnknownExifTag },
                { "#Picture.Exif.MapData", PropertyTagId.UnknownExifTag},
                { "#Picture.Exif.DatePictureTaken", PropertyTagId.EXIF_DT_ORIG},
                { "#Picture.Exif.CameraModel" , PropertyTagId.EquipModel},
                { "#Picture.Exif.EquipmentMake", PropertyTagId.EquipMake},
                { "#Picture.Exif.ExposureBias", PropertyTagId.ExifExposureBias},
                { "#Picture.Exif.ExposureTime", PropertyTagId.ExifExposureTime},
                { "#Picture.Exif.ExposureMode", PropertyTagId.ExifExposureMode},
                { "#Picture.Exif.Flash", PropertyTagId.ExifFlash},
                { "#Picture.Exif.Fstop", PropertyTagId.ExifFNumber},
                { "#Picture.Exif.ImageDimensionsX", PropertyTagId.ExifPixXDim },
                { "#Picture.Exif.ImageDimensionsY", PropertyTagId.ExifPixYDim },
                { "#Picture.Exif.MeteringMode", PropertyTagId.ExifMeteringMode },
                { "#Picture.Exif.ResolutionX", PropertyTagId.XResolution },
                { "#Picture.Exif.ResolutionY", PropertyTagId.YResolution },
                { "#Picture.Exif.ShutterSpeed", PropertyTagId.ExifShutterSpeed },
                { "#Picture.Exif.ISOSpeed", PropertyTagId.ExifISOSpeed },
                { "#Picture.Exif.FocalLength", PropertyTagId.ExifFocalLength },
                { "#Picture.Exif.FocalLengthIn35mm", PropertyTagId.EXIF_FOCAL_LENGTH_IN35_MM_FILM },
                { "#Picture.Exif.GPSLatidude", PropertyTagId.GpsLatitude },
                { "#Picture.Exif.GPSLongitude", PropertyTagId.GpsLongitude },
                { "#Picture.Exif.GPSLatidudeRef", PropertyTagId.GpsLatitudeRef },
                { "#Picture.Exif.GPSLongitudeRef", PropertyTagId.GpsLongitudeRef },
            };

        }
        #endregion
    }
}
