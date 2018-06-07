using log4net;
using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.NetPayment.Interfaces;
using Umbraco.Web;

namespace Umbraco.NetPayment.API
{
    /// <summary>
    /// The NetPayment API, access payment providers and get orders from request data.
    /// </summary>
    public class NetPayment
    {
        private static NetPayment _current;
        /// <summary>
        /// OrderService Singleton
        /// </summary>
        public static NetPayment Current
        {
            get
            {
                return _current ?? (_current = Settings.container.GetInstance<NetPayment>());
            }
        }

        private readonly ILog _log;
        private readonly Settings _settings;
        private readonly UmbracoService _uService;

        public NetPayment(ILogFactory logFac, Settings settings, UmbracoService uService)
        {
            _log = logFac.GetLogger<NetPayment>();
            _settings = settings;
            _uService = uService;
        }

        /// <summary>
        /// Attempt to retrieve order using reference from http request.
        /// Loops over all registered <see cref="IOrderRetriever"/> to attempt to find the order reference.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public OrderStatus GetOrder(HttpRequestBase request = null)
        {
            request = request ?? new HttpRequestWrapper(HttpContext.Current.Request);

            foreach (var orType in _orderRetrievers)
            {
                var or = Settings.container.GetInstance(orType) as IOrderRetriever;
                var order = or.Get(request);

                if (order != null) return order;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a payment provider by name
        /// </summary>
        /// <param name="pp">Payment provider alias or name. Must have a matching umbraco pp node</param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(string pp)
        {
            var ppNode = _uService.GetPPNode(pp);
            var ppProp = ppNode.GetProperty("basePaymentProvider");

            string basePpName = ppProp.HasValue ? ppProp.GetValue<string>() : pp;

            var ppType = _paymentProviders[basePpName.ToLower()];
            return Activator.CreateInstance(ppType) as IPaymentProvider;
        }

        internal static List<Type> _orderRetrievers = new List<Type>();
        internal static Dictionary<string, Type> _paymentProviders = new Dictionary<string, Type>();
    }
}
