using System.Collections.Generic;
using System.Xml.Linq;

namespace Ekom.NetPayment
{
    /// <summary>
    /// Handles the Payment Providers XML configuration
    /// </summary>
    public interface IXMLConfigurationService
    {
        /// <summary>
        /// Tries to get the xml configuration from cache, falls back to file.
        /// </summary>
        XDocument Configuration { get; }

        /// <summary>
        /// Parses the configuration for a given payment provider and returns all nodes as
        /// key values pairs.
        /// </summary>
        /// <param name="pp">Payment Provider title attribute</param>
        /// <param name="basePPName">
        /// Base Payment Provider name, this allows overloaded payment providers to override umbraco properties
        /// but share a common xml configuration
        /// </param>
        /// <param name="secondaryMatches">
        /// Optional collection of attributes and values to match on provider element.
        /// F.x. lang
        /// </param>
        Dictionary<string, string> GetConfigForPP(
            string pp,
            string basePPName,
            Dictionary<string, string> secondaryMatches = null);
    }
}
