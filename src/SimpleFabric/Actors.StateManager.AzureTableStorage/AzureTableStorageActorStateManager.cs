using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SimpleFabric.Actors.Implementation;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SimpleFabric.Data;

namespace SimpleFabric.Actors.StateManager.AzureTableStorage
{
    /// <summary>
    /// For this implementation of the IActorStateManager, the partitionkey
    /// is composed of the type of Actor, combined with the type of key
    /// (long, Guid, string), combined with the id of that type turned into 
    /// a string. This is (probably) unique enough.
    /// 
    /// The rowkey is the state entry into the state manager, and the data is 
    /// the state JSON serialized.
    /// </summary>
    public class AzureTableStorageActorStateManager : InMemoryActorStateManager, IActorAwareStateManager
    {
        string tableStorageConnection;
        string tableStorageTable;

        protected CloudStorageAccount account;
        protected CloudTableClient client;
        protected CloudTable table;

        public Actor Actor { get; set; }

        string partitionKey;
        string PartitionKey
        {
            get
            {
                if (partitionKey == null)
                {
                    partitionKey = PartitionKeyCreator.CalculatePartitionKey(Actor);
                }
                return partitionKey;
            }
        }

        public void Initialize()
        {
            GetConfiguration();
            tableStorageTable.Replace("{{actorTypeName}}", Actor.GetType().Name);

            if (ConfigurationManager.ConnectionStrings["SimpleFabric.AzureTableStorage.Connection"] == null)
                throw new ConfigurationErrorsException("Missing SimpleFabric.AzureTableStorage.Connection AppSetting");

            var connectionString = ConfigurationManager.ConnectionStrings["SimpleFabric.AzureTableStorage.Connection"].ConnectionString;
            account = CloudStorageAccount.Parse(connectionString);
            client = account.CreateCloudTableClient();

            table = client.GetTableReference(tableStorageTable);
            table.CreateIfNotExists();
        }

        private void GetConfiguration()
        {
            tableStorageTable = ConfigurationManager.AppSettings["AzureTableStorage.Table"];

            if (tableStorageTable == null)
            {
                throw new ConfigurationErrorsException("Missing configuration key SimpleFabric.AzureTableStorage.Table");
            }
        }

        #region Overrides

        #region Azure calls
        public async Task<T> GetAsync<T>(string partitionKey, string rowKey, CancellationToken cancellationToken) where T : class, ITableEntity
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation, cancellationToken);
            T obj = result.Result as T;
            return obj;
        }

        public async Task UpsertAsync<T>(T obj, CancellationToken cancellationToken) where T : ITableEntity
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(obj);

            TableResult result = await table.ExecuteAsync(insertOrReplaceOperation, cancellationToken);
        }

        public async Task InsertAsync<T>(T obj, CancellationToken cancellationToken) where T : ITableEntity
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrMerge(obj);

            TableResult result = await table.ExecuteAsync(insertOrReplaceOperation, cancellationToken);
        }

        public async Task UpdateAsync<T>(T obj, CancellationToken cancellationToken) where T : ITableEntity
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(obj);

            TableResult result = await table.ExecuteAsync(insertOrReplaceOperation, cancellationToken);
        }
        #endregion

        public async override Task<T> AddOrUpdateStateAsync<T>(string stateName, T addValue, Func<string, T, T> updateValueFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await base.AddOrUpdateStateAsync<T>(stateName, addValue, updateValueFactory, cancellationToken);
            var tableData = new TableStorageDataWrapper();
            tableData.PartitionKey = PartitionKey;
            tableData.RowKey = stateName;
            tableData.SerializeObject(state);
            await UpsertAsync(tableData, cancellationToken);
            return state;
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
            catch (IndexOutOfRangeException)
            {
                var remoteState = await GetAsync<TableStorageDataWrapper>(PartitionKey, stateName, cancellationToken);
                if (remoteState == null)
                {
                    throw new IndexOutOfRangeException();
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
            await base.SetStateAsync<T>(stateName, value, cancellationToken);
            return;
        }

        public override Task<IEnumerable<string>> GetStateNamesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override Task RemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TryAddStateAsync<T>(string stateName, T value, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override Task<ConditionalValue<T>> TryGetStateAsync<T>(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TryRemoveStateAsync(string stateName, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
