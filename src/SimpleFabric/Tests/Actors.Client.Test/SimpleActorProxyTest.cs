using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Runtime;

namespace Actors.Client.Test
{
    /// <summary>
    /// This interface cannot be instantiated because it
    /// doesn't have an implementing class
    /// </summary>
    public interface IUnimplementedActor { }

    /// <summary>
    /// This interface is bad, because it doesn't derive
    /// from IActor
    /// </summary>
    public interface IBadInterface { }
    
    /// <summary>
    /// So this implementation won't help
    /// </summary>
    public class TestBadInterface : IBadInterface { }

    /// <summary>
    /// This is a well-formed actor interface
    /// </summary>
    public interface ITestActor : IActor
    {
        Task<string> Hello(string world); 
    }

    /// <summary>
    /// And this is a simple implementation of it
    /// </summary>
    public class TestActor : Actor, ITestActor
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
            var unimplementedActor = ActorProxy.Create<IUnimplementedActor>(new ActorId("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInterfaceMissingIActorMarkerInterface()
        {
            var badInterface = ActorProxy.Create<IBadInterface>(new ActorId("Test"));
        }

        [TestMethod]
        public void TestSuccessfulActorCreation()
        {
            var actorProxy = ActorProxy.Create<ITestActor>(new ActorId("Test"));
            var response = actorProxy.Hello("World");
            Assert.AreEqual("Hello: World", response.Result);
        }
    }
}
