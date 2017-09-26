using Microsoft.Practices.Unity;
using System;
using System.IO.Abstractions;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterTypes(
                AllClasses.FromAssemblies(typeof(UnityConfig).Assembly),
                WithMappings.FromMatchingInterface,
                WithName.Default
            );

            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();
            container.RegisterType<HttpContextBase>(new InjectionFactory(c => new HttpContextWrapper(HttpContext.Current)));
            container.RegisterType<HttpRequestBase>(
                new InjectionFactory(c =>
                        container.Resolve<HttpContextBase>().Request
            ));
            container.RegisterType<HttpResponseBase>(
                new InjectionFactory(c =>
                        container.Resolve<HttpContextBase>().Response
            ));
            container.RegisterType<HttpServerUtilityBase>(
                new InjectionFactory(c =>
                        container.Resolve<HttpContextBase>().Server
            ));

            container.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
            container.RegisterType<ApplicationContext>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => ApplicationContext.Current));
            container.RegisterType<UmbracoConfig>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => UmbracoConfig.For));

            container.RegisterType<UmbracoContext>(new InjectionFactory(c => UmbracoContext.Current));
            container.RegisterType<UmbracoHelper>(new InjectionConstructor(typeof(UmbracoContext)));

            container.RegisterType<IFileSystem, FileSystem>();
            container.RegisterType<ILogFactory, LogFactory>();
            container.RegisterType<IOrderService>(new InjectionFactory(c => OrderService.Current));
        }
    }
}
