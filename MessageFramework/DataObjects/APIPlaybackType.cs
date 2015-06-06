namespace MessageFramework.DataObjects
{
    // ReSharper disable InconsistentNaming
    // attention: Values must match enum PlaybackType in VisibleConditionEditorDialog.xaml.cs

    public enum APIPlaybackType
    {
        None = 0,
        IsTV,
        IsCDA,
        IsDVD,
        IsVideo,
        IsMusic,
        IsRadio,
        IsTVRecording,
        IsPlugin,
        MyFilms,
        MovingPictures,
        MPTVSeries,
        mvCentral,
        YoutubeFm,
        OnlineVideos,
        MyAnime,
        Rockstar,
        PandoraMusicBox,
        RadioTime,
        Streamradio,
        TuneIn
    }

    public enum APIPlaybackState
    {
        None = 0,
        Playing,
        Stopped
      //  Started
    }
}
