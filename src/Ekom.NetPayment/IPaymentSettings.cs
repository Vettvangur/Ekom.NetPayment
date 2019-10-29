using System;
using System.Collections.Generic;

namespace Umbraco.NetPayment
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
        /// Controls payment portal language
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Vorto language for payment provider properties
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
        /// 255 char max length.
        /// </summary>
        string OrderCustomString { get; set; }

        /// <summary>
        /// Override umbraco configured success url.
        /// </summary>
        string SuccessUrl { get; set; }

        /// <summary>
        /// Receives notifications on successful payment.
        /// </summary>
        string ReportUrl { get; set; }

        /// <summary>
        /// Override umbraco configured cancel url.
        /// Supported by: Borgun, Valitor
        /// </summary>
        string CancelUrl { get; set; }

        /// <summary>
        /// Override umbraco configured error url.
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
        /// Provider specific configuration.
        /// Currently unused, prefer typed properties with support description.
        /// </summary>
        Dictionary<string, object> CustomSettings { get; set; }
    }
}
