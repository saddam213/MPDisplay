using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GUIFramework.Managers;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// 
    /// </summary>
    public class GUIActionCollection
    {
        /// <summary>
        /// The _actions
        /// </summary>
        public List<Action> Actions = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIActionCollection"/> class.
        /// </summary>
        /// <param name="xmlActions">The XML actions.</param>
        public GUIActionCollection(IEnumerable<XmlAction> xmlActions)
        {
            if (xmlActions == null) return;

            foreach (var xmlaction in xmlActions)
            {
                var xmlaction1 = xmlaction;
                Actions.Add(() => GUIActionManager.ActionService.NotifyListeners(xmlaction1.ActionType, xmlaction1));
            }
        }

        /// <summary>
        /// Executes the actions.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteActions()
        {
            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                if (Actions.Any())
                {
                    Actions.ForEach(action => action());
                }
            });
        }
    }
}
