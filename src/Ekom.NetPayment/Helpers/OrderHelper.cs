using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Core.Persistence;

namespace Umbraco.NetPayment.Helpers
{
    public static class OrderHelper
    {
        /// <summary>
        /// Attempts to retrieve an order using data from the querystring or posted values
        /// </summary>
        /// <returns>Returns the referenced order or null otherwise</returns>
        public static Order Get()
        {
            string reference = HttpContext.Current.Request.QueryString["referenceNumber"];

            if (string.IsNullOrEmpty(reference))
            {
                reference = HttpContext.Current.Request.QueryString["orderId"];
            }

            if (string.IsNullOrEmpty(reference))
            {
                reference = HttpContext.Current.Request["reference"];
            }

            if (string.IsNullOrEmpty(reference))
            {
                reference = HttpContext.Current.Request["orderid"];
            }

            if (!string.IsNullOrEmpty(reference))
            {
                int referenceId;

                bool _referenceId = int.TryParse(reference, out referenceId);

                if (_referenceId)
                {
                    using (var db = new Database("umbracoDbDSN"))
                    {
                        return db.Single<Order>(referenceId);
                    }
                }
            }

            return null;
        }
    }
}
