using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors
{
    public interface IActorEventPublisher
    {
    }


    public interface IActorEventPublisher<T> : IActorEventPublisher where T : IActorEvents
    {
    }

}
