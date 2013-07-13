using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUISkinFramework.Property
{
    public class XmlPropertyInfo 
    {

        private ObservableCollection<XmlProperty> _properties = new ObservableCollection<XmlProperty>(); 
        public ObservableCollection<XmlProperty> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
    }
}
