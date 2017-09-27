using System.Collections.Generic;
using System.Xml.Linq;

namespace Umbraco.NetPayment
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
        /// <param name="secondaryMatches">
        /// Optional collection of attributes and values to match on provider element.
        /// F.x. lang
        /// </param>
        Dictionary<string, string> GetConfigForPP(string pp, Dictionary<string, string> secondaryMatches = null);

        /// <summary>
        /// This method attempts to read the payment providers node guid from the PaymentProviders.config xml file.
        /// It can handle failures such as no xml config present, no value present and more,
        /// in some cases creating a new configuration file.
        /// </summary>
        /// <param name="ppConfig">PaymentProviders.config XML</param>
        void SetConfiguration(XDocument ppConfig);
    }
}
