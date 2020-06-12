using Examine;
using System.IO.Abstractions;
using System.Web;
using TinyIoC;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Ekom.NetPayment.Interfaces;
using Umbraco.Web;

namespace Ekom.NetPayment
{
    /// <summary>
    /// Specifies the TinyIoC configuration for the main container.
    /// </summary>
    static class TinyIoCConfig
    {
        /// <summary>Registers the type mappings with the TinyIoC container.</summary>
        /// <param name="container">The TinyIoC container to configure.</param>
        public static void RegisterTypes(TinyIoCContainer container)
        {
            container.Register<HttpContextBase>((c, p) => new HttpContextWrapper(HttpContext.Current));
            container.Register<HttpRequestBase>((c, p) => c.Resolve<HttpContextBase>().Request);
            container.Register<HttpResponseBase>((c, p) => c.Resolve<HttpContextBase>().Response);
            container.Register<HttpServerUtilityBase>((c, p) => c.Resolve<HttpContextBase>().Server);

            container.Register<Settings>().AsSingleton();
            container.Register<ApplicationContext>((c, p) => ApplicationContext.Current);
            container.Register<ExamineManagerBase>((c, p) => new ExamineManagerWrapper(ExamineManager.Instance));
            container.Register<UmbracoConfig>((c, p) => UmbracoConfig.For);

            container.Register<UmbracoContext>((c, p) => UmbracoContext.Current);
            container.Register<UmbracoHelper>((c, p) => new UmbracoHelper(c.Resolve<UmbracoContext>()));

            container.Register<IFileSystem, FileSystem>().AsMultiInstance();
            container.Register<IDatabaseFactory, DatabaseFactory>().AsMultiInstance();
            container.Register<IOrderService, OrderService>().AsMultiInstance();
            container.Register<IXMLConfigurationService>((c, p) =>
                new XMLConfigurationService(
                    c.Resolve<HttpServerUtilityBase>(),
                    c.Resolve<ApplicationContext>(),
                    c.Resolve<Settings>(),
                    c.Resolve<IFileSystem>(),
                    c.Resolve<ILogFactory>(),
                    c.Resolve<ExamineManagerBase>()
                )
            );

            container.Register<ILogFactory, LogFactory>();
            container.Register<IMailService, MailService>();
        }
    }
}
