using ImpromptuInterface;
using SimpleFabric.Actors.Client.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client
{
    /// <summary>
    /// 
    /// Here are some potentially valuable resources:
    /// 
    /// http://stackoverflow.com/questions/8387004/how-to-make-a-simple-dynamic-proxy-in-c-sharp
    /// https://github.com/Curit/DynamicProxy
    /// 
    /// This crude implementation uses ImpromptuInterface and DynamicObject
    /// 
    /// </summary>

    public abstract class ActorProxy
    {

        public static T Create<T>(ActorId actorId) where T : class
        {
            if (typeof(T).IsInterface == false)
            {
                throw new ArgumentException("T must be an interface");
            }

            var inMemoryActorProxy = new InMemoryActorProxy<T>();
            inMemoryActorProxy.ActorId = actorId;
            inMemoryActorProxy.Initialize();
            return inMemoryActorProxy.ActLike<T>();
        }
       
    }
}
