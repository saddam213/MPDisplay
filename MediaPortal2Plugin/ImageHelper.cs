using System.IO;
using Common.Helpers;
using MediaPortal.UI.SkinEngine.SkinManagement;
using MessageFramework.DataObjects;

namespace MediaPortal2Plugin
{
    public static class ImageHelper
    {
        public static APIImage CreateImage(string resourcename)
        {
            if( string.IsNullOrEmpty(resourcename)) return new APIImage();
            if (FileHelpers.IsUrl(resourcename) && FileHelpers.ExistsUrl(resourcename)) // check for url to prevent exception
				        return new APIImage(FileHelpers.ReadBytesFromFile(resourcename));
            // todo: not sure if all images are in this director
            var filename = SkinContext.SkinResources.GetResourceFilePath($@"{SkinResources.IMAGES_DIRECTORY}\{resourcename}.fx");
             if ( filename == null ) return new APIImage();
 
             var imageFile = File.Exists(filename) ? filename : null;

            return new APIImage(FileHelpers.ReadBytesFromFile(imageFile));
        }
    }
}
