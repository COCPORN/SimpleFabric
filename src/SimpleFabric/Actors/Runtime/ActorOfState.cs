using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public class Actor<TState> : Actor where TState : class
    {
        TState backingState = null;
        StateProxy StateProxy { get; }
        protected TState State = null;       
        
        public Actor()
        {
            backingState = StateManager.GetOrAddStateAsync<TState>("default", default(TState)).Result;
       
            StateProxy = new StateProxy(backingState);
            State = StateProxy.ActLike<TState>();
        }

        public override async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            if (StateProxy.StateDirty == true)
            {
                await StateManager.SetStateAsync<TState>("default", backingState);
            }
            StateProxy.StateDirty = false;
            base.OnPostActorMethodAsync(actorMethodContext).Wait();
        }

    }
}
