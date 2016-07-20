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
            InMemoryActorProxyBase.ActorLifetime = 1000;
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
            await Task.Delay(1000);
            Assert.AreEqual(false, await actorRef1.GetActive());
        }

    }
}
