using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GUIFramework.GUI;
using GUISkinFramework.Common;
using MPDisplay.Common;


namespace GUIFramework.Managers
{
    public static class GUIActionManager
    {
        private static MessengerService<XmlActionType> _actionService = new MessengerService<XmlActionType>();
        public static MessengerService<XmlActionType> ActionService
        {
            get { return _actionService; }
        }

        public static void RegisterAction(XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register<XmlAction>(action, callback);
        }

        public static void DeregisterAction(XmlActionType action, object owner)
        {
            ActionService.Deregister(action, owner);
        }

        public static void DeregisterAction(this IControlHost window, XmlActionType action)
        {
            ActionService.Deregister(action, window);
        }

        public static void RegisterAction(this IControlHost window, XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register<XmlAction>(action, callback);
        }

        public static void RegisterAction(this GUIControl control, XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register<XmlAction>(action, callback);
        }

        public static void DeregisterAction(this GUIControl control, XmlActionType action)
        {
            ActionService.Deregister(action, control);
        }

        public static T GetParam1As<T>(this XmlAction action, T defaultValue)
        {
            if (action != null && action.Param1 != null)
            {
                try
                {
                  return (T)Convert.ChangeType(action.Param1, typeof(T));
                }
                catch { }
            }
            return defaultValue;
        }

        public static T GetParam2As<T>(this XmlAction action, T defaultValue)
        {
            if (action != null && action.Param1 != null)
            {
                try
                {
                    return (T)Convert.ChangeType(action.Param1, typeof(T));
                }
                catch { }
            }
            return defaultValue;
        }
    }
}
