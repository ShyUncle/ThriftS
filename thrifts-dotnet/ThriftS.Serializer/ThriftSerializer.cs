using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common;
using Thrift.Protocol;
using Thrift.Transport;

namespace ThriftS.Serializer
{
    public static class ThriftSerializer
    {
        public static byte[] Serialize(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            var type = instance.GetType();

            if (type.IsValueType
                || type == typeof(string)
                || type == typeof(byte[]))
            {
                var genericListType = typeof(ValueContainer<>).MakeGenericType(type);
                instance = Activator.CreateInstance(genericListType, instance);
            }
            else if (type.IsList())
            {
                var genericType = type.GetGenericArguments()[0];
                var genericListType = typeof(ListContainer<>).MakeGenericType(genericType);

                instance = Activator.CreateInstance(genericListType, instance);
            }
            else if (type.IsMap())
            {
                var genericTypes = type.GetGenericArguments();
                var genericTypeKey = genericTypes[0];
                var genericTypeValue = genericTypes[1];
                var genericMapType = typeof(MapContainer<,>).MakeGenericType(genericTypeKey, genericTypeValue);

                instance = Activator.CreateInstance(genericMapType, instance);
            }

            using (var stream = new MemoryStream())
            using (var transport = new TStreamTransport(null, stream))
            using (var protocol = new TBinaryProtocol(transport))
            {
                protocol.WriteStruct(new TStruct(), instance);
                protocol.Transport.Flush();

                return stream.ToArray();
            }
        }

        public static object Deserialize(Type type, byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return null;
            }

            var isContainer = false;
            if (type.IsValueType
                || type == typeof (string)
                || type == typeof (byte[]))
            {
                type = typeof(ValueContainer<>).MakeGenericType(type);
                isContainer = true;
            }
            else if (type.IsList())
            {
                var genericArg = type.GetGenericArguments()[0];
                type = typeof(ListContainer<>).MakeGenericType(genericArg);
                isContainer = true;
            }
            else if (type.IsMap())
            {
                var genericTypes = type.GetGenericArguments();
                var genericTypeKey = genericTypes[0];
                var genericTypeValue = genericTypes[1];

                type = typeof(MapContainer<,>).MakeGenericType(genericTypeKey, genericTypeValue);
                isContainer = true;
            }

            using (var stream = new MemoryStream(buffer))
            using (var transport = new TStreamTransport(stream, null))
            using (var protocol = new TBinaryProtocol(transport))
            {
                var result = protocol.ReadStruct(type);

                if(isContainer)
                {
                    return type.GetProperty("Value").GetValue(result);
                }
                return result;
            }
        }
    }
}
