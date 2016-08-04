using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public class StateProxy : DynamicObject
    {
        public bool StateDirty { get; set; } = false;

        object proxiedObject;

        public StateProxy(object proxiedObject)
        {
            this.proxiedObject = proxiedObject;
        }

        protected PropertyInfo GetPropertyInfo(string propertyName)
        {
            return proxiedObject.GetType().GetProperties().First
            (propertyInfo => propertyInfo.Name == propertyName);
        }

        protected virtual object GetMember(string propertyName)
        {
            return GetPropertyInfo(propertyName).GetValue(proxiedObject, null);
        }

        protected virtual void SetMember(string propertyName, object value)
        {
            GetPropertyInfo(propertyName).SetValue(proxiedObject, value, null);            
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetMember(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            StateDirty = true;
            SetMember(binder.Name, value);
            return true;
        }
    }
}
