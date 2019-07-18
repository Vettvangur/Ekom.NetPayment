using System.Threading.Tasks;

namespace Umbraco.NetPayment.Interfaces
{
    public interface IMailService
    {
        string Body { get; set; }
        string Recipient { get; set; }
        string Sender { get; set; }
        string Subject { get; set; }

        Task SendAsync();
    }
}
