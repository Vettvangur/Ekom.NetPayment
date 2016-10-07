using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    public static class Payment
    {
        public static void Request(int uPaymentProviderNodeId, int member, decimal total, IEnumerable<OrderItem> orders)
        {
            var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);

            var paymentProvider = umbracoHelper.TypedContent(uPaymentProviderNodeId);

            // Reformat decimal amount as some providers only accept two decimal places

            NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

            string totalStr = Math.Round(total, 2).ToString("#.00", nfi);

            switch (paymentProvider.Name)
            {
                case "borgun":
                case "Borgun":

                    Borgun.Payment.Request(uPaymentProviderNodeId, member, totalStr, orders);

                    break;
            }
        }
    }
}
