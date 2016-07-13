using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    //
    // Summary:
    //     Represents Timer set on an Actor
    public interface IActorTimer : IDisposable
    {
        //
        // Summary:
        //     Time when timer is first due.
        TimeSpan DueTime { get; }
        //
        // Summary:
        //     Periodic time when timer will be invoked.
        TimeSpan Period { get; }
    }
}
