using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;

namespace ThriftS.Serializer
{
    internal static class TypeExtension
    {
        public static bool IsList(this Type type)
        {
            return type == typeof(IList)
                   || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static bool IsMap(this Type type)
        {
            return type == typeof(IDictionary)
                   || (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)));
        }

        public static TType ToThriftType(this Type sourceType)
        {
            // list也是class, 优先检测list
            //if (sourceType == null)
            //{
            //    return TType.Void;
            //}else 
            if (sourceType.IsList())
            {
                return TType.List;
            }
            else if (sourceType.IsMap())
            {
                return TType.Map;
            }
            else if (sourceType == typeof(byte[]))
            {
                return TType.String;
            }
            else if (sourceType.IsEnum)
            {
                return TType.I32;
            }
            else if (sourceType.IsClass
                     && sourceType != typeof(string))
            {
                return TType.Struct;
            }
            else if (sourceType == typeof(bool))
            {
                return TType.Bool;
            }
            else if (sourceType == typeof(sbyte))
            {
                return TType.Byte;
            }
            else if (sourceType == typeof(short)
                     || sourceType == typeof(byte))
            {
                return TType.I16;
            }
            else if (sourceType == typeof(int))
            {
                return TType.I32;
            }
            else if (sourceType == typeof(long)
                || sourceType == typeof(DateTime)
                || sourceType == typeof(DateTime?))
            {
                return TType.I64;
            }
            else if (sourceType == typeof(double)
                     || sourceType == typeof(decimal)
                     || sourceType == typeof(float))
            {
                return TType.Double;
            }
            else if (sourceType == typeof(string)
                     || sourceType == typeof(Guid))
            {
                return TType.String;
            }
            else
            {
                throw new NotSupportedException(string.Format("Not supported type:{0}.", sourceType));
            }
        }
    }
}
