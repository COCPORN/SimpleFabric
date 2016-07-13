using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    //
    // Summary:
    //     Represents the call-type associated with the method invoked by actor runtime.
    //
    // Remarks:
    //     This is provided as part of Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext
    //     which is passed as argument to Microsoft.ServiceFabric.Actors.Runtime.ActorBase.OnPreActorMethodAsync(Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext)
    //     and Microsoft.ServiceFabric.Actors.Runtime.ActorBase.OnPostActorMethodAsync(Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext).
    public enum ActorCallType
    {
        //
        // Summary:
        //     The method invoked is an actor interface method for a given client request.
        ActorInterfaceMethod = 0,
        //
        // Summary:
        //     The method invoked is a timer callback method.
        TimerMethod = 1,
        //
        // Summary:
        //     The method invoked on Microsoft.ServiceFabric.Actors.Runtime.IRemindable interface
        //     when a reminder fires.
        ReminderMethod = 2
    }
}
