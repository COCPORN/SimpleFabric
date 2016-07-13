using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;

namespace Actors.Client.Test
{
    public interface IUnimplementedActor
    {

    }

    public interface IBadInterface
    {

    }

    public class TestBadInterface : IBadInterface
    {

    }

    public interface ITestActor : IActor
    {
        Task<string> Hello(string world); 
    }

    public class TestActor : ITestActor
    {
        public Task<string> Hello(string world)
        {
            return Task.FromResult("Hello: " + world);
        }
    }

    [TestClass]
    public class SimpleActorProxyTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUnimplementedActor()
        {
            var unimplementedActor = ActorProxy.Create<IUnimplementedActor>(new ActorId());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInterfaceMissingIActorMarkerInterface()
        {
            var badInterface = ActorProxy.Create<IBadInterface>(new ActorId());
        }

        [TestMethod]
        public void TestSuccessfulActorCreation()
        {
            var actorProxy = ActorProxy.Create<ITestActor>(new ActorId());
            var response = actorProxy.Hello("World");
            Assert.AreEqual("Hello: World", response.Result);
        }
    }
}
