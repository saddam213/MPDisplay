using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Helpers;
using MessageFramework.DataObjects;
using Common.Settings;
using System.IO;
using MediaPortal.GUI.Library;

namespace MediaPortalPlugin
{
    public static class ImageHelper
    {
        public static APIImage CreateImage(string filename)
        {
            string imageFile = File.Exists(filename)
                ? filename : GUIGraphicsContext.GetThemedSkinFile("\\media\\" + filename);

            return new APIImage(FileHelpers.ReadBytesFromFile(imageFile));
        }
    }
}
