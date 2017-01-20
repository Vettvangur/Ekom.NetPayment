using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string name { get; set; }

        public int id { get; set; }

        public int member { get; set; }

        public decimal amount { get; set; }

        public DateTime date { get; set; }

        public bool paid { get; set; }

        public string paymentProvider { get; set; }

        /// <summary>
        /// Store custom order data here
        /// </summary>
        public string custom { get; set; }
    }
}
