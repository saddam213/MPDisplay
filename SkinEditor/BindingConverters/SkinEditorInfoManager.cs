using System.Linq;
using GUISkinFramework.Skin;

namespace SkinEditor.BindingConverters
{
    public static class SkinEditorInfoManager
    {
        private static XmlSkinInfo _skinInfo = new XmlSkinInfo();
        public static XmlSkinInfo SkinInfo => _skinInfo;

        public static void LoadSkinInfo(XmlSkinInfo skinInfo)
        {
            _skinInfo = skinInfo;
        }

        public static string GetLanguageValue(string skinTag)
        {
            if (_skinInfo?.Language == null) return skinTag;

            var entry = _skinInfo.Language.LanguageEntries.FirstOrDefault(x => x.SkinTag == skinTag);
            if (entry == null) return skinTag;

            var value = entry.Values.FirstOrDefault(x => x.Language == _skinInfo.CurrentLanguage) ?? entry.Values.FirstOrDefault();
            return value == null ? skinTag : value.Value;
        }
    }
}
