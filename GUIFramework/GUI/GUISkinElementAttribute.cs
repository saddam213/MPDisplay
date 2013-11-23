using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIFramework.GUI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GUISkinElementAttribute : Attribute
    {
        protected Type xmlType;

        public GUISkinElementAttribute(Type xmlType)
        {
            this.xmlType = xmlType;
        }

        public Type XmlType
        {
            get { return this.xmlType; }
        }
    }
}
