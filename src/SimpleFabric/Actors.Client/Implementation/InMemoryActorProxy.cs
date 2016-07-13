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
        public string ApplicationName { get; set; }
        IActor actor;

        public void Initialize()
        {          
            // TODO: This is gross and ineffient, rewrite locking 
            lock (actorRegistry)
            {
                if (actorRegistry.TryGetValue(Tuple.Create(ActorId, ApplicationName), out actor) == false)
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

                    IActor iactor;
                    Actor cactor;
                    CreateActor(createType, out iactor, out cactor);

                    cactor.Id = ActorId;
                    actorRegistry.Add(Tuple.Create(ActorId, ApplicationName), iactor);
                    actor = iactor;
                }
            }
        }

        private static void CreateActor(Type createType, out IActor iactor, out Actor cactor)
        {
            var t_actor = Activator.CreateInstance(createType);

            iactor = t_actor as IActor;
            cactor = t_actor as Actor;
            if (t_actor == null) throw new InvalidOperationException("Internal error: Failed to create Actor");
            if (iactor == null)
            {
                throw new InvalidOperationException("The actor class needs to implement IActor-interface");
            }
            if (cactor == null)
            {
                throw new InvalidOperationException("The actor class needs to derive from the SimpleFabric.Actors.Runtime.Actor class");
            }
        }

        static Dictionary<Type, Type> interfaceMapping = new Dictionary<Type, Type>();
        static Dictionary<Tuple<ActorId, string>, IActor> actorRegistry = new Dictionary<Tuple<ActorId, string>, IActor>();

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
