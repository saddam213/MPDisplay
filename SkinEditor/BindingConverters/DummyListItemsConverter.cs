using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Common.Settings;
using GUISkinFramework.Skin;
using SkinEditor.Controls;

namespace SkinEditor.BindingConverters
{
    public class DummyListItemsConverter : IMultiValueConverter
    {
        private List<string> _allowedsExtensions = new List<string>(new[] { ".BMP", ".JPG", ".GIF", ".PNG" });

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = new ObservableCollection<CoverFlowListBoxItem>();
            bool vertical;                                                  // layout of list to load the suitable images

            if (value[1] is XmlListLayout)                                  // determine layout
            {
                XmlListLayout listLayout = (XmlListLayout)value[1];
                switch (listLayout)
                {
                    case XmlListLayout.Auto:
                    case XmlListLayout.Vertical:
                    case XmlListLayout.VerticalIcon:
                        vertical = true;
                        break;
                    default:
                        vertical = false;
                        break;
                }
            }
            else
            {
                vertical = true;
            }

             if (value[0] is XmlListType)
             {
                 XmlListType listType = (XmlListType)value[0];
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
                                 var extension = Path.GetExtension(file);
                                 if (extension != null && _allowedsExtensions.Contains(extension.ToUpper()))
                                 {
                                     var filename = Path.GetFileNameWithoutExtension(file);
                                     if (filename != null && ((vertical && filename.StartsWith("_")) || (!vertical && !filename.StartsWith("_"))))
                                     {
                                         if (filename.StartsWith("_")) filename = filename.Remove(0, 1);
                                         items.Add(new CoverFlowListBoxItem
                                         {
                                             Label = filename,
                                             Label2 = "Label2",
                                             Label3 = "Label3",
                                             Image = file,
                                             Index = index
                                         });
                                         index++;
                                     }
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
                 }

             }

            return items;
         
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
