using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    public static class Payment
    {
        public static void Request(int uPaymentProviderNodeId, int member, int total, IEnumerable<OrderItem> orders)
        {
            var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);

            var paymentProvider = umbracoHelper.TypedContent(uPaymentProviderNodeId);

            switch (paymentProvider.Name)
            {
                case "borgun":
                case "Borgun":

                    Borgun.Payment.Request(uPaymentProviderNodeId, member, total, orders);

                    break;
            }
        }
    }
}
