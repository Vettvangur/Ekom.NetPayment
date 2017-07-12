using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
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
        public static Dictionary<string, string> BasePPProperties
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

        UmbracoHelper _umbracoHelper;
        Settings _settings;

        public UmbracoService(UmbracoHelper umbracoHelper, Settings settings)
        {
            _umbracoHelper = umbracoHelper;
            _settings = settings;
        }

        public virtual IPublishedContent GetPPNode(string ppDocTypeAlias)
        {
            var ppContainer = _umbracoHelper.TypedContent(_settings.PPUmbracoNode);
            return ppContainer.Children.First(x => x.DocumentTypeAlias == ppDocTypeAlias);
        }

        public virtual Dictionary<string, string> GetPPProperties(IPublishedContent node, string culture, Dictionary<string, string> properties = null)
        {
            var pp = _umbracoHelper.TypedContent(node);

            properties = properties ?? BasePPProperties;

            foreach (var prop in properties)
            {
                if (pp.HasVortoValue(prop.Key))
                {
                    properties[prop.Key] = pp.GetVortoValue<string>(prop.Key, culture);
                }
                else
                {
                    properties[prop.Key] = pp.GetPropertyValue<string>(prop.Key);
                }
            }

            return properties;
        }
    }
}
