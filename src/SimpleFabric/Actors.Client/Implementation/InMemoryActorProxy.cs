using SimpleFabric.Actors;
using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client.Implementation
{
    public class InMemoryActorProxy<T> : DynamicObject, IActor, IActorProxyImplementation
    {
        public ActorId ActorId { get; set; }

        IActor actor;

        public void Initialize()
        {          
            // TODO: This is gross and ineffient, rewrite locking 
            lock (actorRegistry)
            {
                if (actorRegistry.TryGetValue(ActorId, out actor) == false)
                {
                    // Create new actor instance and add it to registry
                    var type = typeof(T);
                    Type createType = null;

                    if (interfaceMapping.TryGetValue(type, out createType) == false)
                    {
                        var typeToCreate = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetTypes())
                                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && !p.IsInterface);
                        if (typeToCreate.Count() == 0)
                        {
                            throw new InvalidOperationException("The type " + type.Name + " has no implementation");
                        }
                        interfaceMapping.Add(type, typeToCreate.First());
                        createType = typeToCreate.First();
                    }

                    var t_actor = Activator.CreateInstance(createType);
                    var iactor = t_actor as IActor;
                    var cactor = t_actor as Actor;
                    if (t_actor != null && iactor == null)
                    {
                        throw new InvalidOperationException("The actor class needs to implement IActor-interface");
                    }
                    if (cactor == null)
                    {
                        throw new InvalidOperationException("The actor class needs to derive from the SimpleFabric.Actors.Runtime.Actor class");
                    }
                    if (iactor == null) throw new Exception("Internal error: Failed to create Actor");
                    actorRegistry.Add(ActorId, iactor);
                    actor = iactor;
                }
            }
        }
        static Dictionary<Type, Type> interfaceMapping = new Dictionary<Type, Type>();
        static Dictionary<ActorId, IActor> actorRegistry = new Dictionary<ActorId, IActor>();

        // TODO: This method needs to honor service fabric reentrancy rules, this implementation
        // is naive and will easily cause deadlocks
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                lock (actor)
                {
                    result = actor.GetType().GetMethod(binder.Name).Invoke(actor, args);
                }
                return true;
            }
            catch
            {                
                result = null;
                return false;
            }            
        }
    }
}
