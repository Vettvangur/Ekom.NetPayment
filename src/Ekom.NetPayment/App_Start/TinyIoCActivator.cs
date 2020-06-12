using CommonServiceLocator.TinyIoCAdapter;
using TinyIoC;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Ekom.NetPayment.App_Start.TinyIoCActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Ekom.NetPayment.App_Start.TinyIoCActivator), "Shutdown")]

namespace Ekom.NetPayment.App_Start
{
    /// <summary>Provides the bootstrapping for integrating TinyIoC when it is hosted in ASP.NET</summary> 
    static class TinyIoCActivator
    {
        /// <summary>Integrates TinyIoC when the application starts.</summary> 
        public static TinyIoCContainer Start()
        {
            // Register Types 
            var container = TinyIoCContainer.Current;
            TinyIoCConfig.RegisterTypes(container);
            Settings.container = new TinyIoCServiceLocator(container);

            return container;
        }

        /// <summary>Disposes the container when the application is shut down.</summary> 
        public static void Shutdown()
        {
            var container = TinyIoCContainer.Current;
            container.Dispose();
        }
    }
}
