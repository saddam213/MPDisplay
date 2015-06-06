using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MediaPortal.GUI.Library;
using MessageFramework.DataObjects;

namespace MediaPortalPlugin
{
    public static class ExtensionMethods
    {
        public static IEnumerable<GUIControl> GetControls(this GUIWindow window)
        {
            return window != null ? window.Children.GetControls() : null;
        }

        public static IEnumerable<T> GetControls<T>(this GUIWindow window)
        {
            return window != null ? window.Children.GetControls().OfType<T>() : null;
        }

        public static bool HasControl<T>(this GUIWindow window)
        {
            return window != null && window.Children.GetControls().OfType<T>().Any();
        }

        public static IEnumerable<GUIControl> GetControls(this IEnumerable<GUIControl> controls)
        {
            foreach (var control in controls)
            {
                yield return control;
                if (!(control is GUIGroup)) continue;

                foreach (var item in (control as GUIGroup).Children.GetControls())
                {
                    yield return item;
                }
            }
        }

        public static void CloseAll(this Process[] processes, bool closeMainWindow = true, bool killProcess = false)
        {
            if (processes == null) return;

            foreach (var process in processes)
            {
                if (killProcess)
                {
                    process.Kill();
                }
                else if (closeMainWindow)
                {
                    process.CloseMainWindow();
                }
                else
                {
                    process.Close();
                }
            }
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
            }
            return false;
        }
    }
}
