using SimpleFabric.Actors.Client.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client
{
    public interface IProxyCreator
    {
        IActorProxyImplementation Create<T>(); 
    }
}
