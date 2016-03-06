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
    internal class ValueContainer<T>
    {
        [ThriftSMember(1)]
        public T Value { get; set; }

        public ValueContainer()
        {
        }

        public ValueContainer(T value)
        {
            Value = value;
        }
    }
}
