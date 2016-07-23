using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;
using SimpleFabric.Actors.Runtime;
using SimpleFabric.Actors.Implementation;

namespace Actors.Client.Test
{
    /// <summary>
    /// This interface cannot be instantiated because it
    /// doesn't have an implementing class
    /// </summary>
    public interface IUnimplementedActor { }

    public interface IUnimplementedButCorrectInterface : IActor { }

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
    /// Check that ambiguous instantiation doesn't work
    /// </summary>
    public interface IMultiInterface { }
    public class MultiInterface1 : Actor, IMultiInterface { }
    public class MultiInterface2 : Actor, IMultiInterface { }

    /// <summary>
    /// This is a well-formed actor interface
    /// </summary>
    public interface ITestActor : IActor
    {
        Task<string> Hello(string world); 
    }

    /// <summary>
    /// While this forgets to derive from Actor
    /// </summary>
    public class BadTestActor : ITestActor
    {
        public Task<string> Hello(string world)
        {
            return Task.FromResult("Hello: " + world);
        }
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
        [TestInitialize]
        public void Initialize()
        {
            Actor.StateManagerCreator = () =>
            {
                return new InMemoryActorStateManager();
            };
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUnimplementedActor()
        {
            var unimplementedActor = ActorProxy.Create<IUnimplementedActor>(new ActorId("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUnimplementedButCorrectInterface()
        {
            var unimplementedActor = ActorProxy.Create<IUnimplementedButCorrectInterface>(new ActorId("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestInterfaceMissingIActorMarkerInterface()
        {
            var badInterface = ActorProxy.Create<IBadInterface>(new ActorId("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMultipleImplementation()
        {
            var badActor = ActorProxy.Create<IMultiInterface>(new ActorId("Multi"));
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
