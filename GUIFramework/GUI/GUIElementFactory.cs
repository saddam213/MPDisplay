using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GUISkinFramework.Skin;

namespace GUIFramework.GUI
{
    /// <summary>
    /// Factory class to create GUI elements from Xml elements
    /// </summary>
    public static class GUIElementFactory
    {
        #region Fields

        private static Dictionary<Type, Type> _xmlControlTypeMap;
        private static Dictionary<Type, Type> _xmlWindowTypeMap;
        private static Dictionary<Type, Type> _xmlDialogTypeMap; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the XML control type map.
        /// </summary>
        public static Dictionary<Type, Type> XmlControlTypeMap
        {
            get
            {
                return _xmlControlTypeMap ?? (_xmlControlTypeMap = new Dictionary<Type, Type>(
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(t => (t.BaseType == typeof (GUIControl) || t.BaseType == typeof (GUIDraggableListView)))
                        .ToDictionary(key => key.GetCustomAttribute<GUISkinElementAttribute>().XmlType,
                            value => value.UnderlyingSystemType)));
            }
        }

        /// <summary>
        /// Gets the XML window type map.
        /// </summary>
        public static Dictionary<Type, Type> XmlWindowTypeMap
        {
            get
            {
                return _xmlWindowTypeMap ?? (_xmlWindowTypeMap = new Dictionary<Type, Type>(
                    Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof (GUIWindow))
                        .ToDictionary(key => key.GetCustomAttribute<GUISkinElementAttribute>().XmlType,
                            value => value.UnderlyingSystemType)));
            }
        }

        /// <summary>
        /// Gets the XML dialog type map.
        /// </summary>
        public static Dictionary<Type, Type> XmlDialogTypeMap
        {
            get
            {
                return _xmlDialogTypeMap ?? (_xmlDialogTypeMap = new Dictionary<Type, Type>(
                    Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof (GUIDialog))
                        .ToDictionary(key => key.GetCustomAttribute<GUISkinElementAttribute>().XmlType,
                            value => value.UnderlyingSystemType)));
            }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Creates a GUIControl.
        /// </summary>
        /// <typeparam name="T">the type of control</typeparam>
        /// <param name="windowId"></param>
        /// <param name="skinXml">The skin XML.</param>
        /// <returns></returns>
        public static GUIControl CreateControl<T>(int windowId, T skinXml) where T : XmlControl
        {
            if (!XmlControlTypeMap.ContainsKey(skinXml.GetType())) return null;

            var control = (GUIControl)Activator.CreateInstance(XmlControlTypeMap[skinXml.GetType()]);
            control.Initialize(windowId, skinXml);
            return control;
        }

        /// <summary>
        /// Creates a GUIWindow.
        /// </summary>
        /// <typeparam name="T">the type of window</typeparam>
        /// <param name="skinXml">The skin XML.</param>
        /// <returns></returns>
        public static GUIWindow CreateWindow<T>(T skinXml) where T : XmlWindow
        {
            if (!XmlWindowTypeMap.ContainsKey(skinXml.GetType())) return null;

            var window = (GUIWindow)Activator.CreateInstance(XmlWindowTypeMap[skinXml.GetType()]);
            window.Initialize(skinXml);
            return window;
        }

        /// <summary>
        /// Creates a GUIDialog.
        /// </summary>
        /// <typeparam name="T">the type of dialog</typeparam>
        /// <param name="skinXml">The skin XML.</param>
        /// <returns></returns>
        public static GUIDialog CreateDialog<T>(T skinXml) where T : XmlDialog
        {
            if (!XmlDialogTypeMap.ContainsKey(skinXml.GetType())) return null;

            var dialog = (GUIDialog)Activator.CreateInstance(XmlDialogTypeMap[skinXml.GetType()]);
            dialog.Initialize(skinXml);
            return dialog;
        } 

        #endregion
    }
}
