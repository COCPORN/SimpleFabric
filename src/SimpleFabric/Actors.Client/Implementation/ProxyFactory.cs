using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    class ProxyFactory
    {
        public static ActorProxyType ActorProxyType { get; set; } 
                                = ActorProxyType.InMemoryActorProxy;

        public static IActorProxyImplementation CreateProxy<T>()
        {
            switch (ActorProxyType)
            {
                case ActorProxyType.InMemoryActorProxy:
                    return new InMemoryActorProxy<T>();
                default:
                    throw new InvalidOperationException("Unknown actor proxy type");
            }
            
        }
    }
}
