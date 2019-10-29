using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using Umbraco.Core.Configuration;
using Umbraco.NetPayment.Interfaces;

namespace Umbraco.NetPayment
{
    class MailService : IMailService
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _ssl;

        private UmbracoConfig _uConfig;

        public string Sender { get; set; } = "no-reply@umbraco.netpayment";
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="uConfig">/config/umbracoSettings.config</param>
        public MailService(UmbracoConfig uConfig)
        {
            _uConfig = uConfig;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            Recipient = _uConfig.UmbracoSettings().Content.NotificationEmailAddress;

            //MailServer - Represents the SMTP Server
            _host = smtpSection.Network.Host;
            //Port- Represents the port number
            _port = smtpSection.Network.Port;
            //MailAuthUser and MailAuthPass - Used for Authentication for sending email
            _user = smtpSection.Network.UserName;
            _pass = smtpSection.Network.Password;
            _ssl = smtpSection.Network.EnableSsl;

            Sender = smtpSection.From;
        }

        public async virtual Task SendAsync()
        {
            // We do not catch the error here... let it pass direct to the caller
            using (var smtp = new SmtpClient(_host, _port))
            using (var message = new MailMessage(Sender, Recipient, Subject, Body) { IsBodyHtml = true })
            {
                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.EnableSsl = _ssl;
                }

                await smtp.SendMailAsync(message).ConfigureAwait(false);
            }
        }
    }
}
