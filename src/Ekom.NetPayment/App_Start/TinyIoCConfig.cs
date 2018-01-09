using Examine;
using System.IO.Abstractions;
using System.Web;
using TinyIoC;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;

namespace Umbraco.NetPayment
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
            container.Register<HttpContextBase>((c, p) => new HttpContextWrapper(HttpContext.Current)).AsPerRequestSingleton();
            container.Register<HttpRequestBase>((c, p) => c.Resolve<HttpContextBase>().Request).AsPerRequestSingleton();
            container.Register<HttpResponseBase>((c, p) => c.Resolve<HttpContextBase>().Response).AsPerRequestSingleton();
            container.Register<HttpServerUtilityBase>((c, p) => c.Resolve<HttpContextBase>().Server).AsPerRequestSingleton();

            container.Register<Settings>().AsSingleton();
            container.Register<ApplicationContext>((c, p) => ApplicationContext.Current).AsSingleton();
            container.Register<ExamineManagerBase>((c, p) => new ExamineManagerWrapper(ExamineManager.Instance)).AsSingleton();
            container.Register<UmbracoConfig>((c, p) => UmbracoConfig.For).AsSingleton();

            container.Register<UmbracoContext>((c, p) => UmbracoContext.Current).AsPerRequestSingleton();
            container.Register<UmbracoHelper>((c, p) => new UmbracoHelper(c.Resolve<UmbracoContext>())).AsPerRequestSingleton();

            container.Register<IFileSystem, FileSystem>().AsMultiInstance();
            container.Register<IDatabaseFactory, DatabaseFactory>().AsMultiInstance();
            container.Register<ILogFactory, LogFactory>().AsMultiInstance();
            container.Register<IOrderService, OrderService>().AsMultiInstance();
            container.Register<IXMLConfigurationService, XMLConfigurationService>().AsMultiInstance();
        }
    }
}
