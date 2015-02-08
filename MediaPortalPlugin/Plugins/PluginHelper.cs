using Common.Helpers;
using Common.Settings.SettingsObjects;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPortalPlugin.PluginHelpers
{
    public class PluginHelper
    {
        private GUIWindow _window;
        private SupportedPluginSettings _settings;

        public PluginHelper(GUIWindow pluginWindow, SupportedPluginSettings settings)
        {
            _window = pluginWindow;
            _settings = settings;
        }

        public SupportedPluginSettings Settings 
        {
            get{return _settings;}
        }
        public GUIWindow PluginWindow 
        {
            get { return _window; }
        }

        public bool IsEnabled
        {
            get { return _window != null; }
        }

        public int WindowId
        {
            get { return IsEnabled ? _window.GetID : -1; }
        }

        public virtual APIPlaybackType PlayType
        {
            get { return APIPlaybackType.None; }
        }

        public virtual bool IsPlaying(string filename, APIPlaybackType playtype)
        {
            return false;
        }

        public virtual bool MustResendListOnLayoutChange()
        {
            return false;
        }

        public virtual List<APIListItem> GetListItems(List<GUIListItem> items, APIListLayout layout) 
        {
            var returnValue = new List<APIListItem>();
            int index = 0;
            foreach (var item in items)
            {
                returnValue.Add(new APIListItem
                {
                    Index = index,
                    Label = GetListItemLabel1(item, layout),
                    Label2 = GetListItemLabel2(item, layout),
                    Label3 = GetListItemLabel3(item, layout),

                    Image = GetListItemImage1(item, layout),
                    Image2 = GetListItemImage2(item, layout),
                    Image3 = GetListItemImage3(item, layout)
                });
                index++;
            }
            return returnValue;
        }

        public virtual APIImage GetListItemImage1(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (Settings != null && item != null)
            {
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalListItemThumb1Path, string.Empty);
                        break;
                    case APIListLayout.VerticalIcon:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalIconListItemThumb1Path, string.Empty);
                        break;

                    case APIListLayout.Horizontal:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.HorizontalListItemThumb1Path, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.CoverflowListItemThumb1Path, string.Empty);
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrEmpty(filename))
                {
                    if (item.HasThumbnail)
                    {
                        filename = item.ThumbnailImage;
                    }
                    else if (item.HasIconBig)
                    {
                        filename = item.IconImageBig;
                    }
                    else if (item.HasIcon)
                    {
                        filename = item.IconImage;
                    }
                    else if (item.HasPinIcon)
                    {
                        filename = item.PinImage;
                    }
                }
            }
            return ImageHelper.CreateImage(filename);
        }

        public virtual APIImage GetListItemImage2(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (Settings != null && item != null)
            {
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalListItemThumb2Path, string.Empty);
                        break;
                    case APIListLayout.VerticalIcon: 
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalIconListItemThumb2Path, string.Empty);
                        break;
                    case APIListLayout.Horizontal:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.HorizontalListItemThumb2Path, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.CoverflowListItemThumb2Path, string.Empty);
                        break;
                    default:
                        break;
                }
            }
            return ImageHelper.CreateImage(filename);
        }

        public virtual APIImage GetListItemImage3(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (Settings != null && item != null)
            {
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalListItemThumb3Path, string.Empty);
                        break;
                    case APIListLayout.VerticalIcon:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalIconListItemThumb3Path, string.Empty);
                        break;
                    case APIListLayout.Horizontal:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.HorizontalListItemThumb3Path, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.CoverflowListItemThumb3Path, string.Empty);
                        break;
                    default:
                        break;
                }
            }
            return ImageHelper.CreateImage(filename);
        }

        public virtual string GetListItemLabel1(GUIListItem item, APIListLayout layout)
        {
            if (item != null)
            {
                string propertyPath = string.Empty;
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        propertyPath = _settings.VerticalListItemLabel1Path;
                        break;
                    case APIListLayout.VerticalIcon:
                        propertyPath = _settings.VerticalIconListItemLabel1Path;
                        break;
                    case APIListLayout.Horizontal:
                        propertyPath = _settings.HorizontalListItemLabel1Path;
                        break;
                    case APIListLayout.CoverFlow:
                        propertyPath = _settings.CoverflowListItemLabel1Path;
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label"))
                {
                    return ReflectionHelper.GetPropertyPath<string>(item, propertyPath, string.Empty);
                }

                return item.Label;
            }
            return string.Empty;
        }

        public virtual string GetListItemLabel2(GUIListItem item, APIListLayout layout)
        {
            if (item != null)
            {
                string propertyPath = string.Empty;
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        propertyPath = _settings.VerticalListItemLabel2Path;
                        break;
                    case APIListLayout.VerticalIcon:
                        propertyPath = _settings.VerticalIconListItemLabel2Path;
                        break;
                    case APIListLayout.Horizontal:
                        propertyPath = _settings.HorizontalListItemLabel2Path;
                        break;
                    case APIListLayout.CoverFlow:
                        propertyPath = _settings.CoverflowListItemLabel2Path;
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label2"))
                {
                    return ReflectionHelper.GetPropertyPath<string>(item, propertyPath, string.Empty);
                }

                return item.Label2;
            }
            return string.Empty;
        }

        public virtual string GetListItemLabel3(GUIListItem item, APIListLayout layout)
        {
            if (item != null)
            {
                string propertyPath = string.Empty;
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        propertyPath = _settings.VerticalListItemLabel3Path;
                        break;
                    case APIListLayout.VerticalIcon:
                        propertyPath = _settings.VerticalIconListItemLabel3Path;
                        break;
                    case APIListLayout.Horizontal:
                        propertyPath = _settings.HorizontalListItemLabel3Path;
                        break;
                    case APIListLayout.CoverFlow:
                        propertyPath = _settings.CoverflowListItemLabel3Path;
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label3"))
                {
                    return ReflectionHelper.GetPropertyPath<string>(item, propertyPath, string.Empty);
                }

                return item.Label3;
            }
            return string.Empty;
        }
     
    }
}
