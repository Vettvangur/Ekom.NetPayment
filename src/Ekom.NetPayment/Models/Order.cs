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
    [PrimaryKey("id", autoIncrement = true)]
    public class Order
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Database ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Umbraco member id
        /// </summary>
        public int member { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool paid { get; set; }

        /// <summary>
        /// String name of payment provider <see cref="IPublishedContent"/> node
        /// </summary>
        public string paymentProvider { get; set; }

        /// <summary>
        /// Store custom order data here
        /// </summary>
        public string custom { get; set; }
    }
}
