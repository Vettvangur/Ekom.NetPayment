﻿using log4net;
using System;

namespace Ekom.NetPayment
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
        public ILog GetLogger<T>()
        {
            return LogManager.GetLogger(typeof(T));
        }
    }

    /// <summary>
    /// Creates an <see cref="ILog"/> instance for the provided <see cref="Type"/>
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="T">Type of class this logger logs for</param>
        /// <returns></returns>
        ILog GetLogger(Type T);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ILog GetLogger<T>();
    }
}
