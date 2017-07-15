using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
