using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Umbraco.NetPayment.Borgun
{
    /// <summary>
    /// Receives a callback from Borgun when customer completes payment.
    /// Changes order status and optionally runs a custom callback provided by the application consuming this library.
    /// </summary>
    public class BorgunResponseController : SurfaceController
    {
        /// <summary>
        /// Receives a callback from Borgun when customer completes payment.
        /// Changes order status and optionally runs a custom callback provided by the application consuming this library.
        /// </summary>
        public void Post(FormCollection form)
        {
            try
            {
                Log.Info("Borgun Payment Response Hit");

                // Posted data

                string orderidStr        = form["orderid"];
                string orderhash         = form["orderhash"];
                string authorizationcode = form["authorizationcode"];

                int orderid = int.Parse(orderidStr);

                Log.Info("Borgun Payment Response Hit - OrderId: " + orderid);

                // Retrieve order

                Order order;

                using (var db = new Database("umbracoDbDSN"))
                {
                    order = db.Single<Order>(orderid);
                }

                // MSSQL adds a minimum of 4 decimal places to decimal sql values
                // Some providers support a maximum of 2
                NumberFormatInfo nfi = new CultureInfo("is-IS", false).NumberFormat;

                string orderAmount = order.amount.ToString("#.00", nfi);

                // Retrieve payment provider

                IPublishedContent paymentProvider;

                try
                {
                    var paymentProviders = Umbraco.GetDictionaryValue("PaymentProviders");
                    paymentProvider = Umbraco.TypedContent(paymentProviders).Children.
                                            Where(x => string.Compare(x.Name, "Borgun", true) == 0).First();
                }
                catch (Exception ex)
                {
                    Log.Error("Error retrieving payment provider", ex);
                    return;
                }

                string secretCode = paymentProvider.GetProperty("secretCode").Value.ToString();

                string orderhashcheck = GetHMACSum(secretCode, 
                    new CheckHashMessage(orderid.ToString(), orderAmount, "ISK"));

                Log.Info("Borgun Payment Response Hit - Checking Validation with:\r\n" +
                            "secretCode: " + secretCode + "\r\n" +
                            "orderid: "    + orderid    + "\r\n" + 
                            "amount: "     + orderAmount);

                if (string.Compare(orderhash, orderhashcheck, true) == 0)
                {
                    Log.Info("Borgun Payment Response Hit - Validation Success - OrderHash");

                    order.paid = true;

                    using (var db = new Database("umbracoDbDSN"))
                    {
                        db.Update(order);

                        NetPayment.Payment.callback?.Invoke(order);
                    }

                    Log.Info("Borgun Payment Response - SUCCESS");
                }
                else
                {
                    Log.Error("Borgun Payment Response - Failed verification for hashes:\r\n" +
                        orderhash + "\r\n" +
                        orderhashcheck);
                }
            }
            catch(FormatException ex)
            {
                Log.Error("Error parsing orderid", ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Error("Borgun Payment Response Error", ex);
                throw;
            }
        }

        static string GetHMACSum(string secretcode, CheckHashMessage checkHashMessage)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretcode);

            var hasher = new HMACSHA256(secretBytes);

            byte[] result = hasher.ComputeHash(Encoding.UTF8.GetBytes(checkHashMessage.message));

            string checkhash = BitConverter.ToString(result).Replace("-", "");

            return checkhash;
        }

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );
    }
}
