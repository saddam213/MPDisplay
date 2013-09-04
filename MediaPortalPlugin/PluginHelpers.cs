using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;

namespace MediaPortalPlugin
{
    public static class PluginHelpers
    {

        public static void GUISafeInvoke(System.Action action)
        {
            try
            {
                GUIGraphicsContext.form.Invoke(action);
            }
            catch (Exception ex)
            {

            }
        }


        #region MPTVSeries

        public static string GetMPTVSeriesItemFilename(this GUIListItem item)
        {
            if (item != null && item.TVTag != null)
            {
                var tvseriesProperty = item.TVTag.GetType().GetProperty("Item");
                if (tvseriesProperty != null)
                {
                    try
                    {
                        return (string)tvseriesProperty.GetValue(item.TVTag, new object[] { "EpisodeFilename" });
                    }
                    catch { }
                }
            }
            return string.Empty;
        }

        public static string GetMPTVSeriesItemThumbnail(this GUIListItem item)
        {
            if (item != null && item.TVTag != null)
            {
                var tvseriesProperty = item.TVTag.GetType().GetProperty("Item");
                if (tvseriesProperty != null)
                {
                    try
                    {
                        return (string)tvseriesProperty.GetValue(item.TVTag, new object[] { "EpisodeFilename" });
                    }
                    catch { }
                }
            }
            return string.Empty;
        }

        #endregion
    }
}
