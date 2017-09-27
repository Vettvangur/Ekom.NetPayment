using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.NetPayment.Helpers;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Utility functions for handling <see cref="OrderStatus"/> objects
    /// </summary>
    public class OrderService : IOrderService
    {
        private static OrderService _current;
        /// <summary>
        /// OrderService Singleton
        /// </summary>
        public static OrderService Current
        {
            get
            {
                return _current ?? (_current = UnityConfig.GetConfiguredContainer().Resolve<OrderService>());
            }
        }

        ApplicationContext _appCtx;
        ISettings _settings;
        IDatabaseFactory _dbFac;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="appCtx"></param>
        /// <param name="settings"></param>
        /// <param name="dbFac"></param>
        public OrderService(
            ApplicationContext appCtx,
            ISettings settings,
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
        public async Task<OrderStatus> GetAsync(Guid id)
        {
            using (var db = _dbFac.GetDb())
            {
                return await db.SingleByIdAsync<OrderStatus>(id).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        internal OrderStatus GetOrderFromEncryptedReference(string reference, string key)
        {
            var keyShaSum = CryptoHelpers.GetSHA256StringSum(key);
            var orderIdStr = AesCryptoHelper.Decrypt(keyShaSum, reference);

            var orderId = Guid.Parse(orderIdStr);

            return Current.GetAsync(orderId).Result;
        }

        OrderStatus IOrderService.GetOrderFromEncryptedReference(string reference, string key)
        {
            return GetOrderFromEncryptedReference(reference, key);
        }

        /// <summary>
        /// Persist in database and retrieve unique order id
        /// </summary>
        /// <returns>Order Id</returns>
        internal async Task<Guid> InsertAsync(
            int member,
            string total,
            string paymentProvider,
            string custom,
            IEnumerable<OrderItem> orders
        )
        {
            NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

            var name = new StringBuilder();

            foreach (var order in orders)
            {
                name.Append(order.Title + " ");
            }

            var orderid = Guid.NewGuid();

            using (var db = _dbFac.GetDb())
            {
                // Return order id
                await db.InsertAsync(new OrderStatus
                {
                    Id = orderid,
                    Name = name.ToString(),
                    Member = member,
                    Amount = decimal.Parse(total, nfi),
                    Date = DateTime.Now,
                    PaymentProvider = paymentProvider,
                    Custom = custom
                }).ConfigureAwait(false);
            }

            return orderid;
        }

        Task<Guid> IOrderService.InsertAsync(
            int member,
            string total,
            string paymentProvider,
            string custom,
            IEnumerable<OrderItem> orders
        )
        {
            return InsertAsync(member, total, paymentProvider, custom, orders);
        }
    }
}
