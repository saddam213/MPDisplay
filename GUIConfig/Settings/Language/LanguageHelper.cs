using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Helpers;
using Common.Settings;

namespace GUIConfig.Settings
{
    public class LanguageHelper
    {
        private static LanguageFile _languageXmlFile;
        private static Language _currentLanguage;

        public static void LoadLanguage()
        {
            if (File.Exists(RegistrySettings.MPDisplayLanguageFile))
            {
                _languageXmlFile = SerializationHelper.Deserialize<LanguageFile>(RegistrySettings.MPDisplayLanguageFile);
            }
        }

        public static bool HasLanguage(string language)
        {
            return Languages != null && Languages.Contains(language);
        }

        public static IEnumerable<string> Languages
        {
            get
            {
                if (_languageXmlFile != null && _languageXmlFile.Languages != null && _languageXmlFile.Languages.Any())
                {
                    return _languageXmlFile.Languages.Select(x => x.LanguageName);
                }
                return null;
            }

        }

        public static void SetLanguage(string language)
        {
            if (_languageXmlFile != null && _languageXmlFile.Languages != null && _languageXmlFile.Languages.Any())
            {
                _currentLanguage = _languageXmlFile.Languages.FirstOrDefault(x => x.LanguageName == language) ?? _languageXmlFile.Languages.FirstOrDefault();
                return;
            }

            _currentLanguage = new Language
            {
                LanguageName = "English",
                LanguageKeys = new List<LanguageKey>()
            };

            _languageXmlFile = new LanguageFile
            {
                Languages = new List<Language>
                {
                  _currentLanguage
                }
            };

            SerializationHelper.Serialize(_languageXmlFile, RegistrySettings.MPDisplayLanguageFile);
        }

        public static string GetLanguageValue(string key)
        {
            if (key == null || _currentLanguage == null)
            {
                return key ?? string.Empty;
            }
            if (_currentLanguage.LanguageKeys.All(k => k.Key != key))
            {
                _currentLanguage.LanguageKeys.Add(new LanguageKey { Key = key, Value = key });
                SerializationHelper.Serialize(_languageXmlFile, RegistrySettings.MPDisplayLanguageFile);
            }

            var firstOrDefault = _currentLanguage.LanguageKeys.FirstOrDefault(k => k.Key == key);
            if (firstOrDefault != null)
                return firstOrDefault.Value ?? "";
            return string.Empty;
        }
    }

    public class LanguageFile
    {
        public List<Language> Languages { get; set; }
    }

    public class Language
    {
        public string LanguageName { get; set; }
        public List<LanguageKey> LanguageKeys { get; set; }
    }

    public class LanguageKey
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
