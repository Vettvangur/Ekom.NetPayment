using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.NetPayment.Exceptions;
using Umbraco.NetPayment.GMO.Umbraco;
using Umbraco.Web;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Retrieves Umbraco data
    /// </summary>
    public class UmbracoService
    {
        /// <summary>
        /// Default set of properties common for all payment providers
        /// </summary>
        public Dictionary<string, string> BasePPProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "successUrl", "" },
                    { "cancelUrl", "" },
                    { "errorUrl", "" },
                };
            }
        }

        readonly UmbracoHelper _umbracoHelper;
        readonly Settings _settings;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="umbracoHelper"></param>
        /// <param name="settings"></param>
        public UmbracoService(UmbracoHelper umbracoHelper, Settings settings)
        {
            _umbracoHelper = umbracoHelper;
            _settings = settings;
        }

        /// <summary>
        /// Get umbraco content by node name
        /// </summary>
        /// <param name="ppNodeName">Payment Provider Node Name</param>
        public virtual IPublishedContent GetPPNode(string ppNodeName)
        {
            var ppContainer = _umbracoHelper.Content(_settings.PPUmbracoNode);

            if (ppContainer == null) throw new NetPaymentException("Payment Provider container node not found.");

            return ppContainer.Children.Where(x => x.IsVisible()).FirstOrDefault(x =>
                       x.Name.Equals(ppNodeName, StringComparison.InvariantCultureIgnoreCase))
                   ?? ppContainer.Children.First(x =>
                       x.HasProperty("basePaymentProvider") && x.GetProperty("basePaymentProvider").Value<string>()
                          .Equals(ppNodeName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Get umbraco content by node Key
        /// </summary>
        /// <param name="ppNodeKey">Payment Provider Node Guid</param>
        public virtual IPublishedContent GetPPNode(Guid ppNodeKey)
        {
            return _umbracoHelper.Content(ppNodeKey);
        }

        /// <summary>
        /// Fill dictionary with values for each dictionary key.
        /// </summary>
        /// <param name="pp">Umbraco payment provider content node</param>
        /// <param name="culture">IS/EN f.x.</param>
        /// <param name="properties">Optional dictionary of keys to get, default uses base properties dictionary</param>
        public virtual Dictionary<string, string> GetPPProperties(IPublishedContent pp, string culture, Dictionary<string, string> properties = null)
        {
            properties = properties ?? BasePPProperties;

            for (var x = 0; x < properties.Count; x++)
            {
                var key = properties.ElementAt(x).Key;
                var prop = pp.GetProperty(key);

                if (prop != null)
                {
                    var value = _umbracoHelper.GetValueFromProperty(prop, pp, culture, forceUrl: true);
                    properties[key] = value?.ToString();
                }
            }

            return properties;
        }
    }
}
