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
    internal class ListContainer<T>
    {
        [ThriftSMember(1)]
        public List<T> Value { get; set; }

        public ListContainer()
        {
        }

        public ListContainer(List<T> value)
        {
            Value = value;
        }
    }
}
