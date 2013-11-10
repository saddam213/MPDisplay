using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Helpers;
using MessageFramework.DataObjects;
using MPDisplay.Common.Settings;

namespace MediaPortalPlugin
{
    public static class ImageHelper
    {
        public static APIImage CreateImage(string filename)
        {
            if (RegistrySettings.InstallType == MPDisplayInstallType.Full)
            {
                return new APIImage(filename);
            }
            return new APIImage(FileHelpers.ReadBytesFromFile(filename));
        }
    }
}
