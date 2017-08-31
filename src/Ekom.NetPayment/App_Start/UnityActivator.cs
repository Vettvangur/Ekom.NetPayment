[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Umbraco.NetPayment.App_Start.UnityActivator), "Shutdown")]

namespace Umbraco.NetPayment.App_Start
{
	/// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
	static class UnityActivator
	{
		/// <summary>Disposes the Unity container when the application is shut down.</summary>
		public static void Shutdown()
		{
			var container = UnityConfig.GetConfiguredContainer();
			container.Dispose();
		}
	}
}
