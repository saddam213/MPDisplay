using System;
using Common.Log;
using Common.MessengerService;
using GUIFramework.GUI;
using GUISkinFramework.Skin;

namespace GUIFramework.Managers
{
    /// <summary>
    /// Helper class to manage GUIActions
    /// </summary>
    public static class GUIActionManager
    {
        private static readonly Log Log = LoggingManager.GetLog(typeof(GUIActionManager));

        /// <summary>
        /// Gets the action service.
        /// </summary>
        public static MessengerService<XmlActionType> ActionService { get; } = new MessengerService<XmlActionType>();

        /// <summary>
        /// Registers the action to the specified callback.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterAction(XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register(action, callback);
        }

        /// <summary>
        /// Registers the action to the specified callback.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="action">The action.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterAction(this IControlHost window, XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register(action, callback);
        }

        /// <summary>
        /// Registers the action to the specified callback.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="callback">The callback.</param>
        public static void RegisterAction(this GUIControl control, XmlActionType action, Action<XmlAction> callback)
        {
            ActionService.Register(action, callback);
        }

        /// <summary>
        /// Deregisters the action from the specified owner.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="owner">The owner.</param>
        public static void DeregisterAction(XmlActionType action, object owner)
        {
            ActionService.Deregister(action, owner);
        }

        /// <summary>
        /// Deregisters the action from the specified owner.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="action">The action.</param>
        public static void DeregisterAction(this IControlHost window, XmlActionType action)
        {
            ActionService.Deregister(action, window);
        }

        /// <summary>
        /// Deregisters the action from the specified owner.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="action">The action.</param>
        public static void DeregisterAction(this GUIControl control, XmlActionType action)
        {
            ActionService.Deregister(action, control);
        }

        /// <summary>
        /// Gets the param1 as.
        /// </summary>
        /// <typeparam name="T">The type to return as</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetParam1As<T>(this XmlAction action, T defaultValue) where T : IConvertible
        {
            if (action?.Param1 == null) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(action.Param1, typeof(T));
            }
            catch(Exception ex)
            {
                Log.Message(LogLevel.Error, "[GetParam1As] - An exception getting parameter {0} of type {1}, exception: {2}", action.Param1, typeof(T), ex);
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the param2 as.
        /// </summary>
        /// <typeparam name="T">The type to return as</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T GetParam2As<T>(this XmlAction action, T defaultValue) where T : IConvertible
        {
            if (action?.Param2 == null) return defaultValue;

            try
            {
                return (T)Convert.ChangeType(action.Param2, typeof(T));
            }
            catch( Exception ex)
            {
                Log.Message(LogLevel.Error, "[GetParam2As] - An exception getting parameter {0} of type {1}, exception: {2}", action.Param2, typeof(T), ex);
            }
            return defaultValue;
        }
    }
}
