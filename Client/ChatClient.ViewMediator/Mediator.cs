using System;
using System.Collections.Generic;

namespace ChatClient.ViewMediator
{
    public sealed class Mediator
    {
        private static readonly Mediator MediatorInstance = new Mediator();

        private readonly Dictionary<ViewName, List<Action<object>>> callbacks = new Dictionary<ViewName, List<Action<object>>>();

        private Mediator()
        {
        }

        public static Mediator Instance
        {
            get { return MediatorInstance; }
        }

        public void Register(ViewName viewName, Action<object> action)
        {
            if (!callbacks.ContainsKey(viewName))
            {
                callbacks[viewName] = new List<Action<object>>();
            }

            callbacks[viewName].Add(action);
        }

        public void Unregister(ViewName viewName, Action<object> action)
        {
            callbacks[viewName].Remove(action);

            if (callbacks[viewName].Count == 0)
            {
                callbacks.Remove(viewName);
            }
        }

        public void SendMessage(ViewName viewName, object message)
        {
            callbacks[viewName].ForEach(action => action(message));
        }
    }
}