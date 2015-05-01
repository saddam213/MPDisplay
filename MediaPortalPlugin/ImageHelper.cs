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
            if (FileHelpers.IsURL(filename) && FileHelpers.ExistsURL(filename)) // check for url to prevent exception
				return new APIImage(FileHelpers.ReadBytesFromFile(filename));
            string imageFile = File.Exists(filename) ? filename : GUIGraphicsContext.GetThemedSkinFile("\\media\\" + filename);
            return new APIImage(FileHelpers.ReadBytesFromFile(imageFile));
        }
    }
}
