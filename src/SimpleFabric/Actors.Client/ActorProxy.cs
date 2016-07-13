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
        public static ActorProxyType ActorProxyType
        {
            get { return ProxyFactory.ActorProxyType; }
            set { ProxyFactory.ActorProxyType = value; }
        }

        public static T Create<T>(ActorId actorId) where T : class
        {
            if (typeof(T).IsInterface == false)
            {
                throw new ArgumentException("T must be an interface");
            }

            var proxy = ProxyFactory.CreateProxy<T>();

            proxy.ActorId = actorId;
            proxy.Initialize();
            return proxy.ActLike<T>();
        }

    }
}
