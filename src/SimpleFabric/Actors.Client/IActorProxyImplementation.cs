using SimpleFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    interface IActorProxyImplementation
    {
        ActorId ActorId { get; set; }
        void Initialize();
    }
}
