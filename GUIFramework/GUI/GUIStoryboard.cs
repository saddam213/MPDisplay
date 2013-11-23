using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using GUISkinFramework.Animations;

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
            this.Completed += (s, e) =>
            {
                if (OnAnimationComplete != null)
                {
                    OnAnimationComplete(Condition);
                }
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
