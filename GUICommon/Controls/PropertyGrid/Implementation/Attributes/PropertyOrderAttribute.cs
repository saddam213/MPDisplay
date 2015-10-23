using System;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        public int Order { get; set; }
        
        public PropertyOrderAttribute(int order)
        {
            Order = order;
        }
    }
}
