using System.Collections.ObjectModel;
using GUISkinFramework.Controls;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIGroup.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlGroup))]  
    public partial class GUIGroup : GUIControl, IControlHost
    {
        #region Fields

        private ObservableCollection<GUIControl> _controls = new ObservableCollection<GUIControl>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIGroup"/> class.
        /// </summary>
        public GUIGroup()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlGroup SkinXml
        {
            get { return BaseXml as XmlGroup; }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            CreateControls();
        }

        #endregion

        #region IControlHost

        /// <summary>
        /// Gets or sets the controls.
        /// </summary>
        public ObservableCollection<GUIControl> Controls
        {
            get { return _controls; }
            set { _controls = value; NotifyPropertyChanged("Controls"); }
        }

        /// <summary>
        /// Creates the controls.
        /// </summary>
        public void CreateControls()
        {
            foreach (var xmlControl in SkinXml.Controls)
            {
                Controls.Add(GUIElementFactory.CreateControl(ParentId, xmlControl));
            }
        }

        #endregion
    }
}
