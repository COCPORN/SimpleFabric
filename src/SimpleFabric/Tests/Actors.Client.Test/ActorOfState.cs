using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Client.Implementation;
using SimpleFabric.Actors.Implementation;
using SimpleFabric.Actors.Runtime;
using System.Threading.Tasks;

namespace Actors.Client.Test
{

    public interface IStatefulIncrementor : IActor
    {
        Task Increment();
        Task<int> GetCurrentValue();
    }

    public class IncrementorState
    {
        public int Counter { get; set; }
    }

    public class StatefulIncrementor : Actor<IncrementorState>, IStatefulIncrementor
    {
        public Task Increment()
        {
            State.Counter++;
            return Task.FromResult(true);
        }

        public Task<int> GetCurrentValue()
        {
            return Task.FromResult(State.Counter);
        }
    }


    [TestClass]
    public class ActorOfState
    {
        [TestInitialize]
        public void Init()
        {
            // Setup actors to die rather quickly
            InMemoryActorProxyBase.ActorLifetime = InMemoryActorProxyBase.DefaultActorLifetime;
            Actor.StateManagerCreator = () =>
            {
                return new InMemoryActorStateManager();
            };
        }

        [TestMethod]
        public async Task TestStatefulIncrementor()
        {
            var actor1 = ActorProxy.Create<IStatefulIncrementor>(new ActorId("StatefulIncrementor1"));
            Assert.AreEqual(0, await actor1.GetCurrentValue());
            await actor1.Increment();
            Assert.AreEqual(1, await actor1.GetCurrentValue());
        }
    }
}
