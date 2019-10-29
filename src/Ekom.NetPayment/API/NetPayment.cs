using log4net;
using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.NetPayment.Exceptions;
using Umbraco.NetPayment.Helpers;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NetPayment"/> class.
        /// </summary>
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
        /// <param name="request">Http request</param>
        /// <param name="ppNameOverride">When storing your xml configuration under an unstandard name, specify pp name override.</param>
        /// <returns></returns>
        public OrderStatus GetOrder(HttpRequestBase request = null, string ppNameOverride = null)
        {
            request = request ?? new HttpRequestWrapper(HttpContext.Current.Request);

            foreach (var orType in orderRetrievers)
            {
                var or = Settings.container.GetInstance(orType) as IOrderRetriever;
                var order = or.Get(request, ppNameOverride);

                if (order != null) return order;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a payment provider by name
        /// </summary>
        /// <param name="ppName">Payment provider alias or name. Must have a matching umbraco pp node or basePaymentProvider property</param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(string ppName)
        {
            var ppNode = _uService.GetPPNode(ppName);

            var basePpName = PublishedPaymentProviderHelper.GetName(ppNode);

            if (paymentProviders.ContainsKey(basePpName))
            {
                var ppType = paymentProviders[basePpName];

                var pp = Activator.CreateInstance(ppType) as IPaymentProvider;

                return pp;
            }
            else
            {
                throw new PaymentProviderNotFoundException("Base Payment Provider not found. DLL possibly missing. Name: " + basePpName);
            }
        }

        /// <summary>
        /// Retrieve a payment provider by key
        /// </summary>
        /// <param name="ppKey">Payment provider unique id.</param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(Guid ppKey)
        {
            var ppNode = _uService.GetPPNode(ppKey);
            var basePpName = PublishedPaymentProviderHelper.GetName(ppNode);

            if (paymentProviders.ContainsKey(basePpName))
            {
                var ppType = paymentProviders[basePpName];

                var pp = Activator.CreateInstance(ppType) as IPaymentProvider;

                return pp;
            }
            else
            {
                throw new PaymentProviderNotFoundException("Base Payment Provider not found. DLL possibly missing. Name: " + basePpName);
            }
        }

        internal static List<Type> orderRetrievers = new List<Type>();
        internal static Dictionary<string, Type> paymentProviders = new Dictionary<string, Type>();
    }
}
