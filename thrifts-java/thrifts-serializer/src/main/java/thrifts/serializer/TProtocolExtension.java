package thrifts.serializer;

import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
import org.apache.thrift.TException;
import org.apache.thrift.protocol.*;
import thrifts.common.ThriftSException;
import thrifts.common.annotations.ThriftSMember;

import java.lang.reflect.Field;
import java.lang.reflect.Type;
import java.nio.ByteBuffer;
import java.util.*;

final class TProtocolExtension {
    static final TypeResolver typeResolver = new TypeResolver();

    public static void writeStruct(TProtocol protocol, TStruct struct, Object instance) throws TException, IllegalAccessException, ThriftSException {
        protocol.writeStructBegin(struct);

        Class<?> type = instance.getClass();
        Field[] props = type.getDeclaredFields();
        for (Field prop : props) {
            short fieldId = 0;
            ThriftSMember attr = prop.getAnnotation(ThriftSMember.class);
            if (attr != null) {
                fieldId = attr.tag();
                prop.setAccessible(true);
                writeField(protocol, fieldId, prop.getName(), prop.get(instance));
            }
        }

        protocol.writeFieldStop();
        protocol.writeStructEnd();
    }

    private static void writeField(TProtocol protocol, short fieldId, String fieldName, Object value) throws TException, ThriftSException, IllegalAccessException {
        if (value == null) return;

        //java类型擦除
        Class<?> sourceType = value.getClass();

        byte fieldType = TypeExtension.toThriftType(typeResolver.resolve(sourceType));
        TField field = new TField(fieldName, fieldType, fieldId);


        protocol.writeFieldBegin(field);
        writeFieldValue(protocol, sourceType, field.type, value);
        protocol.writeFieldEnd();
    }

    private static void writeFieldValue(TProtocol protocol, Class<?> sourceType, byte thriftType, Object value) throws TException, IllegalAccessException, ThriftSException {
        switch (thriftType) {
            case TType.BOOL:
                protocol.writeBool(((Boolean) value).booleanValue());
                break;
            case TType.BYTE:
                protocol.writeByte(((Byte) value).byteValue());
                break;
            case TType.I16:
                protocol.writeI16(((Short) value).shortValue());
                break;
            case TType.I32:
                protocol.writeI32(((Integer) value).intValue());
                break;
            case TType.I64:
                if (sourceType == Date.class) {
//                    protocol.writeI64(DateTimeHelper.ToUnixTimestamp(Convert.ToDateTime(value)));
                } else {
                    protocol.writeI64(((Long) (value)).longValue());
                }

                break;
            case TType.DOUBLE:
                protocol.writeDouble(((Double) (value)).doubleValue());
                break;
            case TType.STRING:
                if (sourceType == String.class
                        || sourceType == UUID.class) {
                    protocol.writeString(value.toString());
                } else if (sourceType == byte[].class) {
                    protocol.writeBinary(ByteBuffer.wrap((byte[]) value));
                }
                break;
            case TType.STRUCT:
                writeStruct(protocol, new TStruct(), value);
                break;
            case TType.LIST:
                List<?> list = (List<?>) value;

                byte itemType = TType.STRUCT;
                if(list.size()>0)
                {
                    itemType = TypeExtension.toThriftType(typeResolver.resolve(list.get(0).getClass()));
                }
                protocol.writeListBegin(new TList(TType.STRUCT, list.size()));
                for (Object item : list) {
                    //排除后和count不一致
                    if (item == null) {
                        throw new ThriftSException("The item must be not null.");
                    }

                    writeFieldValue(protocol, item.getClass(), itemType, item);
                }
                protocol.writeListEnd();
                break;
            case TType.MAP:
                Map<?, ?> map = (Map<?, ?>) value;
//                Type[] genericArgs = ((ParameterizedType) value.getClass().getGenericSuperclass()).getActualTypeArguments();
                byte keyType = TType.STRUCT;
                byte valueType = TType.STRUCT;
                if(map.size()>0)
                {
                    keyType = TypeExtension.toThriftType(typeResolver.resolve(map.keySet().toArray()[0].getClass()));
                    valueType = TypeExtension.toThriftType(typeResolver.resolve(map.values().toArray()[0].getClass()));
                }

                protocol.writeMapBegin(new TMap(
                        keyType,
                        valueType,
                        map.size()));
//                protocol.writeMapBegin(new TMap(
//                        TypeExtension.toThriftType((Class<?>) genericArgs[0]),
//                        TypeExtension.toThriftType((Class<?>) genericArgs[1]),
//                        map.size()));
                for (Object key : map.keySet()) {
                    //排除后和count不一致
                    if (key == null || map.get(key) == null) {
                        throw new ThriftSException("The key or value must be not null.");
                    }
                    writeFieldValue(protocol,
                            key.getClass(),
                            TypeExtension.toThriftType(typeResolver.resolve(key.getClass())),
                            key);

                    writeFieldValue(protocol,
                            map.get(key).getClass(),
                            TypeExtension.toThriftType(typeResolver.resolve(map.get(key).getClass())),
                            map.get(key));
                }
                protocol.writeMapEnd();
                break;
            default:
                break;
        }
    }

    public static Object readStruct(TProtocol protocol, ResolvedType type) throws IllegalAccessException, InstantiationException, TException {
        Object result = null;
        HashMap<Short, Field> dicProps = new HashMap<Short, Field>();

//        Class<?> targetType = null;
//        if(type instanceof ParameterizedType) {
//            targetType = (Class<?>) ((ParameterizedType) type).getRawType();
//        }
//        else {
//            targetType = (Class<?>) type;
//        }

        Class<?> targetType = type.getErasedType();

        result = targetType.newInstance();

        if (targetType.isPrimitive() == false)
        {
            Field[] props = targetType.getDeclaredFields();
            for(Field prop : props)
            {
                ThriftSMember attr = prop.getAnnotation(ThriftSMember.class);
                if(attr!=null){
                    dicProps.put(attr.tag(), prop);
                }
            }
        }

        TStruct struct = protocol.readStructBegin();

        while (true)
        {
            TField field = protocol.readFieldBegin();
            if (field.type == TType.STOP)
            {
                break;
            }

            // 跳过不存在的字段
            if (dicProps.containsKey(field.id))
            {
                Object fieldValue = null;

                Field prop = dicProps.get(field.id);
                prop.setAccessible(true);

                //fieldValue = readFieldValue(protocol, field.type, prop.getGenericType());

                List<ResolvedType> typeParam = type.getTypeParameters();


                if(result instanceof ValueContainer)
                {
                    ResolvedType tt = typeResolver.resolve(typeParam.get(0).getErasedType());
                    fieldValue = readFieldValue(protocol, field.type, tt);
                }
                else if(result instanceof ListContainer)
                {
                    ResolvedType tt = typeResolver.resolve(ArrayList.class,typeParam.get(0).getErasedType());
                    fieldValue = readFieldValue(protocol, field.type, tt);
                }
                else if(result instanceof MapContainer)
                {
                    ResolvedType tt = typeResolver.resolve(HashMap.class,typeParam.get(0),typeParam.get(1));
                    fieldValue = readFieldValue(protocol, field.type, tt);
                }
                else
                {
                    fieldValue = readFieldValue(protocol, field.type, typeResolver.resolve(prop.getGenericType()));
                }

                if (prop.getDeclaringClass() == UUID.class)
                {
                    prop.set(result, UUID.fromString(fieldValue.toString()));
                }
                else if (prop.getDeclaringClass() == Date.class)
                {
//                    prop.set(result, DateTimeHelper.ToDateTime((long) fieldValue));
                }
                else if (prop.getDeclaringClass().isEnum())
                {
                    Object enumValue = Enum.valueOf((Class<Enum>) prop.getDeclaringClass(), fieldValue.toString());
                    prop.set(result, enumValue);
                }
                else if (prop.getDeclaringClass().isPrimitive()
                        && prop.getDeclaringClass() != fieldValue.getClass())
                {
                    // ex: double --> decimal
//                    var newFieldValue = Convert.ChangeType(fieldValue, dicProps[field.ID].PropertyType);
//                    dicProps.get(field.id).set(result, newFieldValue);
                }
                else
                {
                    prop.set(result, fieldValue);
                }
            }

            protocol.readFieldEnd();
        }

        protocol.readStructEnd();
        return result;
    }

    private static Object readFieldValue(TProtocol protocol, byte thriftType, ResolvedType type) throws IllegalAccessException, TException, InstantiationException {
        Class<?> targetType = null;
        Type[] genericArguments = null;
//        TypeResolver typeResolver = new TypeResolver();
//        if(type instanceof ParameterizedType)
//        {
//            targetType = (Class<?>)((ParameterizedType)type).getRawType();
//            genericArguments = ((ParameterizedType)type).getActualTypeArguments();
//        }
//        else
//        {
//            targetType = (Class<?>)type.getErasedType();
//        }
        targetType = (Class<?>)type.getErasedType();

        switch (thriftType)
        {
            //case TType.Void:
            //    return null;
            case TType.BOOL:
                return protocol.readBool();
            case TType.BYTE:
                return protocol.readByte();
            case TType.I16:
                return protocol.readI16();
            case TType.I32:
                return protocol.readI32();
            case TType.I64:
                return protocol.readI64();
            case TType.DOUBLE:
                return protocol.readDouble();
            case TType.STRING:
                if (targetType == String.class
                        || targetType == UUID.class)
                {
                    return protocol.readString();
                }
                else
                {
                    return protocol.readBinary();
                }
            case TType.STRUCT:
                return readStruct(protocol, type);
            case TType.LIST:
                Object result = targetType.newInstance();
                ArrayList list = (ArrayList)result;

                // 只考虑泛型list
//                if (targetType.IsGenericType)
//                {
//                    Class<?> genericType = (Class<?>)((ParameterizedType) targetType.getGenericSuperclass()).getActualTypeArguments()[0];
                    //Class<?> genericType = (Class<?>)genericArguments[0];
                ResolvedType genericType = type.getTypeParameters().get(0);
                    TList tlist = protocol.readListBegin();
                    for (int i = 0; i < tlist.size; i++)
                    {
                        Object item = readFieldValue(protocol, TypeExtension.toThriftType(genericType), genericType);
                        list.add(item);
                    }
                    protocol.readListEnd();
//                }

                return result;

            case TType.MAP:
                Object resultMap = targetType.newInstance();
                //Map<?,?> dic = (Map<?,?>)resultMap;
                Map<Object,Object> dic = (Map<Object,Object>)resultMap;

                // 只考虑泛型map
//                if (targetType.IsGenericType)
//                {
//                    Type[] genericTypes = ((ParameterizedType) targetType.getGenericSuperclass()).getActualTypeArguments();
//                    Class<?> genericTypeKey = (Class<?>)genericTypes[0];
//                    Class<?> genericTypeValue = (Class<?>)genericTypes[1];

                    Class<?> genericTypeKey = type.getTypeParameters().get(0).getErasedType(); //(Class<?>)genericArguments[0];

                ResolvedType valType = null;
                    if(type.getTypeParameters().get(1).getTypeParameters().size()>0)
                    {
                        valType = type.getTypeParameters().get(1);
                    }
                    else
                    {
                        Class<?> genericTypeValue = type.getTypeParameters().get(1).getErasedType();;
                        valType = typeResolver.resolve(genericTypeValue);
                    }

//                    Type[] genericArgumentsNext = null;
//                    if(genericArguments[1] instanceof ParameterizedType)
//                    {
//                        ParameterizedType genericTypeValueP = (ParameterizedType)genericArguments[1];
//                        genericTypeValue = (Class<?>)genericTypeValueP.getRawType();
//                        genericArgumentsNext = genericTypeValueP.getActualTypeArguments();
//                    }
//                    else
//                    {
//                        genericTypeValue = (Class<?>)genericArguments[1];
//                    }


                    TMap tmap = protocol.readMapBegin();
                    for (int i = 0; i < tmap.size; i++)
                    {
                        Object key = readFieldValue(protocol, TypeExtension.toThriftType(typeResolver.resolve(genericTypeKey)), typeResolver.resolve(genericTypeKey));
                        Object value = readFieldValue(protocol, TypeExtension.toThriftType(valType),valType );
                        dic.put(key, value);
                    }
                    protocol.readMapEnd();
//                }

                return resultMap;
            default:
                TProtocolUtil.skip(protocol, thriftType);
                return null;
        }
    }
}
