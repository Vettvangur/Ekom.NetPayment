using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
