using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUIConfig.Settings
{
    public class IpAddressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            //I know it's called IP Address internally but it really should be hostname.
            var hostName = value as string;
            
            //check for empty/null file path:
            if (string.IsNullOrEmpty(hostName) || hostName.Any(char.IsWhiteSpace))
            {
                return new ValidationResult(false, "Hostname cannot contain empty space");
            }

            //http://tools.ietf.org/html/rfc952
            //See the above link for the list of valid host names.
            if (!Regex.IsMatch(hostName, @"^[A-Za-z0-9.-]+$"))
            {
                return new ValidationResult(false, "Hostname is not valid. Valid chars are A-Z, a-z, 0-9, (.) and (-). See http://tools.ietf.org/html/rfc952 for further details");
            }

            return new ValidationResult(true, null);
        }
    }
}
