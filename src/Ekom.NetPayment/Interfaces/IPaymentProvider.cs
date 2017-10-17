using System.Collections.Generic;
using System.Threading.Tasks;

namespace Umbraco.NetPayment.Interfaces
{
    /// <summary>
    /// F.x. Valitor/Borgun, handles submission of payment request 
    /// and returns html string to initiate redirect to portal.
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// Initiate a payment request .
        /// </summary>
        /// <param name="total">Total amount due</param>
        /// <param name="orders">Order lines, displayed as a list during payment</param>
        /// <param name="skipReceipt">Skip Valitor receipt page after payment</param>
        /// <param name="culture">IS/EN f.x.</param>
        /// <param name="member">Can be used to store umbraco member id</param>
        /// <param name="orderCustomString">Perfect for storing custom data/json :)</param>
        string Request(
            decimal total,
            IEnumerable<OrderItem> orders,
            bool skipReceipt,
            string culture,
            int member = 0,
            string orderCustomString = ""
        );

        /// <summary>
        /// Initiate a payment request .
        /// When calling RequestAsync, always await the result.
        /// </summary>
        /// <param name="total">Total amount due</param>
        /// <param name="orders">Order lines, displayed as a list during payment</param>
        /// <param name="skipReceipt">Skip Valitor receipt page after payment</param>
        /// <param name="culture">IS/EN f.x.</param>
        /// <param name="member">Can be used to store umbraco member id</param>
        /// <param name="orderCustomString">Perfect for storing custom data/json :)</param>
        Task<string> RequestAsync(
            decimal total,
            IEnumerable<OrderItem> orders,
            bool skipReceipt,
            string culture,
            int member = 0,
            string orderCustomString = ""
        );
    }
}
