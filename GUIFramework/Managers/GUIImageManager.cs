using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework;
using GUISkinFramework.Common;
using GUISkinFramework.Common.Brushes;
using GUISkinFramework.Controls;
using MPDisplay.Common.Log;

namespace GUIFramework.Managers
{
    public class GUIImageManager
    {
        private static Log Log = LoggingManager.GetLog(typeof(GUIImageManager));
        private static Dictionary<string, ImageBrush> _cache = new Dictionary<string, ImageBrush>();
        private static Dictionary<string, ImageBrush> _styleCache = new Dictionary<string, ImageBrush>();
        private static Dictionary<string, XmlImageFile> _xmlImages = new Dictionary<string, XmlImageFile>();

        public static void Init(XmlSkinInfo skinInfo)
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


        public static ImageBrush GetImage(XmlImageBrush brush)
        {
            if (_xmlImages.ContainsKey(brush.ImageName))
            {
                if (!string.IsNullOrEmpty(brush.StyleId))
                {
                    if (!_styleCache.ContainsKey(brush.StyleId))
                    {
                        var newbrush = new ImageBrush(GetImageNonCached(_xmlImages[brush.ImageName].FileName)) { Stretch = brush.ImageStretch};
                        newbrush.Freeze();
                        _styleCache.Add(brush.StyleId,newbrush );
                    }
                    return _styleCache[brush.StyleId];
                }

                string cacheKey = string.Format("{0} | {1}", brush.ImageName, brush.ImageStretch);
                if (!_cache.ContainsKey(cacheKey))
                {
                    var newBrush = new ImageBrush(GetImageNonCached(_xmlImages[brush.ImageName].FileName)) { Stretch = brush.ImageStretch };
                    newBrush.Freeze();
                    _cache.Add(cacheKey, newBrush);
                }
                return _cache[cacheKey];
            }
            return null;
        }


        public static BitmapImage GetImageNonCached(string filename)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
                {
                    BitmapImage bmImage = new BitmapImage();
                    bmImage.BeginInit();
                  //  bmImage.DecodePixelWidth = GetScaledImageWidth(filename);
                    bmImage.CacheOption = BitmapCacheOption.None;
                    bmImage.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
                    bmImage.EndInit();
                    bmImage.Freeze();
                    return bmImage;
                }
            }
            catch (Exception ex)
            {
                Log.Exception("[GetImageNonCached] - ", ex);
            }
            return null;
        }

        private static int GetScaledImageWidth(string filename)
        {
            BitmapImage bmImage = new BitmapImage();
            bmImage.BeginInit();
            bmImage.CacheOption = BitmapCacheOption.None;
            bmImage.UriSource = new Uri(filename, UriKind.RelativeOrAbsolute);
            bmImage.EndInit();
            bmImage.Freeze();
            return GetScaledImageWidth(bmImage.PixelWidth);
        }

        private static int GetScaledImageWidth(int width)
        {
            return width;
        }

        public static BitmapImage GetImageFromBytes(byte[] p)
        {
            BitmapImage bmImage = new BitmapImage();
            if (p != null)
            {
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.None;
                bmImage.StreamSource = new MemoryStream(p);
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }
    }
}
