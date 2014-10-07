using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Skin;
using SkinEditor.Controls;
using Common.Settings;

namespace SkinEditor.BindingConverters
{
    public class DummyListItemsConverter : IValueConverter
    {
        private List<string> _allowedsExtensions = new List<string>(new string[] { ".BMP", ".JPG", ".GIF", ".PNG" });

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             var items = new ObservableCollection<CoverFlowListBoxItem>();
             if (value is XmlListType)
             {
                 XmlListType listType = (XmlListType)value;
                 switch (listType)
                 {
                     case XmlListType.None:
                         break;
                     case XmlListType.MediaPortalListControl:
                     case XmlListType.MediaPortalDialogList:

                         var dummyItemPath = RegistrySettings.ProgramDataPath + "SkinEditor\\ListControl";
                         if (Directory.Exists(dummyItemPath))
                         {
                             int index = 0;
                             foreach (var file in Directory.GetFiles(dummyItemPath))
                             {
                                 if (_allowedsExtensions.Contains(System.IO.Path.GetExtension(file).ToUpper()))
                                 {
                                     items.Add(new CoverFlowListBoxItem
                                     {
                                         Label = Path.GetFileNameWithoutExtension(file),
                                         Label2 = "Label2",
                                         Label3 = "Label3",
                                         Image = file,
                                         Index = index
                                     });
                                     index++;
                                 }
                             }
                         }
                         return items;
                     case XmlListType.MediaPortalButtonGroup:
                         items.Add(new CoverFlowListBoxItem { Label = "Layout: List" });
                         items.Add(new CoverFlowListBoxItem { Label = "View by: Shares" });
                         items.Add(new CoverFlowListBoxItem { Label = "Sort by: Name" });
                         items.Add(new CoverFlowListBoxItem { Label = "Current Playlist" });
                         items.Add(new CoverFlowListBoxItem { Label = "Playlists" });
                         items.Add(new CoverFlowListBoxItem { Label = "Music" });
                         items.Add(new CoverFlowListBoxItem { Label = "Now Playing" });
                         break;
                     case XmlListType.MediaPortalMenuControl:
                         items.Add(new CoverFlowListBoxItem { Label = "TV" });
                         items.Add(new CoverFlowListBoxItem { Label = "Plugins" });
                         items.Add(new CoverFlowListBoxItem { Label = "Radio" });
                         items.Add(new CoverFlowListBoxItem { Label = "Music" });
                         items.Add(new CoverFlowListBoxItem { Label = "Video" });
                         items.Add(new CoverFlowListBoxItem { Label = "Weather" });
                         items.Add(new CoverFlowListBoxItem { Label = "Settings" });
                         break;
                     case XmlListType.MPDisplaySkins:
                      
                         break;
                     case XmlListType.MPDisplayStyles:
                         foreach (var item in SkinEditorInfoManager.SkinInfo.Styles.Keys)
                         {
                             items.Add(new CoverFlowListBoxItem
                             {
                                 Label = item
                             });
                         }
                         break;
                     case XmlListType.MPDisplayLanguages:
                         foreach (var item in SkinEditorInfoManager.SkinInfo.Languages)
                         {
                             items.Add(new CoverFlowListBoxItem
                             {
                                 Label = item
                             });
                         }
                         break;
                     default:
                         break;
                 }

             }

            return items;
         
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
