using System;

namespace GUIFramework.GUI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class GUISkinElementAttribute : Attribute
    {
        protected Type XxmlType;

        public GUISkinElementAttribute(Type xmlType)
        {
            XxmlType = xmlType;
        }

        public Type XmlType
        {
            get { return XxmlType; }
        }
    }
}
