using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIFramework
{
    public static class GenericExtensions
    {
        public static bool IsNumber(this string str)
        {
            int value = 0;
            return !string.IsNullOrEmpty(str.Trim())
                && !string.IsNullOrWhiteSpace(str.Trim())
                && int.TryParse(str.Trim(), out value);
        }
    }
}
