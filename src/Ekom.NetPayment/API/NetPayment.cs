using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.NetPayment.Exceptions;
using Umbraco.NetPayment.Helpers;
using Umbraco.NetPayment.Interfaces;
using Umbraco.Web;
using UmbracoCurrent = Umbraco.Core.Composing.Current;

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
                return _current ?? (_current = UmbracoCurrent.Factory.CreateInstance<NetPayment>());
            }
        }

        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly UmbracoService _uService;

        /// <summary>
        /// 
        /// </summary>
        public NetPayment(ILogger logger, Settings settings, UmbracoService uService)
        {
            _logger = logger;
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
                var or = UmbracoCurrent.Factory.CreateInstance(orType) as IOrderRetriever;
                var order = or.Get(request, ppNameOverride);

                if (order != null) return order;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a payment provider by name
        /// </summary>
        /// <param name="ppName">Payment provider alias or name. 
        /// Must have a matching umbraco pp node or basePaymentProvider property</param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(string ppName)
        {
            var ppNode = _uService.GetPPNode(ppName);
            return GetPaymentProvider(ppNode);
        }

        /// <summary>
        /// Retrieve a payment provider by key
        /// </summary>
        /// <param name="ppKey">Payment provider unique id.</param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(Guid ppKey)
        {
            var ppNode = _uService.GetPPNode(ppKey);
            return GetPaymentProvider(ppNode);
        }

        private IPaymentProvider GetPaymentProvider(IPublishedContent ppNode)
        {
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
        /// Fill dictionary with values for each dictionary key.
        /// </summary>
        /// <param name="ppName">Payment provider alias or name. Must have a matching umbraco pp node or basePaymentProvider property</param>
        /// <param name="culture">IS/EN f.x.</param>
        /// <param name="properties">Optional dictionary of keys to get, default uses base properties dictionary</param>
        public Dictionary<string, string> GetPPProperties(string ppName, string culture, Dictionary<string, string> properties = null)
        {
            var ppNode = _uService.GetPPNode(ppName);
            return GetPPProperties(ppNode, culture, properties);
        }
        /// <summary>
        /// Fill dictionary with values for each dictionary key.
        /// </summary>
        /// <param name="ppKey">Payment provider unique id.</param>
        /// <param name="culture">IS/EN f.x.</param>
        /// <param name="properties">Optional dictionary of keys to get, default uses base properties dictionary</param>
        public Dictionary<string, string> GetPPProperties(Guid ppKey, string culture, Dictionary<string, string> properties = null)
        {
            var ppNode = _uService.GetPPNode(ppKey);
            return GetPPProperties(ppNode, culture, properties);
        }

        private Dictionary<string, string> GetPPProperties(IPublishedContent pp, string culture, Dictionary<string, string> properties)
        {
            return _uService.GetPPProperties(pp, culture, properties);
        }

        internal static List<Type> orderRetrievers = new List<Type>();
        internal static Dictionary<string, Type> paymentProviders = new Dictionary<string, Type>();
    }
}
