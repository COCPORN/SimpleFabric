using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Implementation;
using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors.Client.Test
{
    [TestClass]
    public class InMemoryRegistryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Actor.StateManagerCreator = () =>
            {
                return new InMemoryActorStateManager();
            };
        }

        /// <summary>
        /// This test shows that actors with the same id in different applications
        /// are treated as different actors
        /// </summary>        
        [TestMethod]
        public async Task TestApplicationName()
        {
            var actorRef1 = ActorProxy.Create<IIncrementActor>(new ActorId("AN:IncrementActor"));
            var actorRef2 = ActorProxy.Create<IIncrementActor>(new ActorId("AN:IncrementActor"));
            var actorRef3 = ActorProxy.Create<IIncrementActor>(new ActorId("AN:IncrementActor"), "DummyApp");
            var actorRef4 = ActorProxy.Create<IIncrementActor>(new ActorId("AN:IncrementActor"), "DummyApp");

            await actorRef1.Increment();
            await actorRef2.Increment();
            await actorRef3.Increment();
            await actorRef4.Increment();
            
            Assert.AreEqual(2, await actorRef1.GetValue());
            Assert.AreEqual(2, await actorRef2.GetValue());
            Assert.AreEqual(2, await actorRef3.GetValue());
            Assert.AreEqual(2, await actorRef4.GetValue());
            Assert.AreEqual(await actorRef1.GetValue(), await actorRef2.GetValue());
            Assert.AreEqual(await actorRef3.GetValue(), await actorRef4.GetValue());
        }
    }
}
