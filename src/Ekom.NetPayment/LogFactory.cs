using log4net;
using System;

namespace Umbraco.NetPayment
{
    public class LogFactory : ILogFactory
    {
        public ILog GetLogger(Type T)
        {
            return LogManager.GetLogger(T);
        }
    }

    public interface ILogFactory
    {
        ILog GetLogger(Type T);
    }
}
