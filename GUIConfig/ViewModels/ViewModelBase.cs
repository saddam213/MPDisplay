using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Common.Settings;

namespace GUIConfig.ViewModels
{
    public class ViewModelBase : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private SettingsBase _dataObject;
        private bool _isSelected;
        private bool _hasPendingChanges;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        public ViewModelBase()
        {
            DataContext = this;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this viewmodel is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is selected; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        public SettingsBase DataObject
        {
            get { return _dataObject; }
            set { _dataObject = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [has pending changes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has pending changes]; otherwise, <c>false</c>.
        /// </value>
        public bool HasPendingChanges
        {
            get { return _hasPendingChanges; }
            set { _hasPendingChanges = value; NotifyPropertyChanged();  }
        }

        /// <summary>
        /// Gets the tabs title.
        /// </summary>
        public virtual string Title
        {
            get { return "ViewModel"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when model tab opens.
        /// </summary>
        public virtual void OnModelOpen()
        {
        }

        /// <summary>
        /// Called when model tab closes.
        /// </summary>
        public virtual void OnModelClose()
        {
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        public virtual void SaveChanges()
        {
        }

        #endregion

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
    }
}
