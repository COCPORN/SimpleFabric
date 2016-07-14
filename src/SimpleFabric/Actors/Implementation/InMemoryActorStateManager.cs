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

        public virtual Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            object value;
            if (state.TryGetValue(stateName, out value))
            {
                var newState = updateValueFactory(stateName, (T)value);
                state[stateName] = newState;
            }
            else
            {
                state.Add(stateName, addValue);
            }
            return Task.FromResult((T)state[stateName]);
        }

        public virtual Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Add(stateName, value);
            return Task.FromResult(false);
        }

        public virtual Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            return Task.FromResult(state.ContainsKey(stateName));
        }

        public virtual Task<T> GetOrAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
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

        public virtual Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            return Task.FromResult((T)state[stateName]);
        }

        public virtual Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(state.Keys.ToList().AsEnumerable());
        }

        public virtual Task ClearCacheAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public virtual Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Remove(stateName);
            return Task.FromResult(false);
        }

        public virtual Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (stateName == null) throw new ArgumentNullException("stateName");
            state.Remove(stateName);
            state.Add(stateName, value);
            return Task.FromResult(false);
        }

        // TODO: I am not sure this is semantically correctly implemented, as the documentation page
        // for TryAddStateAsync says approximately nothing about how this is supposed to work
        public virtual Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
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

        public virtual Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
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

        public virtual Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
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
