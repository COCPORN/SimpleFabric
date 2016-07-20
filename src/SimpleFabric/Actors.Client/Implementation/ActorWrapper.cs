using SimpleFabric.Actors.Client.Implementation;
using SimpleFabric.Actors.Runtime;
using SimpleFabric.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public class ActorWrapper<T> : IExpiring<ActorId>, IActor
    {
        public Actor Actor { get; private set; }        
        
        public InMemoryActorProxy<T> ActorProxy { get; private set; }

        public ActorWrapper(Actor actor, InMemoryActorProxy<T> actorProxy)
        {
            Actor = actor;
            ActorProxy = actorProxy;
        }

        public ActorId Id
        {
            get
            {
                return Actor.Id;
            }
        }

        public bool TimedOut
        {
            get; set;
        }

        public Action TimeoutHandler
        {
            get; set;
        }

        public void OnTimeout()
        {
            ActorProxy.RemoveActor().Wait();
        }
    }
}
