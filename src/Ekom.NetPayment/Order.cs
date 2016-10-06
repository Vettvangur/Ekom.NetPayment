using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace Umbraco.NetPayment
{
    [TableName("customOrder")]
    [PrimaryKey("id", autoIncrement = false)]
    public class Order
    {
        public Guid id { get; set; }

        public int member { get; set; }

        public decimal amount { get; set; }

        public DateTime date { get; set; }

        public bool paid { get; set; }

        public string paymentProvider { get; set; }
    }
}
