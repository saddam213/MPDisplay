using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;

namespace MediaPortalPlugin
{
    public static class ExtensionMethods
    {
        public static IEnumerable<GUIControl> GetControls(this GUIWindow window)
        {
            if (window != null)
            {
                return window.Children.GetControls();
            }
            return null;
        }

        public static IEnumerable<T> GetControls<T>(this GUIWindow window)
        {
            if (window != null)
            {
                return window.Children.GetControls().OfType<T>();
            }
            return null;
        }

        public static bool HasControl<T>(this GUIWindow window)
        {
            if (window != null)
            {
                return window.Children.GetControls().OfType<T>().Any();
            }
            return false;
        }

        public static IEnumerable<GUIControl> GetControls(this IEnumerable<GUIControl> controls)
        {
            foreach (var conrol in controls)
            {
                yield return conrol;
                if (conrol is GUIGroup)
                {
                    foreach (var item in (conrol as GUIGroup).Children.GetControls())
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
