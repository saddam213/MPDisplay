using System;
using GUIFramework.Managers;

namespace GUIFramework.GUI
{
    /// <summary>
    /// A class to control conditional visibility for GUI elements
    /// </summary>
    public class GUIVisibleCondition
    {
        #region Fields

        private Func<bool> _condition;
        private string _xmlString;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIVisibleCondition"/> class.
        /// </summary>
        /// <param name="control">The control.</param>
        public GUIVisibleCondition(GUIControl control)
        {
            _xmlString = control.BaseXml.VisibleCondition;
            _condition = null;
            _condition = GUIVisibilityManager.GetVisibleCondition(control.ParentId, control.Id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIVisibleCondition"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        public GUIVisibleCondition(GUIWindow window)
        {
            _xmlString = window.BaseXml.VisibleCondition;
            _condition = null;
            _condition = GUIVisibilityManager.GetVisibleCondition(window.Id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIVisibleCondition"/> class.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        public GUIVisibleCondition(GUIDialog dialog)
        {
            _xmlString = dialog.BaseXml.VisibleCondition;
            _condition = null;
            _condition = GUIVisibilityManager.GetVisibleCondition(dialog.Id);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the XML string.
        /// </summary>
        public string XmlString
        {
            get { return _xmlString; }
        }

        /// <summary>
        /// Gets a value indicating whether has a condition set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if has condition; otherwise, <c>false</c>.
        /// </value>
        public bool HasCondition
        {
            get { return _condition != null; }
        }

        /// <summary>
        /// Indicates whether the element should be visible.
        /// </summary>
        /// <returns></returns>
        public bool ShouldBeVisible()
        {
            if (HasCondition)
            {
                return _condition();
            }
            return true;
        } 

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return XmlString;
        } 

        #endregion
    }
}
