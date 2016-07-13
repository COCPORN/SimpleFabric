using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    //
    // Summary:
    //     An Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext contains information
    //     about the method that is invoked by actor runtime and is passed as an argument
    //     to Microsoft.ServiceFabric.Actors.Runtime.ActorBase.OnPreActorMethodAsync(Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext)
    //     and Microsoft.ServiceFabric.Actors.Runtime.ActorBase.OnPostActorMethodAsync(Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext).
    public struct ActorMethodContext
    {
        //
        // Summary:
        //     Type of call by actor runtime (e.g. actor interface method, timer callback etc.).
        public ActorCallType CallType { get; }
        //
        // Summary:
        //     Name of the method invoked by actor runtime.
        public string MethodName { get; }
    }
}
