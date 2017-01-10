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
        /// <summary>
        /// Allows for custom code to run after a response is received
        /// </summary>
        public static Action<Order> callback;

        /// <summary>
        /// This method is called to start a payment request, 
        /// it determines which payment provider to run and calls the relevant method
        /// </summary>
        /// <param name="uPaymentProviderNodeId">Umbraco node id of payment provider</param>
        /// <param name="member">The umbraco member making the request</param>
        /// <param name="total">Grand total of order in unspecified currency</param>
        /// <param name="orderItems">Collection of order items</param>
        public static string Request(int uPaymentProviderNodeId, int member, decimal total, IEnumerable<OrderItem> orderItems, string orderCustomString)
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

                    return Borgun.Payment.Request(uPaymentProviderNodeId, member, totalStr, orderItems, orderCustomString);

            }

            throw new Exception("Unable to match payment provider");
        }
    }
}
