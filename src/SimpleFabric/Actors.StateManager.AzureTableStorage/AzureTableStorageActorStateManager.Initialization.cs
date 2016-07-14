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
    public partial class AzureTableStorageActorStateManager : InMemoryActorStateManager, IActorAwareStateManager
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

    
    }
}
