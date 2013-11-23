using System;
using GUIFramework.Managers;
using GUISkinFramework.Controls;

namespace GUIFramework.GUI.Controls
{
    /// <summary>
    /// Interaction logic for GUIProgressBar.xaml
    /// </summary>
    [GUISkinElement(typeof(XmlProgressBar))]  
    public partial class GUIProgressBar : GUIControl
    {
        #region Fields

        private double _progress;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIProgressBar"/> class.
        /// </summary>
        public GUIProgressBar()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the skin XML.
        /// </summary>
        public XmlProgressBar SkinXml
        {
            get { return BaseXml as XmlProgressBar; }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        public double Progress
        {
            get { return _progress; }
            set
            {
                if (HasChanged(_progress, value))
                {
                    _progress = value;
                    NotifyPropertyChanged("Progress");
                }
            }
        }

        #endregion

        #region GUIControl Overrides

        /// <summary>
        /// Creates the control.
        /// </summary>
        public override void CreateControl()
        {
            base.CreateControl();
            RegisteredProperties = PropertyRepository.GetRegisteredProperties(this, SkinXml.ProgressValue);
        }

        /// <summary>
        /// Registers the info data.
        /// </summary>
        public override void OnRegisterInfoData()
        {
            base.OnRegisterInfoData();
            PropertyRepository.RegisterPropertyMessage(this, SkinXml.ProgressValue);
        }

        /// <summary>
        /// Deregisters the info data.
        /// </summary>
        public override void OnDeregisterInfoData()
        {
            base.OnDeregisterInfoData();
            PropertyRepository.DeregisterPropertyMessage(this, SkinXml.ProgressValue);
        }

        /// <summary>
        /// Updates the info data.
        /// </summary>
        public async override void UpdateInfoData()
        {
            base.UpdateInfoData();
            Progress = await PropertyRepository.GetProperty<double>(SkinXml.ProgressValue);
        }

        /// <summary>
        /// Clears the info data.
        /// </summary>
        public override void ClearInfoData()
        {
            base.ClearInfoData();
            Progress = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified value has changed.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>If the value has changed to 1 decmal point</returns>
        private bool HasChanged(double value1, double value2)
        {
            return Math.Round(value1, 1) != Math.Round(value2, 1);
        }

        #endregion
    }
}
