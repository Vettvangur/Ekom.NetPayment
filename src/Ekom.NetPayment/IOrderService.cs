using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// This internal interface allows us to include <see cref="IOrderService"/> as a dependency
    /// without causing a circular IOC resolution to occur when getting the singleton.
    /// We implement some of these members explicitly to hide them from public consumers of the
    /// <see cref="OrderService"/> API.
    /// </summary>
    internal interface IOrderService
    {
        /// <summary>
        /// Get order with the given unique id
        /// </summary>
        /// <param name="id">Order id</param>
        Task<OrderStatus> GetAsync(Guid id);

        /// <summary>
        /// Persist in database and retrieve unique order id
        /// </summary>
        /// <returns>Order Id</returns>
        Task<Guid> InsertAsync(
            int member,
            string total,
            string paymentProvider,
            string custom,
            IEnumerable<OrderItem> orders
        );

        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        OrderStatus GetOrderFromEncryptedReference(string reference, string key);
    }
}
