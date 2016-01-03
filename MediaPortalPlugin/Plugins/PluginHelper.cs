using System.Collections.Generic;
using Common.Helpers;
using Common.Settings;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin.Plugins
{
    public class PluginHelper
    {
        public PluginHelper(GUIWindow pluginWindow, SupportedPluginSettings settings)
        {
            PluginWindow = pluginWindow;
            Settings = settings;
        }

        public SupportedPluginSettings Settings { get; }

        public GUIWindow PluginWindow { get; }

        public bool IsEnabled => PluginWindow != null;

        public int WindowId => IsEnabled ? PluginWindow.GetID : -1;

        public virtual APIPlaybackType PlayType => APIPlaybackType.None;

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
            var index = 0;
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
            var filename = string.Empty;
            if (Settings == null || item == null) return ImageHelper.CreateImage(filename);
            switch (layout)
            {
                case APIListLayout.Vertical:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalListItemThumb1Path, string.Empty);
                    break;
                case APIListLayout.VerticalIcon:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalIconListItemThumb1Path, string.Empty);
                    break;

                case APIListLayout.Horizontal:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.HorizontalListItemThumb1Path, string.Empty);
                    break;
                case APIListLayout.CoverFlow:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.CoverflowListItemThumb1Path, string.Empty);
                    break;
            }

            if (!string.IsNullOrEmpty(filename)) return ImageHelper.CreateImage(filename);

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
            return ImageHelper.CreateImage(filename);
        }

        public virtual APIImage GetListItemImage2(GUIListItem item, APIListLayout layout)
        {
            var filename = string.Empty;
            if (Settings == null || item == null) return ImageHelper.CreateImage(filename);
            switch (layout)
            {
                case APIListLayout.Vertical:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalListItemThumb2Path, string.Empty);
                    break;
                case APIListLayout.VerticalIcon: 
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalIconListItemThumb2Path, string.Empty);
                    break;
                case APIListLayout.Horizontal:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.HorizontalListItemThumb2Path, string.Empty);
                    break;
                case APIListLayout.CoverFlow:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.CoverflowListItemThumb2Path, string.Empty);
                    break;
            }
            return ImageHelper.CreateImage(filename);
        }

        public virtual APIImage GetListItemImage3(GUIListItem item, APIListLayout layout)
        {
            var filename = string.Empty;
            if (Settings == null || item == null) return ImageHelper.CreateImage(filename);
            switch (layout)
            {
                case APIListLayout.Vertical:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalListItemThumb3Path, string.Empty);
                    break;
                case APIListLayout.VerticalIcon:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.VerticalIconListItemThumb3Path, string.Empty);
                    break;
                case APIListLayout.Horizontal:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.HorizontalListItemThumb3Path, string.Empty);
                    break;
                case APIListLayout.CoverFlow:
                    filename = ReflectionHelper.GetPropertyPath(item, Settings.CoverflowListItemThumb3Path, string.Empty);
                    break;
            }
            return ImageHelper.CreateImage(filename);
        }

        public virtual string GetListItemLabel1(GUIListItem item, APIListLayout layout)
        {
            if (item == null) return string.Empty;
            var propertyPath = string.Empty;
            switch (layout)
            {
                case APIListLayout.Vertical:
                    propertyPath = Settings.VerticalListItemLabel1Path;
                    break;
                case APIListLayout.VerticalIcon:
                    propertyPath = Settings.VerticalIconListItemLabel1Path;
                    break;
                case APIListLayout.Horizontal:
                    propertyPath = Settings.HorizontalListItemLabel1Path;
                    break;
                case APIListLayout.CoverFlow:
                    propertyPath = Settings.CoverflowListItemLabel1Path;
                    break;
            }

            if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label"))
            {
                return ReflectionHelper.GetPropertyPath(item, propertyPath, string.Empty);
            }

            return item.Label;
        }

        public virtual string GetListItemLabel2(GUIListItem item, APIListLayout layout)
        {
            if (item == null) return string.Empty;
            var propertyPath = string.Empty;
            switch (layout)
            {
                case APIListLayout.Vertical:
                    propertyPath = Settings.VerticalListItemLabel2Path;
                    break;
                case APIListLayout.VerticalIcon:
                    propertyPath = Settings.VerticalIconListItemLabel2Path;
                    break;
                case APIListLayout.Horizontal:
                    propertyPath = Settings.HorizontalListItemLabel2Path;
                    break;
                case APIListLayout.CoverFlow:
                    propertyPath = Settings.CoverflowListItemLabel2Path;
                    break;
            }

            if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label2"))
            {
                return ReflectionHelper.GetPropertyPath(item, propertyPath, string.Empty);
            }

            return item.Label2;
        }

        public virtual string GetListItemLabel3(GUIListItem item, APIListLayout layout)
        {
            if (item == null) return string.Empty;
            var propertyPath = string.Empty;
            switch (layout)
            {
                case APIListLayout.Vertical:
                    propertyPath = Settings.VerticalListItemLabel3Path;
                    break;
                case APIListLayout.VerticalIcon:
                    propertyPath = Settings.VerticalIconListItemLabel3Path;
                    break;
                case APIListLayout.Horizontal:
                    propertyPath = Settings.HorizontalListItemLabel3Path;
                    break;
                case APIListLayout.CoverFlow:
                    propertyPath = Settings.CoverflowListItemLabel3Path;
                    break;
            }

            if (!string.IsNullOrEmpty(propertyPath) && !propertyPath.Equals("Label3"))
            {
                return ReflectionHelper.GetPropertyPath(item, propertyPath, string.Empty);
            }

            return item.Label3;
        }
     
    }
}
