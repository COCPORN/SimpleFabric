using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Client.Implementation;
using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors.Client.Test
{

    public interface INagger : IActor
    {
        Task StopTimer();
        Task<int> GetNumberOfNags();
    }

    public class Nagger : Actor, INagger
    {
        IActorTimer timer;
        List<string> nags = new List<string>();

        public Nagger()
        {
            timer = RegisterTimer(Nag, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(300));
        }

        Task Nag(object state)
        {
            nags.Add("People who annoy you.");
            return Task.FromResult(true);
        }

        protected override Task OnDeactivateAsync()
        {
            if (timer != null)
            {
                UnregisterTimer(timer);
            }
            return base.OnDeactivateAsync();
        }

        public Task<int> GetNumberOfNags()
        {
            return Task.FromResult(nags.Count);
        }

        public Task StopTimer()
        {
            if (timer != null)
            {
                UnregisterTimer(timer);
            }
            return Task.FromResult(true);
        }
    }

    [TestClass]
    public class ActorTimerTests
    {
        [TestInitialize]
        public void Init()
        {
            InMemoryActorProxyBase.ActorLifetime = InMemoryActorProxyBase.DefaultActorLifetime;
        }


        // TODO: This shit is notoriously hard (or might even be impossible)
        // without mocking out time, but that sorts of defeats the purpose.
        // This test will work sometimes. Oooooh-sometimes.
        [TestMethod]
        public async Task TestNagger()
        {
            var mahNagger = ActorProxy.Create<INagger>(new ActorId("OG"));            
            await Task.Delay(70);
            Assert.AreEqual(1, await mahNagger.GetNumberOfNags());
            await Task.Delay(100);
            Assert.AreEqual(1, await mahNagger.GetNumberOfNags());
            await Task.Delay(100);
            Assert.AreEqual(1, await mahNagger.GetNumberOfNags());
            await Task.Delay(100);
            Assert.AreEqual(2, await mahNagger.GetNumberOfNags());
            await mahNagger.StopTimer();
            await Task.Delay(200);
            Assert.AreEqual(2, await mahNagger.GetNumberOfNags());            
        }

    }
}
