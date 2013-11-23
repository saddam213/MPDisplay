using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MPDisplay.Common;
using GUIFramework.GUI;
using GUIFramework.GUI.Windows;
using GUISkinFramework.Property;
using GUISkinFramework.Skin;
using MessageFramework.DataObjects;
using Common.Helpers;
using Common;

namespace GUIFramework
{
   

    public static class GenericExtensions
    {

        public static void BindTo(this UIElement target, DependencyProperty targetProperty, object source, string sourceProperty, BindingMode mode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            BindingOperations.SetBinding(target, targetProperty, new Binding(sourceProperty)
            {
                Source = source,
                Mode = mode,
                Converter = converter

            });
        }


        public static IEnumerable<GUIControl> GetControls(this IEnumerable<GUIControl> controls)
        {
            foreach (var control in controls)
            {
                yield return control;
                if (control is IControlHost)
                {
                    foreach (var grpControl in (control as IControlHost).Controls.GetControls())
                    {
                        yield return grpControl;
                    }
                }
            }
        }

        public static void ForAllControls(this IEnumerable<GUIControl> controls, Action<GUIControl> action)
        {
            foreach (var control in controls.GetControls())
            {
                action(control);
            }
        }

        public static T GetOrDefault<T>(this IEnumerable<T> windows, int id) where T : GUIWindow
        {
            return windows.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible() && w.Id == id)
                ?? windows.FirstOrDefault(w => w.IsDefault);
        }

        public static T GetOrDefault<T>(this IEnumerable<T> windows) where T : GUIWindow
        {
            return windows.FirstOrDefault(w => w.VisibleCondition.ShouldBeVisible())
                ?? windows.FirstOrDefault(w => w.IsDefault);
        }

   


     

        public static APIListType ToAPIType(this XmlListType listType)
        {
            switch (listType)
            {
                case XmlListType.MediaPortalListControl:
                    return APIListType.List;
                case XmlListType.MediaPortalButtonGroup:
                    return APIListType.GroupMenu;
                case XmlListType.MediaPortalMenuControl:
                    return APIListType.Menu;
                default:
                    break;
            }
            return APIListType.None;
        }

        public static APIPropertyType ToAPIType(this XmlPropertyType type)
        {
            switch (type)
            {
                case XmlPropertyType.Label:
                    return APIPropertyType.Label;
                case XmlPropertyType.Number:
                    return APIPropertyType.Number;
                case XmlPropertyType.Image:
                    return APIPropertyType.Image;
                default:
                    break;
            }
            return APIPropertyType.Label;
        }

        public static byte[] ToImageBytes(this APIImage image)
        {
            if (image != null)
            {
                if (!image.IsFile)
                {
                    return image.FileBytes;
                }

                return FileHelpers.ReadBytesFromFile(image.FileName);
            }
            return null;
        }


        public static bool IsEqual(this APIListAction first, APIListAction second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            return first.ItemText.Equals(second.ItemText) && first.ItemIndex.Equals(second.ItemIndex);
        }

        public static bool IsEqual(this List<APIListItem> first, List<APIListItem> second)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return first == null && second == null;
            }
            if (first.Count != second.Count)
            {
                return false;
            }
            for (int i = 0; i < first.Count; i++)
            {
                if (first[i].Label != second[i].Label)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsEqual(this APIImage first, APIImage second)
        {
            if (first == null)
                return false;

            if (!string.IsNullOrEmpty(second.FileName))
            {
                return second.FileName == first.FileName;
            }

            if (!second.IsFile)
            {
                return second.FileBytes.ImageEquals(first.FileBytes);
            }
            return false;
        }


        public static bool IsMusic(this APIPlaybackType type)
        {
            return type != APIPlaybackType.None && !type.IsVideo();
        }

        public static bool IsVideo(this APIPlaybackType type)
        {
            switch (type)
            {
                case APIPlaybackType.IsTV:
                case APIPlaybackType.IsDVD:
                case APIPlaybackType.IsVideo:
                case APIPlaybackType.IsTVRecording:
                case APIPlaybackType.MyFilms:
                case APIPlaybackType.MovingPictures:
                case APIPlaybackType.MPTVSeries:
                case APIPlaybackType.mvCentral:
                case APIPlaybackType.OnlineVideos:
                case APIPlaybackType.MyAnime:
                    return true;
                default:
                    break;
            }
            return false;
        }


    
    }
}
