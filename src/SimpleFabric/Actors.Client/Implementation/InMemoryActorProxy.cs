using SimpleFabric.Actors;
using SimpleFabric.Actors.Runtime;
using SimpleFabric.Collections;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public class InMemoryActorProxy<T> : 
        InMemoryActorProxyBase, 
        IActor, 
        IActorProxyImplementation,
        ILockManager
    {
        public ActorId ActorId { get; set; }
        public string ApplicationName { get; set; }
        IActor iActor;
        Actor concreteActor;
        ActorWrapper<T> wrappedActor;
        static TimeoutHandler<ActorId, ActorWrapper<T>> timeoutHandler = new TimeoutHandler<ActorId, ActorWrapper<T>>();
        
        public static new int ActorLifetime
        {
            get { return InMemoryActorProxyBase.ActorLifetime; }
            set { InMemoryActorProxyBase.ActorLifetime = value;  }
        }

        public async Task Initialize()
        {
            bool actorExists;            
            lock (actorRegistry)
            {
                actorExists = actorRegistry
                              .TryGetValue(Tuple.Create(ActorId, ApplicationName),
                                           out wrappedActor);
                concreteActor = wrappedActor?.Actor;
                iActor = concreteActor as IActor;
            }
            if (actorExists == false)
            {
                // Create new actor instance and add it to registry
                var type = typeof(T);
                Type typeToCreate = null;

                if (interfaceMapping == null)
                {
                    typeToCreate = CreateInterfaceMapping(type);
                }
                else
                {
                    typeToCreate = interfaceMapping;
                }

                CreateActor(typeToCreate, out iActor, out concreteActor);

                await concreteActor.ActivateActor();
                wrappedActor = new ActorWrapper<T>(concreteActor, this);                
                timeoutHandler.Timeout(wrappedActor, ActorLifetime);

                lock (actorRegistry)
                {
                    actorRegistry.Add(Tuple.Create(ActorId, ApplicationName), wrappedActor);
                }
            }
        }        

        private static Type CreateInterfaceMapping(Type type)
        {
            Type typeToCreate;
            var typesToCreate = AppDomain.CurrentDomain.GetAssemblies()
                                            .SelectMany(s => s.GetTypes())
                                            .Where(p => type.IsAssignableFrom(p)
                                            && p.IsClass
                                            && p.IsAbstract == false
                                            && p.IsInterface == false
                                            && p.IsSubclassOf(typeof(Actor)));

            var createCount = typesToCreate.Count();

            if (createCount == 0)
            {
                throw new InvalidOperationException("The type "
                    + type.Name + " has no implementation");
            }

            if (createCount > 1)
            {
                throw new InvalidOperationException("The interface "
                    + type.Name +
                    " has multiple implementations and instantiation is ambiguous." +
                    " Make sure there is a single implementation in the current AppDomain");
            }

            typeToCreate = typesToCreate.Single();

            interfaceMapping = typeToCreate;
            return typeToCreate;
        }
     
        Tuple<ActorId, string> CreateKey()
        {
            return Tuple.Create(concreteActor.Id, ApplicationName);
        }

        public async Task RemoveActor()
        {
            lock (actorRegistry)
            {
                actorRegistry.Remove(CreateKey());
            }
            await concreteActor.DeactivateActor();
        }

        void CreateActor(Type typeToCreate, out IActor iactor, out Actor cactor)
        {
            var t_actor = Activator.CreateInstance(typeToCreate);

            iactor = t_actor as IActor;
            cactor = t_actor as Actor;
            if (t_actor == null)
            {
                throw new InvalidOperationException("Internal error: Failed to create Actor");
            }
            if (iactor == null)
            {
                throw new InvalidOperationException("The actor class needs to implement IActor-interface");
            }
            if (cactor == null)
            {
                throw new InvalidOperationException("The actor class needs to derive from the SimpleFabric.Actors.Runtime.Actor class");
            }

            cactor.Id = ActorId;
            cactor.LockManager = this;
        }

        static Type interfaceMapping;

        // This, however, makes sense to have in a static type, as it will
        // just limit the lookup of actors to the current type, which is fine
        static Dictionary<Tuple<ActorId, string>, ActorWrapper<T>> actorRegistry =
                            new Dictionary<Tuple<ActorId, string>, ActorWrapper<T>>();

        #region Locking

        // Because locking and unlocking are potentially
        // done on separate threads, these are implemented
        // with a cross thread event

        AutoResetEvent are = new AutoResetEvent(true);

        public void Lock()
        {
            are.WaitOne();
        }

        public void Unlock()
        {
            are.Set();
        }
        #endregion    

        // TODO: This method needs to honor service fabric reentrancy rules, this implementation
        // is naive and will easily cause deadlocks
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                var method = iActor.GetType().GetMethod(binder.Name);

                // "lock" the actor
                Lock();

                // Pre-call
                var methodContext = new ActorMethodContext(ActorCallType.ActorInterfaceMethod,
                                                           binder.Name);

                var preTask = concreteActor.OnPreActorMethodAsync(methodContext);
                preTask.Wait();

                result = method.Invoke(iActor, args);

                var task = result as Task;
                if (task == null)
                {
                    // "unlock" the actor as this call will not be completed 
                    // The test on the return 
                    Unlock();
                    throw new InvalidOperationException("All interface methods must be Task or Task<T>");
                }

                timeoutHandler.UpdateTimeout(concreteActor.Id, wrappedActor, ActorLifetime);

                task.ContinueWith(async (_) =>
                {
                    // Post-call
                    try
                    {
                        await concreteActor.OnPostActorMethodAsync(methodContext);
                    }
                    finally {
                        Unlock();
                    }                    
                });

                return true;
            }
            catch (Exception ex)
            {
                result = null;
                return false;
            }
        }
    }
}
