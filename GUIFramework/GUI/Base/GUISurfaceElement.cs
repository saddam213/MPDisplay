using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace GUIFramework.GUI
{
    public class GUISurfaceElement : ContentControl, IControlHost, INotifyPropertyChanged
    {
        #region Fields

        private ObservableCollection<GUIControl> _controls = new ObservableCollection<GUIControl>(); 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether [is open].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is open]; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen => Visibility == Visibility.Visible;

        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        public ObservableCollection<GUIControl> Controls
        {
            get { return _controls; }
            set { _controls = value; NotifyPropertyChanged(); }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Creates the controls.
        /// </summary>
        public virtual void CreateControls()
        {
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="property">The property.</param>
        public void NotifyPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
