using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Client.Implementation;
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
    public class ActorLifetimeTests
    {
        [TestInitialize]
        public void Init()
        {
            // Setup actors to die rather quickly
            InMemoryActorProxyBase.ActorLifetime = 100;
            Actor.StateManagerCreator = () =>
            {
                return new InMemoryActorStateManager();
            };
        }

        [TestMethod]
        public async Task TestActorDeactivation()
        {
            var actorRef1 = ActorProxy.Create<IIncrementActor>(new ActorId("Lifetime:IncrementActor"));
            Assert.AreEqual(true, await actorRef1.GetActive());
            await Task.Delay(100);

            // This might be a little odd, as we're calling a method
            // on a deactivated actor. Not sure exactly what should happen
            // here, but I suppose it should actually be automatically
            // reactivated. Test will still work, tho, as the bool is 
            // only set to deactivated.
            Assert.AreEqual(false, await actorRef1.GetActive());
        }

        [TestMethod]
        public async Task TestActorKeepAlive()
        {
            var actorRef1 = ActorProxy.Create<IIncrementActor>(new ActorId("Lifetime:IncrementActor"));
            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(true, await actorRef1.GetActive());
                await Task.Delay(10);
                Assert.AreEqual(true, await actorRef1.GetActive());
                await Task.Delay(10);
                Assert.AreEqual(true, await actorRef1.GetActive());
                await Task.Delay(10);
                Assert.AreEqual(true, await actorRef1.GetActive());
                await Task.Delay(10);
                Assert.AreEqual(true, await actorRef1.GetActive());
                await Task.Delay(10);
            }

            await Task.Delay(100);

            // This might be a little odd, as we're calling a method
            // on a deactivated actor. Not sure exactly what should happen
            // here, but I suppose it should actually be automatically
            // reactivated. Test will still work, tho, as the bool is 
            // only set to deactivated.
            Assert.AreEqual(false, await actorRef1.GetActive());
        }

    }
}
