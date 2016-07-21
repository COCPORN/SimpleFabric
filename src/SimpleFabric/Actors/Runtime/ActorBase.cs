using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public abstract class ActorBase
    {
        
        public string ApplicationName { get; set; }

        // Not sure how to implement this elegantly
        // without incurring constructor chaining,
        // TODO: Consider looking at this later
        bool actorIdSet = false;
        ActorId id;
        public ActorId Id
        {
            get { return id; }
            set
            {
                if (actorIdSet == true) throw new InvalidOperationException("ActorId already set");
                actorIdSet = true;
                id = value;
            }
        }

        public ILockManager LockManager
        {
            get; set;
        }
        //
        // Summary:
        //     Gets the event for the specified event interface.
        //
        // Type parameters:
        //   TEvent:
        //     Event interface type.
        //
        // Returns:
        //     Returns Event that represents the specified interface.
        protected TEvent GetEvent<TEvent>() { return default(TEvent); }
        //
        // Summary:
        //     Gets the actor reminder with specified reminder name.
        //
        // Parameters:
        //   reminderName:
        //     Name of the reminder to get.
        //
        // Returns:
        //     An Microsoft.ServiceFabric.Actors.Runtime.IActorReminder that represents an actor
        //     reminder.
        protected IActorReminder GetReminder(string reminderName) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Override this method to initialize the members, initialize state or register
        //     timers. This method is called right after the actor is activated and before any
        //     method call or reminders are dispatched on it.
        //
        // Returns:
        //     A System.Threading.Tasks.Task that represents outstanding OnActivateAsync operation.
        protected virtual async Task OnActivateAsync() {  }
        //
        // Summary:
        //     Override this method to release any resources including unregistering the timers.
        //     This method is called right before the actor is deactivated.
        //
        // Returns:
        //     A System.Threading.Tasks.Task that represents outstanding OnDeactivateAsync operation.
        protected virtual async Task OnDeactivateAsync() {  }
        //
        // Summary:
        //     This method is invoked by actor runtime an actor method has finished execution.
        //     Override this method for performing any actions after an actor method has finished
        //     execution.
        //
        // Parameters:
        //   actorMethodContext:
        //     An Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext describing the method
        //     that was invoked by actor runtime prior to this method.
        //
        // Returns:
        //     A System.Threading.Tasks.Task representing post-actor-method operation.
        //
        // Remarks:
        //     This method is invoked by actor runtime prior to: Invoking an actor interface
        //     method when a client request comes. Invoking a method on Microsoft.ServiceFabric.Actors.Runtime.IRemindable
        //     interface when a reminder fires. Invoking a timer callback when timer fires.
        public virtual async Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext) { }
        //
        // Summary:
        //     This method is invoked by actor runtime just before invoking an actor method.
        //     Override this method for performing any actions prior to an actor method is invoked.
        //
        // Parameters:
        //   actorMethodContext:
        //     An Microsoft.ServiceFabric.Actors.Runtime.ActorMethodContext describing the method
        //     that will be invoked by actor runtime after this method finishes.
        //
        // Returns:
        //     A System.Threading.Tasks.Task representing pre-actor-method operation.
        //
        // Remarks:
        //     This method is invoked by actor runtime prior to: Invoking an actor interface
        //     method when a client request comes. Invoking a method on Microsoft.ServiceFabric.Actors.Runtime.IRemindable
        //     interface when a reminder fires. Invoking a timer callback when timer fires.
        public virtual async Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext) { }
        //
        // Summary:
        //     Registers the specified reminder with actor.
        //
        // Parameters:
        //   reminderName:
        //     Name of the reminder to register.
        //
        //   state:
        //     State associated with reminder.
        //
        //   dueTime:
        //     A System.TimeSpan representing the amount of time to delay before firing the
        //     reminder. Specify negative one (-1) milliseconds to prevent reminder from firing.
        //     Specify zero (0) to fire the reminder immediately.
        //
        //   period:
        //     The time interval between firing of reminders. Specify negative one (-1) milliseconds
        //     to disable periodic firing.
        //
        // Returns:
        //     A task that represents the asynchronous registration operation. The value of
        //     TResult parameter is an Microsoft.ServiceFabric.Actors.Runtime.IActorReminder
        //     that represents the actor reminder that was registered.
        //[AsyncStateMachine(typeof(< RegisterReminderAsync > d__3))]
        //[DebuggerStepThrough]
        protected Task<IActorReminder> RegisterReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Registers a Timer for the actor.
        //
        // Parameters:
        //   asyncCallback:
        //     Callback to invoke when timer fires.
        //
        //   state:
        //     State to pass into timer callback.
        //
        //   dueTime:
        //     TimeSpan when actor timer is first due.
        //
        //   period:
        //     TimeSpan for subsequent actor timer invocation.
        //
        // Returns:
        //     Returns IActorTimer object.
        protected IActorTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period) {
            return new Timer(dueTime, period, asyncCallback, state, this);
        }
        //
        // Summary:
        //     Unregisters the specified reminder with actor.
        //
        // Parameters:
        //   reminder:
        //     The actor reminder to unregister.
        //
        // Returns:
        //     A task that represents the asynchronous unregister operation.
        //
        // Exceptions:
        //   T:System.Fabric.FabricException:
        //     When the specified reminder is not registered with actor.
        //[AsyncStateMachine(typeof(< UnregisterReminderAsync > d__0))]
        //[DebuggerStepThrough]
        protected Task UnregisterReminderAsync(IActorReminder reminder) { throw new NotImplementedException(); }
        //
        // Summary:
        //     Unregisters a Timer previously set on this actor.
        //
        // Parameters:
        //   timer:
        //     IActorTimer representing timer that needs to be unregistered..
        protected void UnregisterTimer(IActorTimer timer) { throw new NotImplementedException(); }

    }
}
