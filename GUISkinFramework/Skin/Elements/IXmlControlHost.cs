using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GUISkinFramework.Controls;
using GUISkinFramework.Windows;

namespace GUISkinFramework
{
    public interface IXmlControlHost
    {
        string Name { get; set; }
        int Id { get; set; }
        string DisplayType { get; }
        ObservableCollection<XmlControl> Controls { get; set; }
    }
}
