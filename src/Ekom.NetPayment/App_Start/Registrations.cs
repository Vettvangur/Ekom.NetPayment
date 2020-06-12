using Examine;
using System.IO.Abstractions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Ekom.NetPayment.Interfaces;

namespace Ekom.NetPayment
{
    /// <summary>
    /// Specifies the DI configuration.
    /// </summary>
    class Registrations : IUserComposer
    {
        /// <summary>Registers the type mappings with the Umbraco IoC container.</summary>
        public void Compose(Composition composition)
        {
            composition.Logger.Debug<Registrations>("Registering NetPayment services");
            DoCompose(composition);
            composition.Logger.Debug<Registrations>("Done");
        }
        /// <summary>
        /// internal for Unit Tests
        /// </summary>
        /// <param name="composition"></param>
        internal void DoCompose(IRegister composition)
        {
            composition.Register(f => new Settings(), Lifetime.Singleton);

            composition.Register<IFileSystem, FileSystem>();
            composition.Register<IDatabaseFactory, DatabaseFactory>();
            composition.Register<IOrderService, OrderService>();
            // This registration allows us to keep the constructor internal for IXMLConfigurationService
            composition.Register<IXMLConfigurationService>((f) =>
                new XMLConfigurationService(
                    f.GetInstance<HttpContextBase>(),
                    f.GetInstance<AppCaches>(),
                    f.GetInstance<Settings>(),
                    f.GetInstance<IFileSystem>(),
                    f.GetInstance<ILogger>(),
                    f.GetInstance<IExamineManager>()
                )
            );
            composition.Register<UmbracoService>();
            composition.Register<IMailService, MailService>();
        }
    }
}
