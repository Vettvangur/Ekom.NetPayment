using Microsoft.Practices.Unity;
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
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Initialise DI container, required by extensions also hooking into ApplicationStarted
            UnityConfig.GetConfiguredContainer();
        }

        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var settings = container.Resolve<Settings>();
            var xmlConfigService = container.Resolve<IXMLConfigurationService>();

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
    }
}
