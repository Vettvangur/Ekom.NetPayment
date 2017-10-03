using System.Web;

namespace Umbraco.NetPayment.Interfaces
{
    /// <summary>
    /// Handles retrival of order using values posted or returned in querystring by payment provider
    /// </summary>
    public interface IOrderRetriever
    {
        OrderStatus Get(HttpRequestBase request);
    }
}
