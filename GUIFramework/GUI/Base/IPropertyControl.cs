using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUISkinFramework.Property;

namespace GUIFramework.GUI
{
    public interface IPropertyControl
    {
        List<XmlProperty> RegisteredProperties { get; set; }
        void RegisterProperties();
        void DergisterProperties();
    }

}
