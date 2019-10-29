using System;
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
        /// Initiate a payment request.
        /// </summary>
        /// <param name="paymentSettings">Configuration object for PaymentProviders</param>
        string Request(IPaymentSettings paymentSettings);

        /// <summary>
        /// Initiate a payment request.
        /// When calling RequestAsync, always await the result.
        /// </summary>
        /// <param name="paymentSettings">Configuration object for PaymentProviders</param>
        Task<string> RequestAsync(IPaymentSettings paymentSettings);
    }
}
