using Umbraco.Core;
using log4net;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Practices.Unity;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Umbraco.Core.Persistence;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    public class UmbEvents : ApplicationEventHandler
    {
        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var container = UnityConfig.GetConfiguredContainer();

            var settings = container.Resolve<Settings>();
            var xmlConfigService = container.Resolve<XMLConfigurationService>();

            // PaymentProviders.config
            var doc = xmlConfigService.Configuration;
            xmlConfigService.SetConfiguration(doc);

            var dbCtx = applicationContext.DatabaseContext;
            var db = new DatabaseSchemaHelper(dbCtx.Database, applicationContext.ProfilingLogger.Logger, dbCtx.SqlSyntax);

            //Check if the DB table does NOT exist
            if (!db.TableExist("customNetPaymentOrder"))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<OrderStatus>(false);
            }
            //Check if the DB table does NOT exist
            if (!db.TableExist("customPayments"))
            {
                //Create DB table - and set overwrite to false
                db.CreateTable<PaymentData>(false);
            }
        }
    }
}
