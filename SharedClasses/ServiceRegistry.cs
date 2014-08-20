using System;
using System.Collections.Generic;
using log4net;

namespace SharedClasses
{
    public sealed class ServiceRegistry : IServiceRegistry
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServiceRegistry));
        private readonly IDictionary<Type, IService> services = new Dictionary<Type, IService>();

        public T GetService<T>() where T : IService
        {
            if (!services.ContainsKey(typeof (T)))
            {
                throw new ArgumentException("Service not registered: " + typeof (T));
            }

            return (T) services[typeof (T)];
        }

        public void RegisterService<T>(IService service) where T : IService
        {
            services.Add(typeof (T), service);
        }
    }
}