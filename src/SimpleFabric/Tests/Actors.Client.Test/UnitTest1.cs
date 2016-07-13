using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using SimpleFabric.Actors;
using SimpleFabric.Actors.Client;

namespace Actors.Client.Test
{
    public interface ITestActor
    {
        Task<string> Hello(string world); 
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var actorProxy = ActorProxy.Create<ITestActor>(new ActorId());
            var response = actorProxy.Hello("World");

        }
    }
}
