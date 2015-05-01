using System;
using Common.Log;
using Microsoft.Win32;

namespace Common.Settings
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

        private static string _programDataPath;
        private static MPDisplayInstallType _installType = MPDisplayInstallType.None;
        private static string _mpdServerExePath;
        private static string _mpdisplayConfigExePath;
        private static string _mpdisplayExePath;
        private static string _skinEditorExePath;
        private static LogLevel _logLevel = LogLevel.None;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the program data path.
        /// </summary>
        public static string ProgramDataPath
        {
            get { return _programDataPath ?? (_programDataPath = GetRegistryValue("ProgramDataPath")); }
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

        public static string MPDisplayLanguageFile
        {
            get
            {
                if (_programDataPath == null)
                {
                    _programDataPath = GetRegistryValue("ProgramDataPath");
                }
                return string.IsNullOrEmpty(_programDataPath) ? _programDataPath : _programDataPath + "Language\\Language.xml";
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
            get { return _mpdisplayExePath ?? (_mpdisplayExePath = GetRegistryValue("MPDisplayExePath")); }
        }

        /// <summary>
        /// Gets the MP display config exe path.
        /// </summary>
        public static string MPDisplayConfigExePath
        {
            get {
                return _mpdisplayConfigExePath ?? (_mpdisplayConfigExePath = GetRegistryValue("MPDisplayConfigExePath"));
            }
        }

        /// <summary>
        /// Gets the MPD server exe path. SkinEditrorExePath
        /// </summary>
        public static string MPDServerExePath
        {
            get { return _mpdServerExePath ?? (_mpdServerExePath = GetRegistryValue("MPDServerExePath")); }
        }

        /// <summary>
        /// Gets the skin editror exe path.
        /// </summary>
        public static string SkinEditorExePath
        {
            get { return _skinEditorExePath ?? (_skinEditorExePath = GetRegistryValue("SkinEditorExePath")); }
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
                    catch
                    {
                        // ignored
                    }
                }
                return _installType;
            }
        }


        /// <summary>
        /// Gets the log level.
        /// </summary>
        public static LogLevel LogLevel
        {
            get
            {
                if (_logLevel == LogLevel.None)
                {
                    try
                    {
                        _logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), GetRegistryValue("LogLevel"));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                return _logLevel;
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
            LogLevel
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
