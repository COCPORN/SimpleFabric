using System;
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

        // Generate random long
        // http://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way
        static void EnsureMinLEQMax(ref long min, ref long max)
        {
            if (min <= max)
                return;
            long temp = min;
            min = max;
            max = temp;
        }

        static bool IsModuloBiased(long randomOffset, long numbersInRange)
        {
            long greatestCompleteRange = numbersInRange * (long.MaxValue / numbersInRange);
            return randomOffset > greatestCompleteRange;
        }

        static long PositiveModuloOrZero(long dividend, long divisor)
        {
            long mod;
            Math.DivRem(dividend, divisor, out mod);
            if (mod < 0)
                mod += divisor;
            return mod;
        }

        public static long RandomLong()
        {
            byte[] buffer = new byte[8];
            rnd.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static long RandomLong(long min, long max)
        {
            EnsureMinLEQMax(ref min, ref max);
            long numbersInRange = unchecked(max - min + 1);
            if (numbersInRange < 0)
                throw new ArgumentException("Size of range between min and max must be less than or equal to Int64.MaxValue");

            long randomOffset = RandomLong();
            if (IsModuloBiased(randomOffset, numbersInRange))
                return RandomLong(min, max); // Try again
            else
                return min + PositiveModuloOrZero(randomOffset, numbersInRange);
        }

        static Random rnd = new Random();
        public void CreateRandom() 
        {
            longId = RandomLong(long.MinValue, long.MaxValue);
            ActorIdKind = ActorIdKind.Long;
        }
        #endregion

        #region Get Keys
        public Guid GetGuidId() 
        {   
            if (ActorIdKind == ActorIdKind.Guid)
            {
                return guidId;
            } else throw new InvalidOperationException("This is not a Guid Id"); 
        }

        public long GetLongId() 
        {
            if (ActorIdKind == ActorIdKind.Long)
            { 
                return longId;
            } else throw new InvalidOperationException("This is not a long id");
        }

        public string GetStringId() 
        {
            if (ActorIdKind == ActorIdKind.String) 
            {
                return stringId;
            } else throw new InvalidOperationException("This is not a string id");
             
        }
        #endregion

		public ActorIdKind ActorIdKind { get; private set; }

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
            if (other == null) return false;
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
            return Equals((ActorId)obj);
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
            if (ReferenceEquals(x, y)) return true;
            if (((object)x == null) && ((object)y == null)) return true;
            if (((object)x == null) || ((object)y == null)) return false;
            return x.Equals(y);
        }

        public static bool operator !=(ActorId x, ActorId y)
        {
            return (x == y) == false;
        }
    }
}
