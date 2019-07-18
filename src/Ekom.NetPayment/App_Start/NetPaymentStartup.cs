using System;
using System.Linq;
using System.Net;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.NetPayment.App_Start;
using Umbraco.NetPayment.Helpers;
using Umbraco.NetPayment.Interfaces;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Hooks into the umbraco application startup lifecycle 
    /// </summary>
    class Composer : IUserComposer
    {
        /// <summary>
        /// Umbraco lifecycle method
        /// </summary>
        public void Compose(Composition composition)
        {
            composition.Components()
                .Append<EnsureTableExists>()
                .Append<NetPaymentStartup>()
                ;
        }
    }

    class NetPaymentStartup : IComponent
    {
        readonly IFactory _factory;
        readonly ILogger _logger;

        public NetPaymentStartup(IFactory factory, ILogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public void Initialize()
        {
            try
            {
                _logger.Info<NetPaymentStartup>("Startup");

                var settings = _factory.GetInstance<Settings>();
                var xmlConfigService = _factory.GetInstance<IXMLConfigurationService>()
                    // Access internal method
                    as XMLConfigurationService;

                // PaymentProviders.config
                var doc = xmlConfigService.Configuration;
                xmlConfigService.SetConfiguration(doc);

                RegisterPaymentProviders();
                RegisterOrderRetrievers();

                // Disable SSL and older TLS versions
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                _logger.Debug<NetPaymentStartup>("Done");
            }
            catch (Exception ex)
            {
                _logger.Error<NetPaymentStartup>(ex, "Fatal NetPayment error, aborting");
            }

        }

        public void Terminate() { }

        /// <summary>
        /// Find and register all <see cref="IPaymentProvider"/> with reflection.
        /// </summary>
        private void RegisterPaymentProviders()
        {
            _logger.Debug<NetPaymentStartup>("Registering NetPayment Providers");

            var ppType = typeof(IPaymentProvider);
            var paymentProviders = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType));

            _logger.Debug<NetPaymentStartup>($"Found {paymentProviders.Count()} payment providers");

            foreach (var pp in paymentProviders)
            {
                // Get value of "_ppNodeName" constant
                var fi = pp.GetField("_ppNodeName", BindingFlags.Static | BindingFlags.NonPublic);

                if (fi != null)
                {
                    var dta = (string)fi.GetRawConstantValue();
                    API.NetPayment.paymentProviders[dta.ToLower()] = pp;
                }
            }

            _logger.Debug<NetPaymentStartup>($"Registering NetPayment Providers - Done");
        }

        /// <summary>
        /// Find and register all <see cref="IOrderRetriever"/> with reflection.
        /// </summary>
        private void RegisterOrderRetrievers()
        {
            _logger.Debug<NetPaymentStartup>($"Registering NetPayment Order Retrievers");

            var ppType = typeof(IOrderRetriever);
            var orderRetrievers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => TypeHelper.GetTypesWithInterface(x, ppType));

            _logger.Debug<NetPaymentStartup>($"Found {orderRetrievers.Count()} Order Retrievers");

            foreach (var or in orderRetrievers)
            {
                API.NetPayment.orderRetrievers.Add(or);
            }

            _logger.Debug<NetPaymentStartup>($"Registering NetPayment Order Retrievers - Done");
        }
    }
}
