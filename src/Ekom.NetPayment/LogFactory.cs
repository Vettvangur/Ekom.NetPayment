using log4net;
using System;

namespace Umbraco.NetPayment
{
	/// <summary>
	/// Creates an <see cref="ILog"/> instance for the provided <see cref="Type"/>
	/// </summary>
	class LogFactory : ILogFactory
	{
		public ILog GetLogger(Type T)
		{
			return LogManager.GetLogger(T);
		}
	}

	/// <summary>
	/// Creates an <see cref="ILog"/> instance for the provided <see cref="Type"/>
	/// </summary>
	interface ILogFactory
	{
		ILog GetLogger(Type T);
	}
}
