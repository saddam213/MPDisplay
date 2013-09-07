using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MPDisplay.Common.Settings;

namespace GUIConfig.ViewModels
{
    public class ViewModelBase : UserControl, INotifyPropertyChanged
    {
        private SettingsBase _dataObject;
        private bool _isSelected;
        private bool _hasPendingChanges;

        public ViewModelBase()
        {
            DataContext = this;
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    OnModelOpen();
                    return;
                }
                OnModelClose();
                NotifyPropertyChanged();
            }
        }

        public SettingsBase DataObject
        {
            get { return _dataObject; }
            set { _dataObject = value; NotifyPropertyChanged(); }
        }

        public bool HasPendingChanges
        {
            get { return _hasPendingChanges; }
            set { _hasPendingChanges = value; NotifyPropertyChanged();  }
        }

        public virtual string Title
        {
            get { return "ViewModel"; }
        }

        public virtual void OnModelOpen()
        {
            
        }

        public virtual void OnModelClose()
        {
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        public virtual void SaveChanges()
        {
           
        }
    }
}
