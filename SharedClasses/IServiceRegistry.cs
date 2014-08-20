namespace SharedClasses
{
    public interface IServiceRegistry
    {
        T GetService<T>() where T : IService;
        void RegisterService<T>(IService service) where T : IService;
    }
}