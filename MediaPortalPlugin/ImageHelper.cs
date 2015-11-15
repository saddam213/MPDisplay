using System.IO;
using Common.Helpers;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin
{
    public static class ImageHelper
    {
        public static APIImage CreateImage(string filename)
        {
            if( string.IsNullOrEmpty(filename)) return new APIImage();

            if (FileHelpers.IsUrl(filename) && FileHelpers.ExistsUrl(filename)) // check for url to prevent exception
				return new APIImage(FileHelpers.ReadBytesFromFile(filename));
            var imageFile = File.Exists(filename) ? filename : GUIGraphicsContext.GetThemedSkinFile("\\media\\" + filename);
            return new APIImage(FileHelpers.ReadBytesFromFile(imageFile));
        }
    }
}
