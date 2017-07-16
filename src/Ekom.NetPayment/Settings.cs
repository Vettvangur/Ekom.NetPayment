using System;
using System.Configuration;
using System.Web;
using Umbraco.NetPayment.Helpers;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Various settings for the Umbraco.NetPayment package.
    /// Most settings can be overriden in AppSettings.
    /// </summary>
    public class Settings
    {
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

        private string _basePath;
        /// <summary> 
        /// Public server URL, used as basepath when requesting callbacks from remote PP's 
        /// </summary> 
        public virtual string BasePath
        {
            get
            {
                if (_basePath == null)
                {
                    var url = HttpContext.Current.Request.Url;
                    _basePath = $"{url.Scheme}://{url.Authority}";
                }

                return _basePath;
            }
            set { _basePath = value; }
        }

        private bool? _sendEmailAlerts;
        /// <summary>
        /// This property controls whether we attempt to send emails when exceptions occur in certain places.
        /// Used in response controllers.
        /// Defaults to true.
        /// </summary>
        public bool SendEmailAlerts
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
    }
}
