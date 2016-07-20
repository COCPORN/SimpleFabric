using SimpleFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public interface IActorProxyImplementation
    {
        ActorId ActorId { get; set; }
        string ApplicationName { get; set; }
        Task Initialize();
    }
}
