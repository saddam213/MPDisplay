using System;
using System.Windows.Media;

namespace MPDisplay.Common.Controls
{
    public class ColorItem
    {
        public Color Color { get; set; }
        public string Name { get; set; }

        public ColorItem(Color color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}
