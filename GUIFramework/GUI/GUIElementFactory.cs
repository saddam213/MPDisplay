using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GUISkinFramework.Common;
using GUISkinFramework.Controls;
using GUISkinFramework.Dialogs;
using GUISkinFramework.Windows;

namespace GUIFramework.GUI
{
    public static class GUIElementFactory
    {
        private static Dictionary<Type,Type> _xmlControlTypeMap;
        private static Dictionary<Type, Type> _xmlWindowTypeMap;
        private static Dictionary<Type, Type> _xmlDialogTypeMap;

        public static Dictionary<Type,Type> XmlControlTypeMap
        {
            get
            {
                if (_xmlControlTypeMap == null)
                {
                    _xmlControlTypeMap = new Dictionary<Type, Type>(
                        Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(GUIControl))
                        .ToDictionary(key => key.GetCustomAttribute<XmlSkinTypeAttribute>().XmlType, value => value.UnderlyingSystemType));
                }
                return _xmlControlTypeMap;
            }
        }

        public static Dictionary<Type, Type> XmlWindowTypeMap
        {
            get
            {
                if (_xmlWindowTypeMap == null)
                {
                    _xmlWindowTypeMap = new Dictionary<Type, Type>(
                        Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(GUIWindow))
                        .ToDictionary(key => key.GetCustomAttribute<XmlSkinTypeAttribute>().XmlType, value => value.UnderlyingSystemType));
                }
                return _xmlWindowTypeMap;
            }
        }

        public static Dictionary<Type, Type> XmlDialogTypeMap
        {
            get
            {
                if (_xmlDialogTypeMap == null)
                {
                    _xmlDialogTypeMap = new Dictionary<Type, Type>(
                        Assembly.GetExecutingAssembly().GetTypes().Where(t => t.BaseType == typeof(GUIDialog))
                        .ToDictionary(key => key.GetCustomAttribute<XmlSkinTypeAttribute>().XmlType, value => value.UnderlyingSystemType));
                }
                return _xmlDialogTypeMap;
            }
        }


        public static GUIControl CreateControl<T>(int windowId, T skinXml) where T : XmlControl
        {
            if (XmlControlTypeMap.ContainsKey(skinXml.GetType()))
            {
                var control = (GUIControl)Activator.CreateInstance(XmlControlTypeMap[skinXml.GetType()]);
                control.Initialize(windowId, skinXml);
                return control;
            }
            return null;
        }

        public static GUIWindow CreateWindow<T>(T skinXml) where T : XmlWindow
        {
            if (XmlWindowTypeMap.ContainsKey(skinXml.GetType()))
            {
                var window = (GUIWindow)Activator.CreateInstance(XmlWindowTypeMap[skinXml.GetType()]);
                window.Initialize(skinXml);
                return window;
            }
            return null;
        }

        public static GUIDialog CreateDialog<T>(T skinXml) where T : XmlDialog
        {
            if (XmlDialogTypeMap.ContainsKey(skinXml.GetType()))
            {
                var dialog = (GUIDialog)Activator.CreateInstance(XmlDialogTypeMap[skinXml.GetType()]);
                dialog.Initialize(skinXml);
                return dialog;
            }
            return null;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XmlSkinTypeAttribute : Attribute
    {
        protected Type xmlType;

        public XmlSkinTypeAttribute(Type xmlType)
        {
            this.xmlType = xmlType;
        }

        public Type XmlType
        {
            get { return this.xmlType; }
        }
    }
}
