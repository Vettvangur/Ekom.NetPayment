﻿using log4net;
using Our.Umbraco.Vorto.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core.Persistence;
using Umbraco.NetPayment.Helpers;
using Umbraco.Web;

namespace Umbraco.NetPayment.Borgun
{
    /// <summary>
    /// Initiate a payment request with Borgun
    /// </summary>
    static partial class Payment
    {
        /// <summary>
        /// Initiate a payment request with Borgun
        /// </summary>
        public static string Request(int uPaymentProviderNodeId, string total, IEnumerable<OrderItem> orders, bool skipReceipt, string culture, int member = 0, string orderCustomString = "")
        {
            try
            {
                Log.Info("Borgun Payment Request - Start");

                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" +
                 HttpContext.Current.Request.Url.Authority;

                // Umbraco.NetPayment ResponseController
                var reportUrl  = baseUrl + "/umbraco/surface/borgunresponse/post";

                var successUrl = string.Empty;
                var errorUrl   = string.Empty;

                var portalUrl        = string.Empty;
                var merchantId       = string.Empty;
                var paymentGatewayId = string.Empty;
                var secretCode       = string.Empty;

                // Retrieve properties from umbraco payment provider node
                var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

                var paymentProvider = umbracoHelper.TypedContent(uPaymentProviderNodeId);

                try
                {
                    if (!string.IsNullOrEmpty(culture))
                    {
                        successUrl = paymentProvider.GetVortoValue<string>("successUrl", culture);
                        errorUrl = paymentProvider.GetVortoValue<string>("errorUrl", culture);
                    }
                    else
                    {
                        successUrl   = paymentProvider.GetPropertyValue<string>("successUrl");
                        errorUrl     = paymentProvider.GetPropertyValue<string>("errorUrl");
                    }

                    portalUrl        = paymentProvider.GetPropertyValue<string>("portalUrl");
                    merchantId       = paymentProvider.GetPropertyValue<string>("merchantId");
                    paymentGatewayId = paymentProvider.GetPropertyValue<string>("paymentGatewayId");
                    secretCode       = paymentProvider.GetPropertyValue<string>("secretCode");
                }
                catch (Exception ex)
                {
                    Log.Error("Error retrieving Umbraco properties", ex);
                    throw;
                }

                if (string.IsNullOrEmpty(culture)) culture = "IS";

                // Begin populating form values to be submitted
                var formValues = new Dictionary<string, string>
                {
                    { "merchantid", merchantId },
                    { "paymentgatewayid", paymentGatewayId },

                    { "returnurlsuccess", baseUrl + successUrl },
                    { "returnurlcancel", baseUrl + errorUrl },
                    { "returnurlerror", baseUrl + errorUrl },
                    { "returnurlsuccessserver", reportUrl },


                    { "amount", total },
                    { "currency", "ISK" },
                    { "language", culture.ToUpper() }
                };

                for (int lineNumber = 0, length = orders.Count(); lineNumber < length; lineNumber++)
                {
                    var order = orders.ElementAt(lineNumber);

                    formValues.Add("itemdescription_" + lineNumber, order.Title);
                    formValues.Add("itemcount_" + lineNumber, order.Quantity.ToString());
                    formValues.Add("itemunitamount_" + lineNumber, order.Price.ToString());
                    formValues.Add("itemamount_" + lineNumber, order.GrandTotal.ToString());
                }

                // Persist in database and retrieve unique order id
                string orderId = OrderHelper.Save(member, total, paymentProvider.Name, orderCustomString, orders);

                //CheckHash
                var checkHash = CreateCheckHash(secretCode,
                    new CheckHashMessage(merchantId, baseUrl + successUrl, 
                                                     baseUrl + "/umbraco/surface/borgunresponse/post", 
                                         orderId, total, "ISK"));

                formValues.Add("checkhash", checkHash);
                formValues.Add("orderid", orderId);

                Log.Info("Borgun Payment Request - Amount: " + total + " OrderId: " + orderId);

                return CreateRequest(formValues, portalUrl, skipReceipt);
            }
            catch (Exception ex)
            {
                Log.Error("Borgun Payment Request - Payment Request Failed", ex);
                throw;
            }
        }

        /// <summary>
        /// CreateCheckHash - HMACSHA256
        /// </summary>
        static string CreateCheckHash(string secretcode, CheckHashMessage checkHashMessage)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretcode);

            HMACSHA256 hasher = new HMACSHA256(secretBytes);

            byte[] result = hasher.ComputeHash(Encoding.UTF8.GetBytes(checkHashMessage.message));

            string checkhash = BitConverter.ToString(result).Replace("-", "");

            return checkhash;
        }


        static string CreateRequest(Dictionary<string, string> request, string url, bool skipReceipt)
        {
            var context = HttpContext.Current;

            var html = "<form action=\"" + url + "\" method=\"post\" id=\"payform\">\r\n";

            foreach (var parameter in request)
            {
                html += "<input type=\"hidden\" name=\"" + parameter.Key + "\" value=\"" + parameter.Value + "\">\r\n";
            }

            if (skipReceipt) html += "<input type=\"hidden\" name=\"skipreceiptpage\" value=\"1\">\r\n";

            html += "<input type=\"submit\" value=\"Submitting\">\r\n";

            html += "</form>";

            html += "<script>(function(){ document.getElementById('payform').submit(); }())</script>";

            return html;
        }

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );
    }
}
