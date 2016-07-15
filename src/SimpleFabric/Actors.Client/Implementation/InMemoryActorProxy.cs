﻿using SimpleFabric.Actors;
using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public class InMemoryActorProxy<T> : DynamicObject, IActor, IActorProxyImplementation
    {
        public ActorId ActorId { get; set; }
        public string ApplicationName { get; set; }
        IActor iActor;
		Actor concreteActor;

        public void Initialize()
        {
            bool actorExists;
            lock (actorRegistry)
            {
                actorExists = actorRegistry
                              .TryGetValue(Tuple.Create(ActorId, ApplicationName), 
                                           out concreteActor);
                iActor = concreteActor as IActor;
            }
            if (actorExists == false)
            {
                // Create new actor instance and add it to registry
                var type = typeof(T);
                Type typeToCreate = null;

                if (interfaceMapping.TryGetValue(type, out typeToCreate) == false)
                {
                    var typesToCreate = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => type.IsAssignableFrom(p)
                                        && p.IsClass
                                        && !p.IsAbstract
                                        && !p.IsInterface
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

                    interfaceMapping.Add(type, typeToCreate);                    
                }

                IActor iactor;
                Actor cactor;

                CreateActor(typeToCreate, out iactor, out cactor);
				concreteActor = cactor;
                ActivateActor(cactor);
                
                lock (actorRegistry)
                {
                    actorRegistry.Add(Tuple.Create(ActorId, ApplicationName), cactor);
                }
                iActor = iactor;
            }
        }

        async void ActivateActor(Actor actor) 
        {
            actor.Id = ActorId;
            await actor.OnActivateAsync();
        }

        async void DeactivateActor(Actor actor) 
        {
            await actor.OnDeactivateAsync();
        }

        private static void CreateActor(Type typeToCreate, out IActor iactor, out Actor cactor)
        {
            var t_actor = Activator.CreateInstance(typeToCreate);

            iactor = t_actor as IActor;
            cactor = t_actor as Actor;
            if (t_actor == null) {
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
        }

		// TODO: Having this as a static field like this doesn't make sense,
		// it can just as well be a single static field, as the lookup will
		// be done on type
        static Dictionary<Type, Type> interfaceMapping = 
                            new Dictionary<Type, Type>();

		// This, however, makes sense to have in a static type, as it will
		// just limit the lookup of actors to the current type, which is fine
        static Dictionary<Tuple<ActorId, string>, Actor> actorRegistry = 
                            new Dictionary<Tuple<ActorId, string>, Actor>();

        #region Locking

        // Because locking and unlocking are potentially
        // done on separate threads, these are implemented
        // with a cross thread event

        AutoResetEvent are = new AutoResetEvent(true);

        void Lock()
        {
            are.WaitOne();
        }

        void Unlock()
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

#if false
                // For some reason this doesn't work for Task with generic
                if (method.ReturnType != typeof(Task) 
                    && method.ReturnType != typeof(Task<>))
                {
                    throw new InvalidOperationException("All interface methods must be Task or Task<T>");
                }
#endif

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

                task.ContinueWith(async (_) =>
                {
                    // Post-call
                    await concreteActor.OnPostActorMethodAsync(methodContext);
                    Unlock();
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
