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
        public static string TableStorageConnection { get; set; }
        public static string TableStorageTable { get; set; }

        protected CloudStorageAccount account;
        static protected CloudTableClient client;
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
            if (client != null && table != null) return;

            GetConfiguration();

            if (Actor == null)
            {
                throw new InvalidOperationException("Actor has not been set, cannot continue");
            }
            
            if (client == null)
            {
                if (string.IsNullOrEmpty(TableStorageConnection))
                {
                    if (ConfigurationManager.ConnectionStrings["SimpleFabric.AzureTableStorage.Connection"] == null)
                        throw new ConfigurationErrorsException("Missing SimpleFabric.AzureTableStorage.Connection AppSetting");

                    TableStorageConnection = ConfigurationManager.ConnectionStrings["SimpleFabric.AzureTableStorage.Connection"].ConnectionString;
                }
                account = CloudStorageAccount.Parse(TableStorageConnection);
                client = account.CreateCloudTableClient();
            }

            if (table == null)
            {
                TableStorageTable.Replace("{{actorTypeName}}", Actor.GetType().Name);
                table = client.GetTableReference(TableStorageTable);
                table.CreateIfNotExists();
            }                        
        }

        private void GetConfiguration()
        {
            if (string.IsNullOrEmpty(TableStorageTable))
                TableStorageTable = ConfigurationManager.AppSettings["AzureTableStorage.Table"];

            if (TableStorageTable == null)
            {
                throw new ConfigurationErrorsException("Missing configuration key SimpleFabric.AzureTableStorage.Table");
            }
        }

    
    }
}
