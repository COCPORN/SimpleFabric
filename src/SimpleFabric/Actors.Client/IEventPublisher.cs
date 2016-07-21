using System.Threading.Tasks;

namespace SimpleFabric.Actors.Client
{
    public interface IEventPublisher
    {
        Task SubscribeAsync<T>(T eventHandler);
    }
}