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
    public interface IPersistedIncrementor : IActor
    {
        Task Increment();
        Task<int> GetValue();
    }


    public class PersistedIncrementor : Actor, IPersistedIncrementor
    {        
        public async Task<int> GetValue()
        {
            var cv = await StateManager.TryGetStateAsync<int>("counter");
            if (cv.HasValue == false)
            {
                await StateManager.SetStateAsync<int>("counter", 0);
                return 0;
            }
            else
            {
                return cv.Value;
            }
        }

        public async Task Increment()
        {            
            await StateManager.AddOrUpdateStateAsync("counter", 0, (statename, currentValue) =>
            {
                return ++currentValue;
            });
        }
    }

    [TestClass]
    public class InMemoryActorStateManagerTest
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
        public async Task TestSimpleState()
        {
            var actor = ActorProxy.Create<IPersistedIncrementor>(new ActorId("PersistentIncrementor"));
            var actor2 = ActorProxy.Create<IPersistedIncrementor>(new ActorId("PersistentIncrementor"));

            Assert.AreEqual(0, await actor.GetValue());
            Assert.AreEqual(0, await actor2.GetValue());

            await actor.Increment();
            await actor2.Increment();

            Assert.AreEqual(2, await actor.GetValue());
            Assert.AreEqual(2, await actor2.GetValue());
            Assert.AreEqual(await actor.GetValue(), await actor2.GetValue());
        }
    }
}
