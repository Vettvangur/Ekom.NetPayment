﻿using System;

namespace Umbraco.NetPayment.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NetPaymentException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public NetPaymentException(string message) : base(message) { }
    }
}
