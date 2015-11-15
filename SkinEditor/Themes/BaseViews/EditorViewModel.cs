using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GUISkinFramework.Skin;
using SkinEditor.Helpers;

namespace SkinEditor.Themes
{
    public class EditorViewModel : UserControl, INotifyPropertyChanged
    {

        public virtual string Title => "Editor";

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            { 
                _isSelected = value; 
                
                NotifyPropertyChanged("SkinInfo");

                if (_isSelected)
                {
                    OnModelOpen();
                    return;
                }
                OnModelClose();
            }
        }



        public XmlSkinInfo SkinInfo
        {
            get { return (XmlSkinInfo)GetValue(SkinInfoProperty); }
            set { SetValue(SkinInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SkinInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SkinInfoProperty =
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(EditorViewModel), new PropertyMetadata(new XmlSkinInfo(), (d,e) => ((EditorViewModel) d).Initialize()));

        

        //private XmlSkinInfo _skinInfo = new XmlSkinInfo();

        //public XmlSkinInfo SkinInfo
        //{
        //    get { return _skinInfo; }
        //    set { _skinInfo = value; NotifyPropertyChanged("SkinInfo"); }
        //}

        private EditorSettingsObject _editorSettings;

        public EditorSettingsObject EditorSettings
        {
            get { return _editorSettings; }
            set { _editorSettings = value; NotifyPropertyChanged("EditorSettings"); }
        }

        private EditorSettingsObject _connectSettings;

        public EditorSettingsObject ConnectSettings
        {
            get { return _connectSettings; }
            set { _connectSettings = value; NotifyPropertyChanged("ConnectSettings"); }
        }

        private ConnectionHelper _connectHelper;

        public ConnectionHelper ConnectionHelper
        {
            get { return _connectHelper; }
            set { _connectHelper = value; NotifyPropertyChanged("ConnectHelper"); }
        }

        private bool _hasPendingChanges;

        public bool HasPendingChanges
        {
            get { return _hasPendingChanges; }
            set { _hasPendingChanges = value; NotifyPropertyChanged("HasPendingChanges"); }
        }

        public virtual void Initialize()
        {
         
        }

        public virtual void OnModelOpen()
        {

        }
        public virtual void OnModelClose()
        {

        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }

        public class SkinPropertyItem : INotifyPropertyChanged
        {
            public string Tag { get; set; }
            private string _value;

            public string Value
            {
                get { return _value; }
                set { _value = value; NotifyPropertyChanged("Value"); }
            }

            private bool _isDefined;

            public bool IsDefined
            {
                get { return _isDefined; }
                set { _isDefined = value; NotifyPropertyChanged("IsDefined"); }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }
}
