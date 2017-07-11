using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
