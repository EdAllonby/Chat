namespace SharedClasses
{
    /// <summary>
    /// Holds references to services.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        /// Gets a service from its type.
        /// </summary>
        /// <typeparam name="T">The type of service to return.</typeparam>
        /// <returns>The service requested.</returns>
        T GetService<T>() where T : IService;

        /// <summary>
        /// Adds a service to the registry.
        /// </summary>
        /// <typeparam name="T">The type of service to add.</typeparam>
        /// <param name="service">The service instance to add.</param>
        void RegisterService<T>(IService service) where T : IService;
    }
}