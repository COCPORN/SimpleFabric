using SimpleFabric.Actors;
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

                        interfaceMapping.Add(type, typeToCreate.First());
                        createType = typeToCreate.First();
                    }

                    var actor = Activator.CreateInstance(createType) as IActor;
                    if (actor == null) throw new Exception("Internal error: Failed to create Actor");
                    actorRegistry.Add(ActorId, actor);
                }
            }
        }
        static Dictionary<Type, Type> interfaceMapping = new Dictionary<Type, Type>();
        static Dictionary<ActorId, IActor> actorRegistry = new Dictionary<ActorId, IActor>();

        // TODO: This method needs to single thread access to actors
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                result = actor.GetType().GetMethod(binder.Name).Invoke(actor, args);
                return true;
            } catch
            {
                result = null;
                return false;
            }
            
        }
    }
}
