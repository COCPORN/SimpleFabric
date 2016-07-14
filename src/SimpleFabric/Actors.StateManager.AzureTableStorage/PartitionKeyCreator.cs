using System;
using SimpleFabric.Actors.Runtime;

namespace Actors.StateManager.AzureTableStorage
{
    class PartitionKeyCreator
    {
        public static string CalculatePartitionKey(Actor actor)
        {        
            if (actor.Id == null) throw new InvalidOperationException("Actor is missing ActorId");

            var r_partitionKey = 
                actor.GetType().Name + ":" 
                + actor.Id.ActorIdKind.ToString() + ":";

            switch (actor.Id.ActorIdKind)
            {
                case SimpleFabric.Actors.ActorIdKind.Guid:
                    r_partitionKey += actor.Id.GetGuidId().ToString();
                    break;
                case SimpleFabric.Actors.ActorIdKind.Long:
                    r_partitionKey += actor.Id.GetLongId().ToString();
                    break;
                case SimpleFabric.Actors.ActorIdKind.String:
                    r_partitionKey += actor.Id.GetStringId().ToString();
                    break;
            }
            return r_partitionKey;
        }
    }
}
