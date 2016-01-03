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
        private static readonly Log Log = LoggingManager.GetLog(typeof(GUIImageManager));
        private static readonly Dictionary<string, ImageBrush> Cache = new Dictionary<string, ImageBrush>();
        private static readonly Dictionary<string, ImageBrush> StyleCache = new Dictionary<string, ImageBrush>();
        private static readonly Dictionary<string, XmlImageFile> XmlImages = new Dictionary<string, XmlImageFile>();

        /// <summary>
        /// Initializes the ImageManager using the specified SkinInfo.
        /// </summary>
        /// <param name="skinInfo">The skin info.</param>
        public static void Initialize(XmlSkinInfo skinInfo)
        {
            Cache.Clear();
            StyleCache.Clear();
            XmlImages.Clear();
            foreach (var xmlImage in skinInfo.Images.Where(xmlImage => !XmlImages.ContainsKey(xmlImage.XmlName)))
            {
                XmlImages.Add(xmlImage.XmlName, xmlImage);
            }
        }

        /// <summary>
        /// Gets a ImageBrush from the specified XmlImageBrush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        public static ImageBrush GetSkinImage(XmlImageBrush brush)
        {
            if (!XmlImages.ContainsKey(brush.ImageName)) return null;

            if (!string.IsNullOrEmpty(brush.StyleId))
            {
                if (StyleCache.ContainsKey(brush.StyleId)) return StyleCache[brush.StyleId];

                var imageSource = GetImage(XmlImages[brush.ImageName].FileName);
                var newBrush = new ImageBrush(imageSource) {Stretch = brush.ImageStretch};
                StyleCache.Add(brush.StyleId, (ImageBrush)newBrush.GetAsFrozen());
                return StyleCache[brush.StyleId];
            }

            var cacheKey = $"{brush.ImageName} | {brush.ImageStretch}";
            if (Cache.ContainsKey(cacheKey)) return Cache[cacheKey];

            var imageSource1 = GetImage(XmlImages[brush.ImageName].FileName);
            var newBrush1 = new ImageBrush(imageSource1) {Stretch = brush.ImageStretch};
            Cache.Add(cacheKey, (ImageBrush)newBrush1.GetAsFrozen());
            return Cache[cacheKey];
        }

        /// <summary>
        /// Gets a BitmapImage from the specified filename.
        /// </summary>
        /// <param name="filename">The filename or URL.</param>
        /// <returns></returns>
        public static BitmapImage GetImage(string filename)
        {
               try
                {
                    if (!string.IsNullOrWhiteSpace(filename) && ( (FileHelpers.IsUrl(filename) && FileHelpers.ExistsUrl(filename)) || File.Exists(filename)))
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
                    Log.Exception("[GetImage] - An exception occured creating BitmapImage", ex);
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
                Log.Exception("[GetImage] - An exception occured creating BitmapImage", ex);
            }
            return new BitmapImage();
        }
    }
}
