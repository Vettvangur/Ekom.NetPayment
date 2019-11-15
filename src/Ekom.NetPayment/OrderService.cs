using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;
using Umbraco.NetPayment.Helpers;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Utility functions for handling <see cref="OrderStatus"/> objects
    /// </summary>
    class OrderService : IOrderService
    {
        ApplicationContext _appCtx;
        Settings _settings;
        IDatabaseFactory _dbFac;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="appCtx"></param>
        /// <param name="settings"></param>
        /// <param name="dbFac"></param>
        public OrderService(
            ApplicationContext appCtx,
            Settings settings,
            IDatabaseFactory dbFac
        )
        {
            _appCtx = appCtx;
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
                return Task.FromResult(db.Single<OrderStatus>(id));
            }
        }

        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        [Obsolete("We no longer encrypt the reference")]
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
        public Task<Guid> InsertAsync(
            int member,
            decimal total,
            string paymentProvider,
            string custom,
            IEnumerable<OrderItem> orders,
            HttpRequestBase Request
        )
        {
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
                db.Insert(new OrderStatus
                {
                    Id = orderid,
                    Name = orderName,
                    Member = member,
                    Amount = total,
                    Date = DateTime.Now,
                    IPAddress = Request.UserHostAddress,
                    UserAgent = Request.UserAgent,
                    PaymentProvider = paymentProvider,
                    Custom = custom
                });
            }

            return Task.FromResult(orderid);
        }
    }
}
