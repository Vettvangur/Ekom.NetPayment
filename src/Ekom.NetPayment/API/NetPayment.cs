﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Umbraco.NetPayment.Interfaces;

namespace Umbraco.NetPayment.API
{
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
                return _current ?? (_current = Settings.container.GetService<NetPayment>());
            }
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
                var or = Settings.container.GetService(orType) as IOrderRetriever;
                var order = or.Get(request);

                if (order != null) return order;
            }

            return null;
        }

        /// <summary>
        /// Retrieve a payment provider by name
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public IPaymentProvider GetPaymentProvider(string pp)
        {
            var ppType = _paymentProviders[pp];
            return Activator.CreateInstance(ppType) as IPaymentProvider;
        }

        internal static List<Type> _orderRetrievers = new List<Type>();
        internal static Dictionary<string, Type> _paymentProviders = new Dictionary<string, Type>();
    }
}
