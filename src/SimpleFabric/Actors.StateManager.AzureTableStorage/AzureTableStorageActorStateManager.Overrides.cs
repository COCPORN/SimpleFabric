using SimpleFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.StateManager.AzureTableStorage
{
    public partial class AzureTableStorageActorStateManager
    {

        // TODO: This can be made faster by checking the in-memory store first
        public async override Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tableData = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
            if (tableData == null)
            {
                tableData = new TableStorageDataWrapper();
                tableData.PartitionKey = PartitionKey;
                tableData.RowKey = stateName;
                tableData.SerializeObject(addValue);
                await base.SetStateAsync(stateName, addValue, cancellationToken);
                await UpsertAsync(tableData, cancellationToken);
                return addValue;
            } else
            {
                var state = tableData.DeserializeObject<T>();
                var newState = updateValueFactory(stateName, state);
                var updateData = new TableStorageDataWrapper();
                updateData.PartitionKey = PartitionKey;
                updateData.RowKey = stateName;
                updateData.SerializeObject(newState);
                await UpdateAsync(updateData, cancellationToken);
                await base.SetStateAsync(stateName, newState);
                return newState;
            }
            
       
        }

        public async override Task AddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.AddStateAsync<T>(stateName, value, cancellationToken);
            var tableData = new TableStorageDataWrapper();
            tableData.PartitionKey = PartitionKey;
            tableData.RowKey = stateName;
            tableData.SerializeObject(value);
            await InsertAsync(tableData, cancellationToken);
            return;
        }

        public async override Task<T> GetOrAddStateAsync<T>(string stateName,
            T value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await base.ContainsStateAsync(stateName) == false)
            {
                var tableData = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
                if (tableData == null)
                {
                    tableData = new TableStorageDataWrapper();
                    tableData.PartitionKey = PartitionKey;
                    tableData.RowKey = stateName;
                    tableData.SerializeObject(value);
                    await InsertAsync(tableData, cancellationToken);
                    var localState = await base.GetOrAddStateAsync<T>(stateName, value, cancellationToken);
                    return localState;
                }
                else
                {
                    var localData = tableData.DeserializeObject<T>();
                    await base.SetStateAsync(stateName, localData, cancellationToken);
                    return localData;
                }

            }
            else
            {
                return await base.GetStateAsync<T>(stateName, cancellationToken);
            }
        }

        // Annoying AF, I am not sure if I can mirror the data locally here
        public async override Task<bool> ContainsStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var local = await base.ContainsStateAsync(stateName, cancellationToken);
            if (local == true) return true;

            var remoteData = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
            if (remoteData != null)
            {
                return true;
            }
            return false;
        }

        // Currently not implemented
        public override Task ClearCacheAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.ClearCacheAsync(cancellationToken);
        }

        public async override Task<T> GetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await base.GetStateAsync<T>(stateName, cancellationToken);
            }
            catch (KeyNotFoundException)
            {
                var remoteState = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
                if (remoteState == null)
                {
                    throw new KeyNotFoundException();
                }
                else
                {
                    var localState = remoteState.DeserializeObject<T>();
                    await base.SetStateAsync(stateName, localState);
                    return localState;
                }
            }
        }

        public async override Task SetStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tableData = new TableStorageDataWrapper();
            tableData.PartitionKey = PartitionKey;
            tableData.RowKey = stateName;
            tableData.SerializeObject(value);
            await UpdateAsync(tableData, cancellationToken);
            await base.SetStateAsync(stateName, value, cancellationToken);            
        }

        public override Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
            //return await base.GetStateNamesAsync(cancellationToken);
        }

        public async override Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DeleteAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
            await base.RemoveStateAsync(stateName, cancellationToken);
        }

        public async override Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tableData = new TableStorageDataWrapper();
            tableData.PartitionKey = PartitionKey;
            tableData.RowKey = stateName;
            tableData.SerializeObject(value);
            await UpdateAsync(tableData, cancellationToken);
            return await base.TryAddStateAsync(stateName, value, cancellationToken);
        }

        public async override Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var localState = await base.TryGetStateAsync<T>(stateName, cancellationToken);
            if (localState.HasValue)
            {
                return localState;
            }
            else
            {
                var remoteState = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
                if (remoteState == null)
                {
                    return new ConditionalValue<T>();
                }
                else
                {
                    var t_localState = remoteState.DeserializeObject<T>();
                    var conditionalValue = new ConditionalValue<T>(true, t_localState);
                    await base.SetStateAsync(stateName, t_localState);
                    return conditionalValue;
                }
            }
        }

        public async override Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DeleteAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
            return await base.TryRemoveStateAsync(stateName, cancellationToken);
        }

    }
}
