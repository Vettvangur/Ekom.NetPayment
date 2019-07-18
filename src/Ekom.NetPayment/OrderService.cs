using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.NetPayment.Helpers;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Utility functions for handling <see cref="OrderStatus"/> objects
    /// </summary>
    class OrderService : IOrderService
    {
        readonly Settings _settings;
        readonly IDatabaseFactory _dbFac;

        /// <summary>
        /// ctor
        /// </summary>
        public OrderService(
            Settings settings,
            IDatabaseFactory dbFac
        )
        {
            _settings = settings;
            _dbFac = dbFac;
        }

        /// <summary>
        /// Get order with the given unique id
        /// </summary>
        /// <param name="id">Order id</param>
        public Task<OrderStatus> GetAsync(Guid id)
        {
            using (var db = _dbFac.GetDb())
            {
                return db.SingleByIdAsync<OrderStatus>(id);
            }
        }

        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        public OrderStatus GetOrderFromEncryptedReference(string reference, string key)
        {
            var keyShaSum = CryptoHelpers.GetSHA256StringSum(key);
            var orderIdStr = AesCryptoHelper.Decrypt(keyShaSum, reference);

            var orderId = Guid.Parse(orderIdStr);

            return GetAsync(orderId).Result;
        }

        /// <summary>
        /// Persist in database and retrieve unique order id
        /// </summary>
        /// <returns>Order Id</returns>
        public async Task<Guid> InsertAsync(
            int member,
            decimal total,
            string paymentProvider,
            string custom,
            IEnumerable<OrderItem> orders,
            HttpRequestBase Request
        )
        {
            NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

            var sb = new StringBuilder();

            foreach (var order in orders)
            {
                sb.Append(order.Title + " ");
            }

            var orderName = sb.ToString().TrimEnd(' ');

            var orderid = Guid.NewGuid();

            using (var db = _dbFac.GetDb())
            {
                // Return order id
                await db.InsertAsync(new OrderStatus
                {
                    Id = orderid,
                    Name = orderName,
                    Member = member,
                    Amount = total,
                    Date = DateTime.Now,
                    IPAddress = Request?.UserHostAddress,
                    UserAgent = Request.UserAgent,
                    PaymentProvider = paymentProvider,
                    Custom = custom
                });
            }

            return orderid;
        }
    }
}
