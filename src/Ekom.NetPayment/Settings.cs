using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Various settings for the Umbraco.NetPayment package.
    /// Most settings can be overriden in AppSettings.
    /// </summary>
    public class Settings
    {
        private string _ppConfigPath = null;
        /// <summary>
        /// Path to payment provider configuration.
        /// That file stores payment provider specific information that often gets xml transformed.
        /// F.x. merchantId and secret
        /// </summary>
        public virtual string PPConfigPath
        {
            get
            {
                if (_ppConfigPath == null)
                {
                    var ppConfigPath = ConfigurationManager.AppSettings["NetPayment.PPConfigPath"];

                    _ppConfigPath = ppConfigPath ?? "~/App_Plugins/NetPayment/config/PaymentProviders.config";
                }

                return _ppConfigPath;
            }
        }

        private Guid _ppUNode;
        /// <summary>
        /// Umbraco node id of payment providers container.
        /// </summary>
        public virtual Guid PPUmbracoNode
        {
            get
            {
                return _ppUNode;
            }
            set
            {
                _ppUNode = value;
            }
        }

        /// <summary>
        /// Payment providers umbraco node configuration element name.
        /// xml element name of configuration element that holds umbraco payment providers container Guid
        /// </summary>
        public virtual string PPUNodeConfElName { get; } = "paymentProvidersNode";

        private string _ppDocType = null;

        /// <summary>
        /// Payment providers document type alias.
        /// </summary>
        public virtual string PPDocumentTypeAlias
        {
            get
            {
                if (_ppDocType == null)
                {
                    var ppDocTypeAlias = ConfigurationManager.AppSettings["NetPayment.PPDocumentTypeAlias"];

                    _ppDocType = ppDocTypeAlias ?? "paymentProviders";
                }

                return _ppDocType;
            }
        }

        public virtual string BasePath { get; set; }
    }
}
