﻿using System;

namespace Ekom.NetPayment.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentProviderNotFoundException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public PaymentProviderNotFoundException(string message) : base(message) { }
    }
}
