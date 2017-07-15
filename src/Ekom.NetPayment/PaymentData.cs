using System;
using Umbraco.Core.Persistence;

namespace uWebshop.Payment.Valitor
{
    /// <summary>
    /// Saves payment information for transactions
    /// </summary>
    [TableName("customPayments")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class paymentData
    {
        /// <summary>
        /// SQL entry Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
