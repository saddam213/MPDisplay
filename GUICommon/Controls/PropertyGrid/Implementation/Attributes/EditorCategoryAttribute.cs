using System.ComponentModel;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class EditorCategoryAttribute : CategoryAttribute
    {
        public int Order { get; set; }

        public EditorCategoryAttribute(string category, int order) : base(category)
        {
            Order = order;
        }

    }
}
