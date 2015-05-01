using System.Collections.ObjectModel;

namespace GUISkinFramework.Skin
{
    public interface IXmlControlHost
    {
        string Name { get; set; }
        int Id { get; set; }
        string DisplayType { get; }
        ObservableCollection<XmlControl> Controls { get; set; }
        string VisibleCondition { get; set; }
    }
}
