using System;
using System.Windows.Media.Animation;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Custom StoryBoard object
    /// </summary>
    public class GUIStoryboard : Storyboard
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIStoryboard"/> class.
        /// </summary>
        public GUIStoryboard()
        {
            Completed += (s, e) =>
            {
                OnAnimationComplete?.Invoke(Condition);
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        public XmlAnimationCondition Condition { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when on animation complete.
        /// </summary>
        public event Action<XmlAnimationCondition> OnAnimationComplete; 

        #endregion
    }
}
