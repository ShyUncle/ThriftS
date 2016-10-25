package org.zeeman.thrifts.serializer;

import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
import org.apache.thrift.TException;
import org.apache.thrift.protocol.TBinaryProtocol;
import org.apache.thrift.protocol.TStruct;
import org.apache.thrift.transport.TIOStreamTransport;
import org.zeeman.thrifts.common.ThriftSException;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.lang.reflect.*;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public final class ThriftSerializer {
    public static byte[] Serialize(Object instance) throws ThriftSException {
        if (instance == null) {
            return null;
        }

        Class<?> type = instance.getClass();

        try {
            if (instance instanceof Integer
                    || type == String.class
                    || type == byte[].class) {
                Constructor con = ValueContainer.class.getConstructor(new Class[]{Object.class});
                instance = con.newInstance(new Object[]{instance});
            } else if (TypeExtension.isList(type)) {
                Constructor con = ListContainer.class.getConstructor(new Class[]{ArrayList.class});
                instance = con.newInstance(new Object[]{instance});
            } else if (TypeExtension.isMap(type)) {
                Constructor con = MapContainer.class.getConstructor(new Class[]{HashMap.class});
                instance = con.newInstance(new Object[]{instance});
            }
        } catch (NoSuchMethodException e) {
            throw new ThriftSException("Serialize error", e);
        } catch (IllegalAccessException e) {
            throw new ThriftSException("Serialize error", e);
        } catch (InstantiationException e) {
            throw new ThriftSException("Serialize error", e);
        } catch (InvocationTargetException e) {
            throw new ThriftSException("Serialize error", e);
        }

        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        try {
            TIOStreamTransport transport = new TIOStreamTransport(null, stream);
            TBinaryProtocol protocol = new TBinaryProtocol(transport);
            try {
                TProtocolExtension.writeStruct(protocol, new TStruct(), instance);
                protocol.getTransport().flush();
            } catch (TException e) {
                throw new ThriftSException("Serialize error", e);
            } catch (IllegalAccessException e) {
                throw new ThriftSException("Serialize error", e);
            }
        } finally {
            try {
                stream.close();
            } catch (IOException e) {
                throw new ThriftSException("Serialize error", e);
            }
        }

        return stream.toByteArray();
    }

    //, Class<?> type, Type[] genericArguments
    public static Object Deserialize(ResolvedType type, byte[] buffer) throws ThriftSException {
        if (buffer == null || buffer.length == 0) {
            return null;
        }

        TypeResolver typeResolver = new TypeResolver();
//        ResolvedType t = typeResolver.resolve(String.class);
//        Class t1 = t.getErasedType();
//        ResolvedType listType = typeResolver.resolve(ArrayList.class, String.class);
//        Class t2 = listType.getErasedType();
//        listType.getTypeParameters()

//        if(type instanceof ParameterizedType){
//            val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, ((ParameterizedType)methodParameterTypesP[i]).getActualTypeArguments());
//        }

//        Class containerType = type;
        boolean isContainer = false;
        Class<?> rawType = type.getErasedType();
//        if((rawType instanceof ParameterizedType) == false) {
            if (rawType.isPrimitive()
                    || rawType == String.class
                    || rawType == byte[].class) {
                type = typeResolver.resolve(ValueContainer.class,rawType);
                isContainer = true;
            }
//        }
//        else {
            //Type rawType = ((ParameterizedType)type).getRawType();
            else if (TypeExtension.isList(rawType)) {
                //type = ListContainer.class;
                List<ResolvedType> tps = type.getTypeParameters();
                type = typeResolver.resolve(ListContainer.class, tps.get(0));
                isContainer = true;
            } else if (TypeExtension.isMap((Class<?>) rawType)) {
                List<ResolvedType> tps = type.getTypeParameters();
                type = typeResolver.resolve(MapContainer.class, tps.get(0), tps.get(1));
                //type = new TypeToken<MapContainer<Integer,String>>(){}.getType();
                //type =  MapContainer.class;
                isContainer = true;
            }
//        }

        ByteArrayInputStream stream = new ByteArrayInputStream(buffer);
        Object result = null;
        try {
            TIOStreamTransport transport = new TIOStreamTransport(stream, null);
            TBinaryProtocol protocol = new TBinaryProtocol(transport);
            result = TProtocolExtension.readStruct(protocol, type);//, genericArguments
        } catch (IllegalAccessException e) {
            throw new ThriftSException("Deserialize error", e);
        } catch (TException e) {
            throw new ThriftSException("Deserialize error", e);
        } catch (InstantiationException e) {
            throw new ThriftSException("Deserialize error", e);
        } finally {
            try {
                stream.close();
            } catch (IOException e) {
                throw new ThriftSException("Deserialize error", e);
            }
        }

        if (isContainer) {
            Field prop = null;
            try {
                prop = ((Class<?>)type.getErasedType()).getDeclaredField("value");
            } catch (NoSuchFieldException e) {
                throw new ThriftSException("Deserialize error", e);
            }

            prop.setAccessible(true);
            try {
                return prop.get(result);
                //return rawType.cast(prop.get(result));
            } catch (IllegalAccessException e) {
                throw new ThriftSException("Deserialize error", e);
            }
        }

        return result;
    }
}
