using System;

namespace Umbraco.NetPayment
{
    /// <summary>
    /// Callbacks to run for all payment providers on success/error.
    /// Local in this context is in contrast with callbacks to be performed after a remote provider response f.x.
    /// Supplied by library consumer.
    /// </summary>
    public class LocalCallbacks
    {
        /// <summary>
        /// Raises the success event on successful payment verification
        /// </summary>
        /// <param name="o"></param>
        internal static void OnSuccess(OrderStatus o)
        {
            Success?.Invoke(o);
        }

        /// <summary>
        /// Raises the success event on successful payment verification
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ex"></param>
        internal static void OnError(OrderStatus o, Exception ex)
        {
            Error?.Invoke(o, ex);
        }

        /// <summary>
        /// Event fired on successful payment verification
        /// </summary>
        public static event successCallback Success;
        /// <summary>
        /// Event fired on payment verification error
        /// </summary>
        public static event errorCallback Error;

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
