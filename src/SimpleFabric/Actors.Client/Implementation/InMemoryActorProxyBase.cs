using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public abstract class InMemoryActorProxyBase : DynamicObject, IActorProxyImplementation
    {
        public static int DefaultActorLifetime = 30 * 60 * 1000;
        public static int ActorLifetime { get; set; } = 30 * 60 * 1000; // Default to 30 minutes
        public ActorId ActorId { get; set; }
        public string ApplicationName { get; set; }        

        public abstract Task Initialize();

        Dictionary<Type, Action> eventHandlers = null; 

        Task IActorProxyImplementation.SubscribeAsync<T>(T eventHandler)
        {
            return Task.FromResult(true);
        }

    }
}
