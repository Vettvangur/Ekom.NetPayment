using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Base class to initiate a Payment Request, offers configuration of custom callback as well.
    /// </summary>
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
        /// <param name="orderCustomString">Custom string to persist in database with order</param>
        /// <param name="culture">Language culture to use when retrieving data from Vorto properties</param>
        /// <param name="skipReceipt">Skip the receipt page of payment provider if possible</param>
        public static string Request(
            int uPaymentProviderNodeId, 
            int member, 
            decimal total, 
            IEnumerable<OrderItem> orderItems, 
            string orderCustomString, 
            string culture = "", 
            bool skipReceipt = true
        )
        {
            var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);

            var paymentProvider = umbracoHelper.TypedContent(uPaymentProviderNodeId);

            // Reformat decimal amount as some providers only accept two decimal places

            NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

            string totalStr = Math.Round(total, 2).ToString("#.00", nfi);

            if (string.IsNullOrEmpty(culture))
            {
                culture = umbracoHelper.CultureDictionary.Culture.TwoLetterISOLanguageName;
            }

            switch (paymentProvider.Name.ToLower())
            {
                case "borgun":

                    return Borgun.Payment.Request(uPaymentProviderNodeId, totalStr, orderItems, skipReceipt, culture, member, orderCustomString);
            }

            throw new Exception("Unable to match payment provider");
        }

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );
    }
}
