using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment.Helpers
{
    /// <summary>
    /// We often have multiple ways to configure a payment provider setting.
    /// This helper chooses the one with the highest priority, making sure to discard empty strings and null values.
    /// </summary>
    static class GetOverridingValue
    {
        /// <summary>
        /// We often have multiple ways to configure a payment provider setting.
        /// This helper chooses the one with the highest priority, making sure to discard empty strings and null values.
        /// </summary>
        /// <param name="propertyName">
        /// Provide the property name, allows us to check safely for it's existence in xmlConfig and uProperties.
        /// </param>
        /// <param name="paymentSettingsValue">Highest priority.</param>
        /// <param name="uProperties">Second highest priority</param>
        /// <param name="xmlConfig">Third highest priority</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string Get(
            string propertyName,
            Dictionary<string, string> uProperties, 
            Dictionary<string, string> xmlConfig = null,
            string paymentSettingsValue = null,
            string defaultValue = null)
        {
            if (!string.IsNullOrEmpty(paymentSettingsValue))
            {
                return paymentSettingsValue;
            }

            if (uProperties?.ContainsKey(propertyName) == true && 
                !string.IsNullOrEmpty(uProperties[propertyName]))
            {
                return uProperties[propertyName];
            }

            if (xmlConfig?.ContainsKey(propertyName) == true && 
                !string.IsNullOrEmpty(xmlConfig[propertyName]))
            {
                return xmlConfig[propertyName];
            }

            return defaultValue;
        }
    }
}
