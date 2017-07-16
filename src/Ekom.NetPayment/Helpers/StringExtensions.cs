using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment.Helpers
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Coerces a string to a boolean value, case insensitive and also registers true for 1 and y
        /// </summary>
        /// <param name="value">String value to convert to bool</param>
        public static bool ConvertToBool(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var bVal = bool.TryParse(value, out bool result);

                if (bVal)
                {
                    return result;
                }
                else
                {
                    return (value == "1" || value.ToLower() == "y");
                }
            }

            return false;
        }
    }
}
