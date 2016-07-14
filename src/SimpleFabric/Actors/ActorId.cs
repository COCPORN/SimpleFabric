﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors
{
    public sealed class ActorId : IEquatable<ActorId>, IComparable<ActorId>
    {
        #region Construction
        Guid guidId;
        public ActorId(Guid id) {
            guidId = id;
            ActorIdKind = ActorIdKind.Guid;
        }

        long longId;
        public ActorId(long id) {
            longId = id;
            ActorIdKind = ActorIdKind.Long;
        }

        string stringId;
        public ActorId(string id) {
            stringId = id;
            ActorIdKind = ActorIdKind.String;
        }
        #endregion

        #region Get Keys
        public Guid GetGuidId() { return guidId; }
        public long GetLongId() { return longId; }
        public string GetStringId() { return stringId; }
        #endregion

        public ActorIdKind ActorIdKind { get; }

        /// <summary>
        /// I have no idea how to implement this correctly
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ActorId other)
        {
            if (other.ActorIdKind != ActorIdKind) return ActorIdKind.CompareTo(other.ActorIdKind);
            else
            {
                switch (ActorIdKind)
                {
                    case ActorIdKind.String:
                        return stringId.CompareTo(other.stringId);
                    case ActorIdKind.Long:
                        return longId.CompareTo(other.longId);
                    case ActorIdKind.Guid:
                        return guidId.CompareTo(other.guidId);
                    default:
                        throw new InvalidOperationException("This should never happen");
                }
            }
        }

        public bool Equals(ActorId other)
        {
            if (ActorIdKind != other.ActorIdKind) return false;
            switch (ActorIdKind)
            {
                case ActorIdKind.String:
                    return stringId == other.stringId;
                case ActorIdKind.Long:
                    return longId == other.longId;
                case ActorIdKind.Guid:
                    return guidId == other.guidId;
                default:
                    throw new InvalidOperationException("This should never happen");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is ActorId == false) return false;
            return Equals(obj as ActorId);
        }

        public override int GetHashCode()
        {
            switch (ActorIdKind)
            {
                case ActorIdKind.String:
                    return stringId.GetHashCode();
                case ActorIdKind.Long:
                    return longId.GetHashCode();
                case ActorIdKind.Guid:
                    return guidId.GetHashCode();
                default:
                    throw new InvalidOperationException("This should never happen");
            }
        }

        // TODO: Not sure how this should be implemented in this case
        public long GetPartitionKey()
        {
            return 0L;
        }

        public static bool operator ==(ActorId x, ActorId y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(ActorId x, ActorId y)
        {
            return !x.Equals(y);
        }
    }
}
