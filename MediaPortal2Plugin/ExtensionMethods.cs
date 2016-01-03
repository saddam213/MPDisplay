using System.Diagnostics;
using MessageFramework.DataObjects;


namespace MediaPortal2Plugin
{
    public static class ExtensionMethods
    {
 
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
