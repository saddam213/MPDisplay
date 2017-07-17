using System.IO;
using Common.Helpers;
using MessageFramework.DataObjects;

namespace MediaPortal2Plugin
{
    public static class ImageHelper
    {
        public static APIImage CreateImage(string filename)
        {
            if( string.IsNullOrEmpty(filename)) return new APIImage();

            if (FileHelpers.IsUrl(filename) && FileHelpers.ExistsUrl(filename)) // check for url to prevent exception
				return new APIImage(FileHelpers.ReadBytesFromFile(filename));
            // TODO: Eventually get file from skin
            // var imageFile = File.Exists(filename) ? filename : GUIGraphicsContext.GetThemedSkinFile("\\media\\" + filename);
            var imageFile = File.Exists(filename) ? filename : null;

            return new APIImage(FileHelpers.ReadBytesFromFile(imageFile));
        }
    }
}
