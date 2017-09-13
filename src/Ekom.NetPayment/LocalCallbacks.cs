using System;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Callbacks to run on success/error.
    /// Supplied by library consumer.
    /// </summary>
    public class LocalCallbacks
    {
        /// <summary>
        /// Callback to run on success
        /// </summary>
        /// <param name="o"></param>
        public delegate void successCallback(OrderStatus o);

        /// <summary>
        /// Callback to run on error
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ex"></param>
        public delegate void errorCallback(OrderStatus o, Exception ex);
    }
}
