using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
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
    }

    [TestClass]
    public class TransientStateTest
    {
        [TestMethod]
        public async Task TestTransientStateAndRegistryLookup()
        {
            var actorRef1 = ActorProxy.Create<IIncrementActor>(new ActorId("IncrementActor"));
            var actorRef2 = ActorProxy.Create<IIncrementActor>(new ActorId("IncrementActor"));

            await actorRef1.Increment();
            await actorRef2.Increment();

            Assert.AreEqual(2, await actorRef2.GetValue());
            Assert.AreEqual(2, await actorRef1.GetValue());
            Assert.AreEqual(await actorRef1.GetValue(), await actorRef2.GetValue());
        }
    }
}
