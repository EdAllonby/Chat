using System;
using System.Collections.Generic;
using log4net;

namespace SharedClasses
{
    /// <summary>
    /// Holds references to services.
    /// </summary>
    public sealed class ServiceRegistry : IServiceRegistry
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceRegistry));

        private readonly IDictionary<Type, IService> services = new Dictionary<Type, IService>();

        /// <summary>
        /// Gets a service from its type.
        /// </summary>
        /// <typeparam name="T">The type of service to return.</typeparam>
        /// <returns>The service requested.</returns>
        public T GetService<T>() where T : IService
        {
            if (!services.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Service not registered: " + typeof(T));
            }

            return (T) services[typeof(T)];
        }

        /// <summary>
        /// Adds a service to the registry.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="service">The service instance to add.</param>
        public void RegisterService<T>(IService service) where T : IService
        {
            services.Add(typeof(T), service);
            Log.DebugFormat($"Service [{typeof(T).FullName}] added to service registry");
        }
    }
}