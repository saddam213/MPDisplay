using System.Collections.ObjectModel;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Interface used for GUI elements that host controls
    /// </summary>
    public interface IControlHost
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        ObservableCollection<GUIControl> Controls { get; set; }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        void CreateControls();
    }
}
