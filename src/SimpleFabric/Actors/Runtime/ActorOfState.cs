using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public class Actor<TState> : Actor where TState : class, new()
    {
        TState backingState = null;
        protected dynamic State { get; }
       
        
        public Actor()
        {
            backingState = StateManager.GetOrAddStateAsync("default", new TState()).Result;
       
            State = new StateProxy(backingState);            
        }

        public override async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            if (State.StateDirty == true)
            {
                await StateManager.SetStateAsync<TState>("default", backingState);
            }
            State.StateDirty = false;            
            base.OnPostActorMethodAsync(actorMethodContext).Wait();
        }

    }
}
