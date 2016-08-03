using System;

namespace ChatClient.Services.MessageHandler
{
    public interface IBootstrapper
    {
        event EventHandler<EntityBootstrapEventArgs> EntityBootstrapCompleted;
    }
}
