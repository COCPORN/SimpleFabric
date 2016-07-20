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
    public interface IIncrementActor : IActor
    {
        Task Increment();
        Task<int> GetValue();
        Task<bool> GetActive();
    }

    public class IncrementActor : Actor, IIncrementActor
    {
        int i = 0;

        public Task<int> GetValue()
        {
            return Task.FromResult(i);
        }

        public Task Increment()
        {
            i++;
            return Task.FromResult(true);
        }

        public bool Deactivated { get; set; } = false;

        protected override Task OnDeactivateAsync()
        {
            Deactivated = true;
            return base.OnDeactivateAsync();
        }

        public Task<bool> GetActive()
        {
            return Task.FromResult(Deactivated == false);
        }
    }

  

    [TestClass]
    public class TransientStateTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Actor.StateManagerCreator = () =>
            {
                return new InMemoryActorStateManager();
            };
        }


        [TestMethod]
        public async Task TestTransientStateAndRegistryLookup()
        {
            var actorRef1 = ActorProxy.Create<IIncrementActor>(new ActorId("IncrementActor"));
            var actorRef2 = ActorProxy.Create<IIncrementActor>(new ActorId("IncrementActor"));

            await actorRef1.Increment();
            await actorRef2.Increment();

            Assert.AreEqual(2, await actorRef1.GetValue());
            Assert.AreEqual(2, await actorRef2.GetValue());
            Assert.AreEqual(await actorRef1.GetValue(), await actorRef2.GetValue());
        }
    }
}
