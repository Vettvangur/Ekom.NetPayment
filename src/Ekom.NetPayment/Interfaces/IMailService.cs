using System.Threading.Tasks;

namespace Ekom.NetPayment.Interfaces
{
    /// <summary>
    /// Handles creation and sending of emails, uses defaults from configuration when possible.
    /// Default assumes a notification email intended for the administrator.
    /// All defaults are overridable.
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// Email message body
        /// </summary>
        string Body { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Recipient { get; set; }
        /// <summary>
        /// Defaults to "no-reply@umbraco.netpayment"
        /// </summary>
        string Sender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string Subject { get; set; }

        /// <summary>
        /// Send email message
        /// </summary>
        Task SendAsync();
    }
}
