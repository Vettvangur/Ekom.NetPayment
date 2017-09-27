using Microsoft.Practices.Unity;
using Umbraco.Core;
using Umbraco.Core.Persistence;

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

            var settings = container.Resolve<ISettings>();
            var xmlConfigService = container.Resolve<XMLConfigurationService>();

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
        }
    }
}
