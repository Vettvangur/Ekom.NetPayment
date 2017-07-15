using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Saves payment information for transactions
    /// </summary>
    [TableName("customPayments")]
    [PrimaryKey("Id", autoIncrement = false)]
    public class PaymentData
    {
        /// <summary>
        /// SQL entry Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NullSetting(NullSetting = NullSettings.Null)]
        public string PaymentDate { get; set; }

        /// <summary>
        /// From remote payment provider
        /// </summary>
        public string AuthorizationNumber { get; set; }

        /// <summary>
        /// Masked credit card number
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Mastercard/Visa/etc...
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// SQL entry creation date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
