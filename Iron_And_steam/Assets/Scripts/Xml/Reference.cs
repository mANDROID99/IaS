using System.Collections.Generic;
using System.Linq;

namespace IaS.Xml
{
    public class Reference<T> where T : IReferenceable
    {
        private readonly string _value;
        private T _resolvedValue;
        public T Value { get { return _resolvedValue; } }

        public Reference(string value)
        {
            _value = value;
        }

        public T Resolve(IEnumerable<T> items)
        {
            _resolvedValue = items.First(item => ("@" + item.GetId()).Equals(_value));
            return _resolvedValue;
        }
    }
}
