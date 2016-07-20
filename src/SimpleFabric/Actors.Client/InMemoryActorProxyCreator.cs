using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFabric.Actors.Client.Implementation;

namespace SimpleFabric.Actors.Client
{
    public class InMemoryActorProxyCreator : IProxyCreator
    {
        public IActorProxyImplementation Create<T>()
        {
            return new InMemoryActorProxy<T>();
        }
    }
}
