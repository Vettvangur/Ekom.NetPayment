using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    public class OrderItem
    {
        public decimal GrandTotal { get; set; }

        public decimal Price { get; set; }

        public int Discount { get; set; }

        public string Title { get; set; }

        public int Quantity { get; set; }
    }
}
