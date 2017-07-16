using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Utility functions for handling <see cref="OrderStatus"/> objects
    /// </summary>
    public class OrderService
    {
        ApplicationContext _appCtx;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="appCtx"></param>
        public OrderService(ApplicationContext appCtx)
        {
            _appCtx = appCtx;
        }

        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        public static OrderStatus Get()
        {
            var request = HttpContext.Current.Request;

            string reference = request.QueryString["ReferenceNumber"];

            if (string.IsNullOrEmpty(reference))
            {
                reference = request.QueryString["orderId"];
            }

            if (string.IsNullOrEmpty(reference))
            {
                reference = request["reference"];
            }

            if (string.IsNullOrEmpty(reference))
            {
                reference = request["orderid"];
            }

            if (!string.IsNullOrEmpty(reference))
            {
                bool _referenceId = Guid.TryParse(reference, out var referenceId);

                if (_referenceId)
                {
                    using (var db = ApplicationContext.Current.DatabaseContext.Database)
                    {
                        return db.Single<OrderStatus>(referenceId);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get order with the given unique id
        /// </summary>
        /// <param name="id">Order id</param>
        public OrderStatus Get(Guid id)
        {
            using (var db = _appCtx.DatabaseContext.Database)
            {
                return db.Single<OrderStatus>(id);
            }
        }

        /// <summary>
        /// Persist in database and retrieve unique order id
        /// </summary>
        /// <returns>Order Id</returns>
        public virtual Guid Save(
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

            using (var db = _appCtx.DatabaseContext.Database)
            {
                // Return order id
                db.Insert(new OrderStatus
                {
                    Id = orderid,
                    Name = name.ToString(),
                    Member = member,
                    Amount = decimal.Parse(total, nfi),
                    Date = DateTime.Now,
                    PaymentProvider = paymentProvider,
                    Custom = custom
                });
            }

            return orderid;
        }
    }
}
