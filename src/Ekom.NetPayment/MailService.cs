using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using Umbraco.Core.Configuration;

namespace Umbraco.NetPayment
{
    public class MailService
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

        public MailService(UmbracoConfig uConfig)
        {
            _uConfig = uConfig;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string username = smtpSection.Network.UserName;
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

        public virtual void Send()
        {
            using (var smtp = new SmtpClient(_host, _port))
            {
                // We do not catch the error here... let it pass direct to the caller
                using (var message = new MailMessage(Sender, Recipient, Subject, Body) { IsBodyHtml = true })
                {
                    if (_user.Length > 0 && _pass.Length > 0)
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_user, _pass);
                        smtp.EnableSsl = _ssl;
                    }

                    smtp.Send(message);
                }
            }
        }
    }
}