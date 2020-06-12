using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Ekom.NetPayment.Helpers
{
    static class PublishedPaymentProviderHelper
    {
        /// <summary>
        /// Gets the base payment provider name of the given published payment provider node.
        /// Checks for basePaymentProvider umbraco property, uses node Name otherwise.
        /// </summary>
        /// <param name="publishedContent">Published payment provider node</param>
        /// <returns></returns>
        public static string GetName(IPublishedContent publishedContent)
        {
            var ppProp = publishedContent.HasProperty("basePaymentProvider") 
                ? publishedContent.GetProperty("basePaymentProvider") 
                : null;

            var basePpName = ppProp?.HasValue() == true
                ? ppProp.Value<string>() 
                : publishedContent.Name;

            return basePpName.ToLower();
        }
    }
}
