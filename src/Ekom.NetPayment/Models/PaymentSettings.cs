using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ekom.NetPayment
{
    /// <summary>
    /// <inheritDoc cref="IPaymentSettings"/>
    /// </summary>
    public class PaymentSettings : IPaymentSettings
    {
        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public Guid PaymentProviderKey { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public IEnumerable<OrderItem> Orders { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string Language { get; set; } = "IS";

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string VortoLanguage { get; set; } = "IS";

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool SkipReceipt { get; set; } = true;

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public int Member { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string OrderCustomString { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string ReportUrl { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string CancelUrl { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string ErrorUrl { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public int CheckoutTimeoutMinutes { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool RequireCustomerInformation { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string MerchantEmail { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public int LoanType { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string Expiry { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string CVV { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public Dictionary<string, object> CustomSettings { get; set; }
    }
}
