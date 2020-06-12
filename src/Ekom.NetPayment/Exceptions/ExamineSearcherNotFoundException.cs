using System;

namespace Ekom.NetPayment.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalIndexNotFound : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public ExternalIndexNotFound(string message) : base(message) { }
    }
}
