﻿using log4net;
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

namespace Umbraco.NetPayment.Borgun
{
    public static partial class Payment
    {
        public static void Request(int uPaymentProviderNodeId, int member, string total, IEnumerable<OrderItem> orders)
        {
            try
            {
                Log.Info("Borgun Payment Request - Start");

                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" +
                 HttpContext.Current.Request.Url.Authority;

                var reportUrl  = baseUrl + "/umbraco/surface/borgunresponse/post";
                var successUrl = string.Empty;
                var errorUrl   = string.Empty;

                var portalUrl        = string.Empty;
                var merchantId       = string.Empty;
                var paymentGatewayId = string.Empty;
                var secretCode       = string.Empty;

                // Retrieve properties from umbraco payment provider node
                var umbracoHelper = new Umbraco.Web.UmbracoHelper(Umbraco.Web.UmbracoContext.Current);

                var paymentProvider = umbracoHelper.TypedContent(uPaymentProviderNodeId);

                try
                {
                    successUrl = umbracoHelper.TypedContent(umbracoHelper.GetDictionaryValue("PaymentSuccess")).Url;
                    errorUrl   = umbracoHelper.TypedContent(umbracoHelper.GetDictionaryValue("PaymentError")).Url;

                    portalUrl        = paymentProvider.GetProperty("portalUrl").Value.ToString();
                    merchantId       = paymentProvider.GetProperty("merchantId").Value.ToString();
                    paymentGatewayId = paymentProvider.GetProperty("paymentGatewayId").Value.ToString();
                    secretCode       = paymentProvider.GetProperty("secretCode").Value.ToString();
                }
                catch (Exception ex)
                {
                    Log.Error("Error retrieving Umbraco properties", ex);
                    return;
                }

                // Begin populating form values to be submitted
                var formValues = new Dictionary<string, string>();

                formValues.Add("merchantid", merchantId);
                formValues.Add("paymentgatewayid", paymentGatewayId);

                formValues.Add("returnurlsuccess", baseUrl + successUrl);
                formValues.Add("returnurlcancel", baseUrl + errorUrl);
                formValues.Add("returnurlerror", baseUrl + errorUrl);
                formValues.Add("returnurlsuccessserver", reportUrl);


                formValues.Add("amount", total);
                formValues.Add("currency", "ISK");
                formValues.Add("language", "IS");


                for (int lineNumber = 0, length = orders.Count(); lineNumber < length; lineNumber++)
                {
                    var order = orders.ElementAt(lineNumber);

                    formValues.Add("itemdescription_" + lineNumber, order.Title);
                    formValues.Add("itemcount_" + lineNumber, order.Quantity.ToString());
                    formValues.Add("itemunitamount_" + lineNumber, order.Price.ToString());
                    formValues.Add("itemamount_" + lineNumber, order.GrandTotal.ToString());
                }

                // Persist in database and retrieve unique order id
                string orderId;

                NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

                using (var db = new Database("umbracoDbDSN"))
                {
                    orderId = db.Insert(new Order
                    {
                        member          = member,
                        amount          = decimal.Parse(total, nfi),
                        date            = DateTime.Now,
                        paymentProvider = paymentProvider.Name
                    }).ToString();
                }

                //CheckHash
                var checkHash = CreateCheckHash(secretCode,
                    new CheckHashMessage(merchantId, baseUrl + successUrl, 
                                                     baseUrl + "/umbraco/surface/borgunresponse/post", 
                                         orderId, total, "ISK"));

                formValues.Add("checkhash", checkHash);
                formValues.Add("orderid", orderId);

                Log.Info("Borgun Payment Request - Amount: " + total + " OrderId: " + orderId);

                CreateRequest(formValues, portalUrl);
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

        static void CreateRequest(Dictionary<string, string> request, string url)
        {
            var context = HttpContext.Current;

            var html = "<form action=\"" + url + "\" method=\"post\" id=\"payform\">\r\n";

            foreach (var parameter in request)
            {
                html += "<input type=\"hidden\" name=\"" + parameter.Key + "\" value=\"" + parameter.Value + "\">\r\n";
            }

            html += "<input type=\"submit\" value=\"Submitting\">\r\n";

            html += "</form>";

            html += "<script>(function(){ document.getElementById('payform').submit(); }())</script>";

            context.Response.Write(html);
            context.Response.End();
        }

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );
    }
}