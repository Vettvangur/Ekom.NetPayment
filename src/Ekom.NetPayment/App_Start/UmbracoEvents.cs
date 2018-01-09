using log4net;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.NetPayment.Helpers;
using Umbraco.NetPayment.Interfaces;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    class UmbEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            try
            {
                var container = Settings.container;

                var settings = container.GetInstance<Settings>();
                var xmlConfigService = container.GetInstance<IXMLConfigurationService>();

                // PaymentProviders.config
                var doc = xmlConfigService.Configuration;
                xmlConfigService.SetConfiguration(doc);

                var dbCtx = applicationContext.DatabaseContext;

                var dbHelper = new DatabaseSchemaHelper(dbCtx.Database, applicationContext.ProfilingLogger.Logger, dbCtx.SqlSyntax);

                //Check if the DB table does NOT exist
                if (!dbHelper.TableExist("customNetPaymentOrder"))
                {
                    //Create DB table - and set overwrite to false
                    dbHelper.CreateTable<OrderStatus>(false);
                }
                //Check if the DB table does NOT exist
                if (!dbHelper.TableExist("customNetPayments"))
                {
                    //Create DB table - and set overwrite to false
                    dbHelper.CreateTable<PaymentData>(false);
                }

                RegisterPaymentProviders();
                RegisterOrderRetrievers();
            }
            catch (Exception ex)
            {
                Log.Error("Fatal NetPayment error, aborting", ex);
            }
        }

        /// <summary>
        /// Find and register all <see cref="IPaymentProvider"/> with reflection.
        /// </summary>
        private void RegisterPaymentProviders()
        {
            var ppType = typeof(IPaymentProvider);
            var paymentProviders = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType));

            foreach (var pp in paymentProviders)
            {
                // Get value of "_ppNodeName" constant
                var fi = pp.GetField("_ppNodeName", BindingFlags.Static | BindingFlags.NonPublic);

                if (fi != null)
                {
                    var dta = (string)fi.GetRawConstantValue();
                    API.NetPayment._paymentProviders[dta.ToLower()] = pp;
                }
            }
        }

        /// <summary>
        /// Find and register all <see cref="IOrderRetriever"/> with reflection.
        /// </summary>
        private void RegisterOrderRetrievers()
        {
            var ppType = typeof(IOrderRetriever);
            var orderRetrievers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType));

            foreach (var or in orderRetrievers)
            {
                API.NetPayment._orderRetrievers.Add(or);
            }
        }

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
            );
    }
}
