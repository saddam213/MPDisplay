using System.Collections.Generic;
using SkinEditor.Themes;

namespace SkinEditor.Views
{
    public class StyleEditorViewSettings : EditorViewModelSettings
    {
        public List<DesignerStyleSetting> StyleItemSettings { get; set; } = new List<DesignerStyleSetting>();
    }

    public class DesignerStyleSetting
    {
        public string SkinName { get; set; }
        public string StyleId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
