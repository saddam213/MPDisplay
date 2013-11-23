﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GUIFramework.Managers;
using GUISkinFramework.Common;

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
        public List<Action> _actions = new List<Action>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIActionCollection"/> class.
        /// </summary>
        /// <param name="xmlActions">The XML actions.</param>
        public GUIActionCollection(IEnumerable<XmlAction> xmlActions)
        {
            if (xmlActions != null)
            {
                foreach (var xmlaction in xmlActions)
                {
                    _actions.Add(() => GUIActionManager.ActionService.NotifyListeners(xmlaction.ActionType, xmlaction));
                }
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
                if (_actions.Any())
                {
                    _actions.ForEach(action => action());
                }
            });
        }
    }
}
