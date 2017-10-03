using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Umbraco.NetPayment.Helpers;
using Umbraco.NetPayment.Interfaces;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Various settings for the Umbraco.NetPayment package.
    /// Most settings can be overriden in AppSettings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Current dependency resolver instance
        /// </summary>
        public static IDependencyResolver container;


        /// <summary>
        /// 
        /// </summary>
        public virtual string ConnectionStringName { get; }
            = ConfigurationManager.AppSettings["NetPayment.ConnectionStringName"]
            ?? "umbracoDbDsn";

        /// <summary>
        /// Path to payment provider configuration.
        /// That file stores payment provider specific information that often gets xml transformed.
        /// F.x. merchantId and secret
        /// </summary>
        public virtual string PPConfigPath { get; set; }
            = ConfigurationManager.AppSettings["NetPayment.PPConfigPath"]
            ?? "~/App_Plugins/NetPayment/config/PaymentProviders.config";

        /// <summary>
        /// Umbraco node id of payment providers container.
        /// </summary>
        public virtual Guid PPUmbracoNode { get; set; }

        /// <summary>
        /// Payment providers umbraco node configuration element name.
        /// xml element name of configuration element that holds umbraco payment providers container Guid
        /// </summary>
        public virtual string PPUNodeConfElName { get; } = "paymentProvidersNode";

        /// <summary>
        /// Payment providers document type alias.
        /// </summary>
        public virtual string PPDocumentTypeAlias { get; set; }
            = ConfigurationManager.AppSettings["NetPayment.PPDocumentTypeAlias"]
            ?? "paymentProviders";

        private bool? _sendEmailAlerts;
        /// <summary>
        /// This property controls whether we attempt to send emails when exceptions occur in certain places.
        /// Used in response controllers.
        /// Defaults to true.
        /// </summary>
        public virtual bool SendEmailAlerts
        {
            get
            {
                if (_sendEmailAlerts == null)
                {
                    var configVal = ConfigurationManager.AppSettings["NetPayment.SendEmailAlerts"];

                    if (!string.IsNullOrEmpty(configVal))
                    {
                        _sendEmailAlerts = configVal.ConvertToBool();
                    }
                    else
                    {
                        // Default
                        _sendEmailAlerts = true;
                    }
                }

                return _sendEmailAlerts.Value;
            }
            set
            {
                _sendEmailAlerts = value;
            }
        }

        /// <summary>
        /// Enables more detailed logging, serialise request bodies to log f.x.
        /// </summary>
        public virtual bool DetailedLogging => ConfigurationManager.AppSettings["NetPayment.DetailedLogging"].ConvertToBool();
    }
}
