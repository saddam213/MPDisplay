using System.ComponentModel;

namespace MPDisplay.Common.Controls.PropertyGrid
{
    public class EditorCategoryAttribute : CategoryAttribute
    {
        private int _order;

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public EditorCategoryAttribute(string category, int order) : base(category)
        {
            Order = order;
        }

    }
}
