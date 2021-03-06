﻿using ImpromptuInterface;
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
        public static IProxyCreator ActorProxyCreator { get; set; } = new InMemoryActorProxyCreator();

        public static T Create<T>(ActorId actorId, string applicationName = null) where T : class
        {
            if (typeof(T).IsInterface == false)
            {
                throw new ArgumentException("T must be an interface");
            }

            IActorProxyImplementation proxy = ActorProxyCreator.Create<T>();

            proxy.ActorId = actorId;
            proxy.ApplicationName = applicationName;

            try
            {
                proxy.Initialize().Wait();
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }

            return proxy.ActLike<T>();
        }

    }
}
