using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using ThriftS.Common;
using ThriftS.Common.Attributes;

namespace ThriftS.Serializer
{
    internal static class TProtocolExtension
    {
        public static void WriteStruct(this TProtocol protocol, TStruct @struct, object instance)
        {
            protocol.WriteStructBegin(@struct);

            var type = instance.GetType();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                short fieldId = 0;
                var attr = prop.GetCustomAttributes(typeof(ThriftSMemberAttribute), false).FirstOrDefault();
                if (attr != null)
                {
                    fieldId = (attr as ThriftSMemberAttribute).Tag;
                    protocol.WriteField(fieldId, prop.Name, prop.PropertyType, prop.GetValue(instance));
                }
            }

            protocol.WriteFieldStop();
            protocol.WriteStructEnd();
        }

        private static void WriteField(this TProtocol protocol, short fieldId, string fieldName, Type sourceType,
            object value)
        {
            if (value == null) return;

            var field = new TField();
            field.ID = fieldId;
            field.Name = fieldName;
            field.Type = sourceType.ToThriftType();

            protocol.WriteFieldBegin(field);
            protocol.WriteFieldValue(sourceType, field.Type, value);
            protocol.WriteFieldEnd();
        }

        private static void WriteFieldValue(this TProtocol protocol, Type sourceType, TType thriftType, object value)
        {
            switch (thriftType)
            {
                case TType.Bool:
                    protocol.WriteBool(Convert.ToBoolean(value));
                    break;
                case TType.Byte:
                    protocol.WriteByte(Convert.ToSByte(value));
                    break;
                case TType.I16:
                    protocol.WriteI16(Convert.ToInt16(value));
                    break;
                case TType.I32:
                    protocol.WriteI32(Convert.ToInt32(value));
                    break;
                case TType.I64:
                    if (sourceType == typeof(DateTime)|| sourceType == typeof(DateTime?))
                    {
                        protocol.WriteI64(DateTimeHelper.ToUnixTimestamp(Convert.ToDateTime(value)));
                    }
                    else
                    {
                        protocol.WriteI64(Convert.ToInt64(value));
                    }

                    break;
                case TType.Double:
                    protocol.WriteDouble(Convert.ToDouble(value));
                    break;
                case TType.String:
                    if (sourceType == typeof(string)
                        || sourceType == typeof(Guid))
                    {
                        protocol.WriteString(Convert.ToString(value));
                    }
                    else if (sourceType == typeof(byte[]))
                    {
                        protocol.WriteBinary((byte[])value);
                    }
                    break;
                case TType.Struct:
                    protocol.WriteStruct(new TStruct(), value);
                    break;
                case TType.List:
                    var list = value as IList;
                    protocol.WriteListBegin(new TList(TType.Struct, list.Count));
                    foreach (var item in list)
                    {
                        //排除后和count不一致
                        if (item == null)
                        {
                            throw new ThriftSException("The item must be not null.");
                        }

                        protocol.WriteFieldValue(item.GetType(), item.GetType().ToThriftType(), item);
                    }
                    protocol.WriteListEnd();
                    break;
                case TType.Map:
                    var map = value as IDictionary;
                    var genericArgs = value.GetType().GetGenericArguments();
                    protocol.WriteMapBegin(new TMap(genericArgs[0].ToThriftType(), genericArgs[1].ToThriftType(), map.Count));
                    foreach (var key in map.Keys)
                    {
                        //排除后和count不一致
                        if (key == null || map[key] == null)
                        {
                            throw new ThriftSException("The key or value must be not null.");
                        }
                        protocol.WriteFieldValue(key.GetType(), key.GetType().ToThriftType(), key);
                        protocol.WriteFieldValue(map[key].GetType(), map[key].GetType().ToThriftType(), map[key]);
                    }
                    protocol.WriteMapEnd();
                    break;
                default:
                    break;
            }
        }

        public static object ReadStruct(this TProtocol protocol, Type targetType)
        {
            object result = null;
            var dicProps = new Dictionary<int, PropertyInfo>();

            result = Activator.CreateInstance(targetType);

            if (targetType.IsClass)
            {
                var props = targetType.GetProperties();
                foreach (var prop in props)
                {
                    var attrs = prop.GetCustomAttributes(typeof(ThriftSMemberAttribute), false);
                    if (attrs.Any())
                    {
                        dicProps.Add((attrs[0] as ThriftSMemberAttribute).Tag, prop);
                    }
                }
            }

            var @struct = protocol.ReadStructBegin();

            while (true)
            {
                var field = protocol.ReadFieldBegin();
                if (field.Type == TType.Stop)
                {
                    break;
                }

                // 跳过不存在的字段
                if (dicProps.ContainsKey(field.ID))
                {
                    var fieldValue = protocol.ReadFieldValue(field.Type, dicProps[field.ID].PropertyType);

                    if (dicProps[field.ID].PropertyType == typeof(Guid))
                    {
                        dicProps[field.ID].SetValue(result, new Guid(fieldValue.ToString()));
                    }
                    else if (dicProps[field.ID].PropertyType == typeof(DateTime)||dicProps[field.ID].PropertyType == typeof(DateTime?))
                    {
                        dicProps[field.ID].SetValue(result, DateTimeHelper.ToDateTime((long)fieldValue));
                    }
                    else if (dicProps[field.ID].PropertyType.IsEnum)
                    {
                        var enumValue = Enum.Parse(dicProps[field.ID].PropertyType, fieldValue.ToString());
                        dicProps[field.ID].SetValue(result, enumValue);
                    }
                    else if (dicProps[field.ID].PropertyType.IsValueType
                             && dicProps[field.ID].PropertyType != fieldValue.GetType())
                    {
                        // ex: double --> decimal
                        var newFieldValue = Convert.ChangeType(fieldValue, dicProps[field.ID].PropertyType);
                        dicProps[field.ID].SetValue(result, newFieldValue);
                    }
                    else
                    {
                        dicProps[field.ID].SetValue(result, fieldValue);
                    }
                }

                protocol.ReadFieldEnd();
            }

            protocol.ReadStructEnd();
            return result;
        }

        private static object ReadFieldValue(this TProtocol protocol, TType thriftType, Type targetType)
        {
            switch (thriftType)
            {
                //case TType.Void:
                //    return null;
                case TType.Bool:
                    return protocol.ReadBool();
                case TType.Byte:
                    return protocol.ReadByte();
                case TType.I16:
                    return protocol.ReadI16();
                case TType.I32:
                    return protocol.ReadI32();
                case TType.I64:
                    return protocol.ReadI64();
                case TType.Double:
                    return protocol.ReadDouble();
                case TType.String:
                    if (targetType == typeof(string)
                        || targetType == typeof(Guid))
                    {
                        return protocol.ReadString();
                    }
                    else
                    {
                        return protocol.ReadBinary();
                    }
                case TType.Struct:
                    return protocol.ReadStruct(targetType);
                case TType.List:
                    var result = Activator.CreateInstance(targetType);
                    var list = result as IList;

                    // 只考虑泛型list
                    if (targetType.IsGenericType)
                    {
                        var genericType = targetType.GetGenericArguments()[0];
                        var tlist = protocol.ReadListBegin();
                        for (int i = 0; i < tlist.Count; i++)
                        {
                            var item = protocol.ReadFieldValue(genericType.ToThriftType(), genericType);
                            list.Add(item);
                        }
                        protocol.ReadListEnd();
                    }

                    return result;

                case TType.Map:
                    var resultMap = Activator.CreateInstance(targetType);
                    var dic = resultMap as IDictionary;

                    // 只考虑泛型map
                    if (targetType.IsGenericType)
                    {
                        var genericTypes = targetType.GetGenericArguments();
                        var genericTypeKey = genericTypes[0];
                        var genericTypeValue = genericTypes[1];

                        var tmap = protocol.ReadMapBegin();
                        for (int i = 0; i < tmap.Count; i++)
                        {
                            var key = protocol.ReadFieldValue(genericTypeKey.ToThriftType(), genericTypeKey);
                            var value = protocol.ReadFieldValue(genericTypeValue.ToThriftType(), genericTypeValue);
                            dic.Add(key, value);
                        }
                        protocol.ReadMapEnd();
                    }

                    return resultMap;
                default:
                    TProtocolUtil.Skip(protocol, thriftType);
                    return null;
            }
        }
    }
}
