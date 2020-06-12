using System;
using System.Collections.Generic;

namespace Ekom.NetPayment
{
    /// <summary>
    /// Base configuration for PaymentProviders. 
    /// </summary>
    public interface IPaymentSettings
    {
        /// <summary>
        /// Chooses a specific payment provider node.
        /// Useful when you have multiple umbraco nodes targetting the same base payment provider.
        /// F.x. Borgun EN and IS with varying currencies and xml configurations.
        /// </summary>
        Guid PaymentProviderKey { get; set; }

        /// <summary>
        /// Order lines, displayed as a list during payment
        /// </summary>
        IEnumerable<OrderItem> Orders { get; set; }

        /// <summary>
        /// Controls payment portal language.
        /// Default IS
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Vorto language for payment provider properties.
        /// Default IS
        /// </summary>
        string VortoLanguage { get; set; }

        /// <summary>
        /// Skip receipt page after payment
        /// Supported by: Valitor, Borgun
        /// </summary>
        bool SkipReceipt { get; set; }

        /// <summary>
        /// Optionally store umbraco member id in persisted order
        /// </summary>
        int Member { get; set; }

        /// <summary>
        /// Perfect for storing custom data/json in persisted order to be read on callback after payment.
        /// </summary>
        string OrderCustomString { get; set; }

        /// <summary>
        /// Override umbraco configured success url. Used as-is by NetPayment to forward user to f.x. receipt page.
        /// Commonly overrided in consumers checkout 
        /// to provide an url that also contains a queryString with the orderId to use on receipt page.
        /// </summary>
        string SuccessUrl { get; set; }

        /// <summary>
        /// Receives notifications on successful payment.
        /// Supported by: Borgun, BorgunGateway, Valitor, PayPal
        /// </summary>
        string ReportUrl { get; set; }

        /// <summary>
        /// Override umbraco configured cancel url.
        /// Supported by: Borgun, Valitor, BorgunLoans
        /// </summary>
        string CancelUrl { get; set; }

        /// <summary>
        /// Override umbraco configured error url.
        /// Supported by: Borgun, BorgunGateway
        /// </summary>
        string ErrorUrl { get; set; }

        /// <summary>
        /// Controls how long the user has to complete checkout on payment portal page.
        /// Must be configured in tandem with a timeoutRedirectURL property on umbraco payment provider.
        /// Supported by: Valitor
        /// </summary>
        int CheckoutTimeoutMinutes { get; set; }

        /// <summary>
        /// Supported by: PayPal
        /// Others must configure using xml configuration since they will not have access to the settings object in the response callback.
        /// This causes verification issues.
        /// </summary>
        Currency? Currency { get; set; }

        /// <summary>
        /// Required by Valitor loans
        /// </summary>
        string MerchantName { get; set; }

        /// <summary>
        /// Email address to send receipts for purchases to
        /// Supported by: Borgun
        /// </summary>
        string MerchantEmail { get; set; }

        /// <summary>
        /// Customer name, mobile number and home address.
        /// Supported by: Borgun
        /// Merchantemail parameter must be set since cardholder information is returned through email to merchant.
        /// </summary>
        bool RequireCustomerInformation { get; set; }

        /// <summary>
        /// Provide customer information to payment provider.
        /// Supported by: BorgunLoans
        /// </summary>
        CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// LoanType specifier, use provider specific LoanType enum for values.
        /// Supported by: BorgunLoans, Valitor
        /// </summary>
        int LoanType { get; set; }

        #region Borgun Gateway

        /// <summary>
        /// 16 digit payment card number
        /// Supported by: BorgunGateway
        /// </summary>
        string CardNumber { get; set; }

        /// <summary>
        /// Card expiration date
        /// Supported by: BorgunGateway
        /// </summary>
        string Expiry { get; set; }

        /// <summary>
        /// Card Verification Value. Triple digit number on the back of the card.
        /// Supported by: BorgunGateway
        /// </summary>
        string CVV { get; set; }

        #endregion


        /// <summary>
        /// Displayed in SiminnPay payment overview
        /// This value facilitates correlating a orderid from the merchants system (f.x. Ekom orderid) 
        /// with an order displayed in the payment authorizers system.
        /// 
        /// We cannot support this in f.x. Valitor since the reference is used by netPayment
        /// to find it's order and related data in callback.
        /// That is, the value sent to Valitor as the reference must always match 
        /// the netpayment orderid, therefore we cannot allow a custom reference to be provided.
        /// 
        /// Supported by: SiminnPay
        /// </summary>
        string ReferenceId { get; set; }

        /// <summary>
        /// Provider specific configuration.
        /// Currently unused, prefer typed properties with support description.
        /// </summary>
        Dictionary<string, object> CustomSettings { get; set; }
    }
}
