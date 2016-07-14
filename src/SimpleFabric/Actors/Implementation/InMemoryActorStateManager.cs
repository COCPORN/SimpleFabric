using SimpleFabric.Actors.Runtime;
using SimpleFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Implementation
{
    public class InMemoryActorStateManager : IActorStateManager
    {
        Dictionary<string, object> state = new Dictionary<string, object>();

        public Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            object value;
            if (state.TryGetValue(stateName, out value))
            {
                updateValueFactory(stateName, (T)value);
            }
            else
            {
                state.Add(stateName, addValue);
            }
            return Task.FromResult((T)state[stateName]);
        }

        public Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Add(stateName, value);
            return Task.FromResult(false);
        }

        public Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            return Task.FromResult(state.ContainsKey(stateName));
        }

        public Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            if (state.ContainsKey(stateName))
            {
                return Task.FromResult((T)state[stateName]);
            }
            else
            {
                state.Add(stateName, value);
                return Task.FromResult(value);
            }
        }

        public Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            return Task.FromResult((T)state[stateName]);
        }

        public Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(state.Keys.ToList().AsEnumerable());
        }

        public Task ClearCacheAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Remove(stateName);
            return Task.FromResult(false);
        }

        public Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Remove(stateName);
            state.Add(stateName, value);
            return Task.FromResult(false);
        }

        // TODO: I am not sure this is semantically correctly implemented, as the documentation page
        // for TryAddStateAsync says approximately nothing about how this is supposed to work
        public Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            if (state.ContainsKey(stateName) == false)
            {
                state.Add(stateName, value);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            object value;
            if (state.TryGetValue(stateName, out value))
            {
                var val = (T)value;
                return Task.FromResult(new ConditionalValue<T>(true, val));
            }
            else
            {
                return Task.FromResult(new ConditionalValue<T>());
            }
        }

        public Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            if (state.ContainsKey(stateName))
            {
                state.Remove(stateName);
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
