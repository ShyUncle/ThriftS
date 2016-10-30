using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common.Attributes;

namespace ThriftS.Serializer
{
    [ThriftSModel]
    internal class ArrayContainer<T>
    {
        [ThriftSMember(1)]
        public List<T> Value { get; set; }

        public T[] TValue
        {
            get
            {
                return Value.ToArray<T>();
            }
        }
        public ArrayContainer()
        {
        }

        public ArrayContainer(T[] value)
        {
            List<T> lst = new List<T>();
            foreach (T item in value)
            {
                lst.Add(item);
            }
            Value = lst;
        }
    }
}
