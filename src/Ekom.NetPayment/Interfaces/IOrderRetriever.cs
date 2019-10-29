using System.Web;

namespace Umbraco.NetPayment.Interfaces
{
    /// <summary>
    /// Handles retrival of order using values posted or returned in querystring by payment provider
    /// </summary>
    public interface IOrderRetriever
    {
        /// <summary>
        /// Attempts to retrieve an order using data from the <see cref="HttpRequestBase"/>
        /// </summary>
        /// <returns>Returns the referenced order</returns>
        /// <param name="request">The http request.</param>
        /// <param name="ppNameOverride">When storing your xml configuration under an unstandard name, specify pp name override.</param>
        /// <returns></returns>
        OrderStatus Get(HttpRequestBase request, string ppNameOverride);
    }
}
