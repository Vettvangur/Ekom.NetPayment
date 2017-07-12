using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web;

namespace Umbraco.NetPayment.Services
{
    /// <summary>
    /// Retrieves Umbraco data
    /// </summary>
    public class UmbracoService
    {
        /// <summary>
        ///            
        /// </summary>
        public static Dictionary<string, string> BasePPProperties
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "successUrl", "" },
                    { "errorUrl", "" },
                };
            }
        }

        UmbracoHelper _umbracoHelper;

        public UmbracoService(UmbracoHelper umbracoHelper)
        {
            _umbracoHelper = umbracoHelper;
        }

        public Dictionary<string, string> GetPPProperties(Dictionary<string, string> properties, string culture)
        {
            var pp = _umbracoHelper.TypedContent(ppNodeId);

            foreach (var prop in properties)
            {
                if (pp.HasVortoValue(prop.Key))
                {
                    properties[prop.Key] = pp.GetVortoValue<string>(prop.Key);
                }
                else
                {
                    properties[prop.Key] = pp.GetPropertyValue<string>(prop.Key);
                }
            }


        }
    }
}
