using System;

namespace ChatClient.Services.MessageHandler
{
    public class EntityBootstrapEventArgs : EventArgs
    {
        public Type EntityType { get; }

        public EntityBootstrapEventArgs(Type entityType)
        {
            EntityType = entityType;
        }
    }
}