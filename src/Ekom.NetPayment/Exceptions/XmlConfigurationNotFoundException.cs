using System;

namespace Ekom.NetPayment.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlConfigurationNotFoundException : Exception
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        public XmlConfigurationNotFoundException(string message) : base(message) { }
    }
}
