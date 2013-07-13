﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinEditor.Views
{
    public class StyleEditorViewSettings : EditorViewModelSettings
    {
        private List<DesignerStyleSetting> _styleItemSettings = new List<DesignerStyleSetting>();
        public List<DesignerStyleSetting> StyleItemSettings
        {
            get { return _styleItemSettings; }
            set { _styleItemSettings = value; }
        }
    }

    public class DesignerStyleSetting
    {
        public string SkinName { get; set; }
        public string StyleId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
