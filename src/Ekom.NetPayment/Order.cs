using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Generalized object storing basic information on orders and their status
    /// </summary>
    [TableName("customNetPaymentOrder")]
    [PrimaryKey("id", autoIncrement = false)]
    public class Order
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Umbraco member id
        /// </summary>
        public int Member { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// String name of payment provider <see cref="IPublishedContent"/> node
        /// </summary>
        public string PaymentProvider { get; set; }

        /// <summary>
        /// Store custom order data here
        /// </summary>
        public string Custom { get; set; }

        /// <summary>
        /// Visa/Mastercard/etc..
        /// </summary>
        public string CardType { get; set; }
    }
}
