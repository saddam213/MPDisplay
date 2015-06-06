using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace GUIConfig.Settings
{
    public class IpAddressValidationRule : ValidationRule
    {
        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult" /> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
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
            return !Regex.IsMatch(hostName, @"^[A-Za-z0-9.-]+$") ? 
                new ValidationResult(false, "Hostname is not valid. Valid chars are A-Z, a-z, 0-9, (.) and (-). See http://tools.ietf.org/html/rfc952 for further details") :
                new ValidationResult(true, null);
        }
    }
}
