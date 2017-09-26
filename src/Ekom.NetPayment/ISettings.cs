using System;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Various settings for the Umbraco.NetPayment package.
    /// Most settings can be overriden in AppSettings.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// SQL Connection String Name
        /// </summary>
        string ConnectionStringName { get; }
        /// <summary>
        /// Path to payment provider configuration.
        /// That file stores payment provider specific information that often gets xml transformed.
        /// F.x. merchantId and secret
        /// </summary>
        string PPConfigPath { get; set; }
        /// <summary>
        /// Payment providers document type alias.
        /// </summary>
        string PPDocumentTypeAlias { get; set; }
        /// <summary>
        /// Umbraco node id of payment providers container.
        /// </summary>
        Guid PPUmbracoNode { get; set; }
        /// <summary>
        /// Payment providers umbraco node configuration element name.
        /// xml element name of configuration element that holds umbraco payment providers container Guid
        /// </summary>
        string PPUNodeConfElName { get; }
        /// <summary>
        /// This property controls whether we attempt to send emails when exceptions occur in certain places.
        /// Used in response controllers.
        /// Defaults to true.
        /// </summary>
        bool SendEmailAlerts { get; set; }
    }
}
