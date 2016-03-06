using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common;
using ThriftS.Common.Attributes;

namespace ThriftS.Serializer
{
    [ThriftSModel]
    internal class MapContainer<TKey, TValue>
    {
        [ThriftSMember(1)]
        public Dictionary<TKey, TValue> Value { get; set; }

        public MapContainer()
        {
        }

        public MapContainer(Dictionary<TKey, TValue> value)
        {
            Value = value;
        }
    }
}
