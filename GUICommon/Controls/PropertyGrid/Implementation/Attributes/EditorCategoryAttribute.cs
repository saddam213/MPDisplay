using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPDisplay.Common.Controls.PropertyGrid.Attributes
{
    public class EditorCategoryAttribute : CategoryAttribute
    {
        private int order;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        public EditorCategoryAttribute(string category, int order) : base(category)
        {
            Order = order;
        }

    }
}
