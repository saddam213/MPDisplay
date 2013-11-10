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

        public virtual List<APIListItem> GetListItems(List<GUIListItem> items, APIListLayout layout) 
        {
            var returnValue = new List<APIListItem>();
            int index = 0;
            foreach (var item in items)
            {
                returnValue.Add(new APIListItem
                {
                    Index = index,
                    Label = item.Label,
                    Label2 = item.Label2,
                    Label3 = item.Label3,
                    Image = GetListItemImage(item, layout)
                });
                index++;
            }
            return returnValue;
        }

        public virtual APIImage GetListItemImage(GUIListItem item, APIListLayout layout)
        {
            string filename = string.Empty;
            if (Settings != null && item != null)
            {
                switch (layout)
                {
                    case APIListLayout.Vertical:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.Horizontal:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.HorizontalListItemThumbPath, string.Empty);
                        break;
                    case APIListLayout.CoverFlow:
                        filename = ReflectionHelper.GetPropertyPath<string>(item, Settings.VerticalListItemThumbPath, string.Empty);
                        break;
                    default:
                        break;
                }
            }
            return new APIImage(filename);
        }

    }
}
