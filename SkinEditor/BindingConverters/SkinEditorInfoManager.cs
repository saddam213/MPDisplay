using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUISkinFramework;

namespace SkinEditor
{
    public static class SkinEditorInfoManager
    {
        private static XmlSkinInfo _skinInfo = new XmlSkinInfo();
        public static XmlSkinInfo SkinInfo
        {
            get { return _skinInfo; }
        }

        public static void LoadSkinInfo(XmlSkinInfo skinInfo)
        {
            _skinInfo = skinInfo;
        }

        public static string GetLanguageValue(string skinTag)
        {
            if (_skinInfo != null && _skinInfo.Language != null)
            {
                var entry = _skinInfo.Language.LanguageEntries.FirstOrDefault(x => x.SkinTag == skinTag);
                if (entry != null)
                {
                    var value = entry.Values.FirstOrDefault(x => x.Language == _skinInfo.CurrentLanguage) ?? entry.Values.FirstOrDefault();
                    return value == null ? skinTag : value.Value;
                }
            }
            return skinTag;
        }
    }
}
