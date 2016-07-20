using System;

namespace SimpleFabric.Collections
{

    public interface IExpiring<T>
    {
        bool TimedOut { get; set; }
        Action TimeoutHandler { get; }
        T Id { get; }
        void OnTimeout();
    }
}
