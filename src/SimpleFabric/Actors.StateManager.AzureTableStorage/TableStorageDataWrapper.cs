using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.StateManager.AzureTableStorage
{
    class TableStorageDataWrapper : TableEntity
    {
        public string Json { get; set; }

        #region Serialization
        public void SerializeObject<T>(T obj)
        {
            Json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public T DeserializeObject<T>()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Json);            
        }
        #endregion
    }
}
