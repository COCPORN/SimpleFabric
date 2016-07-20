using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public class InMemoryActorProxyBase : DynamicObject
    {
        public static int DefaultActorLifetime = 30 * 60 * 1000;
        public static int ActorLifetime { get; set; } = 30 * 60 * 1000; // Default to 30 minutes
    }
}
