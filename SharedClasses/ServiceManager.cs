using System;
using System.Collections.Generic;

namespace SharedClasses
{
    /// <summary>
    /// Holds references to <see cref="IService"/>s that can be accessed anywhere in the system.
    /// </summary>
    public class ServiceManager
    {
        private static readonly ServiceManager Instance = new ServiceManager();
        private static readonly Dictionary<Type, IService> Services = new Dictionary<Type, IService>();

        private ServiceManager()
        {
        }

        /// <summary>
        /// Adds a service to the <see cref="ServiceManager"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IService"/> to add.</typeparam>
        /// <param name="service">The IService instance to add.</param>
        public static void RegisterService<T>(IService service)
        {
            Instance.Register<T>(service);
        }

        /// <summary>
        /// Gets the <see cref="IService"/> by type stored in the <see cref="ServiceManager"/>.
        /// </summary>
        /// <typeparam name="T">The instance type of <see cref="ISerivce"/> to get.</typeparam>
        /// <returns>The instance of <see cref="IService"/> stored in the <see cref="ServiceManager"/>.</returns>
        public static T GetService<T>() where T : IService
        {
            return Instance.Get<T>();
        }

        /// <summary>
        /// Removes the <see cref="IService"/> by type stored in the <see cref="ServiceManager"/>.
        /// </summary>
        /// <typeparam name="T">The instance type of <see cref="IService"/> to remove.</typeparam>
        public void RemoveService<T>() where T : IService
        {
            Instance.Remove<T>();
        }

        /// <summary>
        /// Removes all instances from the <see cref="ServiceManager"/>.
        /// </summary>
        public void ClearServices()
        {
            Instance.Clear();
        }

        private T Register<T>(IService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            lock (Services)
            {
                if (Services.ContainsKey(typeof (T)))
                {
                    throw new ArgumentException("Service already registered");
                }

                Services[typeof (T)] = service;
            }

            return (T) service;
        }

        private T Get<T>() where T : IService
        {
            lock (Services)
            {
                if (!Services.ContainsKey(typeof (T)))
                {
                    throw new ArgumentException("Service not registered: " + typeof (T));
                }

                return (T) Services[typeof (T)];
            }
        }

        private void Remove<T>() where T : IService
        {
            lock (Services)
            {
                if (Services.ContainsKey(typeof (T)))
                {
                    Services.Remove(typeof (T));
                }
            }
        }

        private void Clear()
        {
            lock (Services)
            {
                Services.Clear();
            }
        }
    }
}