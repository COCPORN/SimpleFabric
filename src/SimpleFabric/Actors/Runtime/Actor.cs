﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public abstract class Actor : ActorBase
    {
        protected Actor() { }

        //
        // Summary:
        //     Saves all the state changes (add/update/remove) that were made since last call
        //     to Microsoft.ServiceFabric.Actors.Runtime.Actor.SaveStateAsync, to the actor
        //     state provider associated with the actor.
        //
        // Returns:
        //     A task that represents the asynchronous save operation.
        protected Task SaveStateAsync() { throw new NotImplementedException(); }

        //
        // Summary:
        //     Gets the state manager for Microsoft.ServiceFabric.Actors.Runtime.Actor which
        //     can be used to get/add/update/remove named states.
        public IActorStateManager StateManager { get; }
    }
}