using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GUISkinFramework;

namespace SkinEditor.Views
{
    public class EditorViewModel : UserControl, INotifyPropertyChanged
    {

        public virtual string Title
        {
            get { return "Editor"; }
        }

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
            DependencyProperty.Register("SkinInfo", typeof(XmlSkinInfo), typeof(EditorViewModel), new PropertyMetadata(new XmlSkinInfo(), (d,e) => (d as EditorViewModel).Initialize()));

        

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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
