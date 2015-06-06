using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            foreach (var xmlImage in skinInfo.Images.Where(xmlImage => !_xmlImages.ContainsKey(xmlImage.XmlName)))
            {
                _xmlImages.Add(xmlImage.XmlName, xmlImage);
            }
        }

        /// <summary>
        /// Gets a ImageBrush from the specified XmlImageBrush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        public static ImageBrush GetSkinImage(XmlImageBrush brush)
        {
            if (!_xmlImages.ContainsKey(brush.ImageName)) return null;

            if (!string.IsNullOrEmpty(brush.StyleId))
            {
                if (_styleCache.ContainsKey(brush.StyleId)) return _styleCache[brush.StyleId];

                var imageSource = GetImage(_xmlImages[brush.ImageName].FileName);
                var newBrush = new ImageBrush(imageSource) {Stretch = brush.ImageStretch};
                _styleCache.Add(brush.StyleId, (ImageBrush)newBrush.GetAsFrozen());
                return _styleCache[brush.StyleId];
            }

            var cacheKey = string.Format("{0} | {1}", brush.ImageName, brush.ImageStretch);
            if (_cache.ContainsKey(cacheKey)) return _cache[cacheKey];

            var imageSource1 = GetImage(_xmlImages[brush.ImageName].FileName);
            var newBrush1 = new ImageBrush(imageSource1) {Stretch = brush.ImageStretch};
            _cache.Add(cacheKey, (ImageBrush)newBrush1.GetAsFrozen());
            return _cache[cacheKey];
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
                    var bmImage = new BitmapImage();
                    bmImage.BeginInit();
                    bmImage.CacheOption = BitmapCacheOption.OnLoad;
                    bmImage.UriSource = new Uri(filename);
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
                    var image = new BitmapImage();
                    using (var stream = new MemoryStream(bytes, false))
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
