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
        OrderStatus Get(HttpRequestBase request);
    }
}
