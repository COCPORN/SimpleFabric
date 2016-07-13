using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Data
{
    public struct ConditionalValue<TValue>
    {
        bool hasValue;
        TValue value;

        public ConditionalValue(bool hasValue, TValue value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        public bool HasValue { get { return hasValue; } }
        public TValue Value {  get { return value; } }
    }
}
