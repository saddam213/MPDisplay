﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.Helpers;
using Common.Log;
using GUISkinFramework.Skin;

namespace GUIFramework.Managers
{
    public class GUIImageManager
    {
        private static Log _log = LoggingManager.GetLog(typeof(GUIImageManager));
        private static Dictionary<string, ImageBrush> _cache = new Dictionary<string, ImageBrush>();
        private static Dictionary<string, ImageBrush> _styleCache = new Dictionary<string, ImageBrush>();
        private static Dictionary<string, XmlImageFile> _xmlImages = new Dictionary<string, XmlImageFile>();

        /// <summary>
        /// Initializes the ImageManager using the specified SkinInfo.
        /// </summary>
        /// <param name="skinInfo">The skin info.</param>
        public static void Initialize(XmlSkinInfo skinInfo)
        {
            _cache.Clear();
            _styleCache.Clear();
            _xmlImages.Clear();
            foreach (var xmlImage in skinInfo.Images)
            {
                if (!_xmlImages.ContainsKey(xmlImage.XmlName))
                {
                    _xmlImages.Add(xmlImage.XmlName, xmlImage);
                }
            }
        }

        /// <summary>
        /// Gets a ImageBrush from the specified XmlImageBrush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        public static ImageBrush GetSkinImage(XmlImageBrush brush)
        {
            if (_xmlImages.ContainsKey(brush.ImageName))
            {
                if (!string.IsNullOrEmpty(brush.StyleId))
                {
                    if (!_styleCache.ContainsKey(brush.StyleId))
                    {
                        var imageSource = GetImage(_xmlImages[brush.ImageName].FileName);
                        var newBrush = new ImageBrush(imageSource) {Stretch = brush.ImageStretch};
                        _styleCache.Add(brush.StyleId, (ImageBrush)newBrush.GetAsFrozen());
                    }
                    return _styleCache[brush.StyleId];
                }

                string cacheKey = string.Format("{0} | {1}", brush.ImageName, brush.ImageStretch);
                if (!_cache.ContainsKey(cacheKey))
                {
                    var imageSource = GetImage(_xmlImages[brush.ImageName].FileName);
                    var newBrush = new ImageBrush(imageSource) {Stretch = brush.ImageStretch};
                    _cache.Add(cacheKey, (ImageBrush)newBrush.GetAsFrozen());
                }
                return _cache[cacheKey];
            }
            return null;
        }

        /// <summary>
        /// Gets a BitmapImage from tshe specified filename.
        /// </summary>
        /// <param name="filename">The filename or URL.</param>
        /// <returns></returns>
        public static BitmapImage GetImage(string filename)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filename) && ( (FileHelpers.IsURL(filename) && FileHelpers.ExistsURL(filename)) || File.Exists(filename)))
                {
                    BitmapImage bmImage = new BitmapImage();
                    bmImage.BeginInit();
                    bmImage.CacheOption = BitmapCacheOption.None;
                    bmImage.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
                    bmImage.EndInit();
                    if( bmImage.CanFreeze) bmImage.Freeze();
                    return bmImage;
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[GetImage] - An exception occured creating BitmapImage", ex);
            }
            return new BitmapImage();
        }

        /// <summary>
        /// Gets a BitmapImage from a set of image bytes.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static BitmapImage GetImage(byte[] bytes)
        {
            try
            {
                if (bytes != null && bytes.Length > 0)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(bytes, false))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        if( image.CanFreeze) image.Freeze();
                        stream.Flush();
                    }
                    return image;
                }
            }
            catch (Exception ex)
            {
                _log.Exception("[GetImage] - An exception occured creating BitmapImage", ex);
            }
            return new BitmapImage();
        }
    }
}
