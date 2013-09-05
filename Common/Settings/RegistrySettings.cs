﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace MPDisplay.Common.Settings
{
    public enum MPDisplayInstallType
    {
        None = 0,
        Full,
        Plugin,
        GUI
    }

    public static class RegistrySettings
    {
        #region Vars

        private static string _programDataPath = null;
        private static MPDisplayInstallType _installType = MPDisplayInstallType.None;
        private static string _mpdServerExePath = null;
        private static string _mpdisplayConfigExePath = null;
        private static string _mpdisplayExePath = null;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the program data path.
        /// </summary>
        public static string ProgramDataPath
        {
            get
            {
                if (_programDataPath == null)
                {
                    _programDataPath = GetRegistryValue("ProgramDataPath");
                }
                return _programDataPath;
            }
        }


        public static string MPDisplaySettingsFile
        {
            get
            {
                if (_programDataPath == null)
                {
                    _programDataPath = GetRegistryValue("ProgramDataPath");
                }
                return string.IsNullOrEmpty(_programDataPath) ? _programDataPath : _programDataPath + "MPDisplay.xml";
            }
        }

        public static string MPDisplayLanguageFolder
        {
            get
            {
                if (_programDataPath == null)
                {
                    _programDataPath = GetRegistryValue("ProgramDataPath");
                }
                return string.IsNullOrEmpty(_programDataPath) ? _programDataPath : _programDataPath + "Language\\";
            }
        }

        public static string MPDisplaySkinFolder
        {
            get
            {
                if (_programDataPath == null)
                {
                    _programDataPath = GetRegistryValue("ProgramDataPath");
                }
                return string.IsNullOrEmpty(_programDataPath) ? _programDataPath : _programDataPath + "Skin\\";
            }
        }

        /// <summary>
        /// Gets the MP display exe path.
        /// </summary>
        public static string MPDisplayExePath
        {
            get
            {
                if (_mpdisplayExePath == null)
                {
                    _mpdisplayExePath = GetRegistryValue("MPDisplayExePath");
                }
                return _mpdisplayExePath;
            }
        }

        /// <summary>
        /// Gets the MP display config exe path.
        /// </summary>
        public static string MPDisplayConfigExePath
        {
            get
            {
                if (_mpdisplayConfigExePath == null)
                {
                    _mpdisplayConfigExePath = GetRegistryValue("MPDisplayConfigExePath");
                }
                return _mpdisplayConfigExePath;
            }
        }

        /// <summary>
        /// Gets the MPD server exe path.
        /// </summary>
        public static string MPDServerExePath
        {
            get
            {
                if (_mpdServerExePath == null)
                {
                    _mpdServerExePath = GetRegistryValue("MPDServerExePath");
                }
                return _mpdServerExePath;
            }
        }

        /// <summary>
        /// Gets the language file.
        /// </summary>
        public static string ConfigLanguage
        {
            get { return GetRegistryValue("LanguageFile"); }
        }

        /// <summary>
        /// Gets the type of the install.
        /// </summary>
        public static MPDisplayInstallType InstallType
        {
            get
            {
                if (_installType == MPDisplayInstallType.None)
                {
                    try
                    {
                        _installType = (MPDisplayInstallType)Enum.Parse(typeof(MPDisplayInstallType), GetRegistryValue("InstallType"));
                    }
                    catch { }
                }
                return _installType;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets the registry value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Value from the registry at the key provided</returns>
        private static string GetRegistryValue(string key)
        {
            return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\MPDisplay", key, null)
                ?? (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\MPDisplay", key, string.Empty);
        }

        public enum MPDisplayKeys
        {
            InstallType,
            LanguageFile,
            MPDServerExePath,
        }

        public static void SetRegistryValue(MPDisplayKeys key, string value)
        {
            if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\MPDisplay", key.ToString(), null) != null)
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\MPDisplay", key.ToString(), value);
            }
            else if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\MPDisplay", key.ToString(), null) != null)
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\MPDisplay", key.ToString(), value);
            }
        }

        #endregion
    }
}