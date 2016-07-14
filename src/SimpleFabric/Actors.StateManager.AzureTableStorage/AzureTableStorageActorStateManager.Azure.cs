using Microsoft.WindowsAzure.Storage.Table;
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

    }
}
