using SimpleFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SimpleFabric.Actors.Implementation;
using System.Configuration;

namespace SimpleFabric.Actors.StateManager.AzureTableStorage
{
    public class AzureTableStorageActorStateManager : InMemoryActorStateManager, IActorAwareStateManager
    {
        string tableStorageConnection;
        string tableStorageTable;

        public Actor Actor { get; set; }

        public AzureTableStorageActorStateManager()
        {
            GetConfiguration();
        }

        private void GetConfiguration()
        {
            tableStorageConnection = ConfigurationManager.AppSettings["AzureTableStorage.Connection"];
            tableStorageTable = ConfigurationManager.AppSettings["AzureTableStorage.Table"];
        }

        public void Initialize()
        {
            tableStorageTable.Replace("{{actorTypeName}}", Actor.GetType().Name);
        }
    }
}
