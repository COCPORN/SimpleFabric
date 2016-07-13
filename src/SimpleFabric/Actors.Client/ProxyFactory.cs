using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    class ProxyFactory
    {
        public static IActorProxyImplementation CreateProxy<T>()
        {
            return new InMemoryActorProxy<T>();
        }
    }
}
