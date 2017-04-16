namespace Common.Settings
{
    public class MP2IdMapping : SettingsBase
    {
        private string _mp2Guid;
        private int _mpdid;


        public string MP2Guid
        {
            get { return _mp2Guid; }
            set { _mp2Guid = value; NotifyPropertyChanged("MP2Guid"); }
        }

        public int MPDid
        {
            get { return _mpdid; }
            set { _mpdid = value; NotifyPropertyChanged("MPDId"); }
        }

    }
}
