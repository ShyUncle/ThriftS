package org.zeeman.thrifts.serializer;

import com.fasterxml.classmate.ResolvedType;
import org.apache.thrift.protocol.TType;

import java.util.Date;
import java.util.List;
import java.util.Map;
import java.util.UUID;

final class TypeExtension {
    public static boolean isList(Class<?> type) {
        return List.class.isAssignableFrom(type);
    }

    public static boolean isMap(Class<?> type) {
        return Map.class.isAssignableFrom(type);//type.isAssignableFrom(Map.class);
    }

    public static byte toThriftType(ResolvedType type) {//(Class<?> sourceType) {
        // list也是class, 优先检测list
        //if (sourceType == null)
        //{
        //    return TType.Void;
        //}else

        if(type.getTypeParameters().size()>0) {
            if (isList(type.getErasedType())) {
                return TType.LIST;
            } else {
                return TType.STRUCT;
            }
        }

        Class<?> sourceType = type.getErasedType();
        if (isList(sourceType)) {
            return TType.LIST;
        } else if (isMap(sourceType)) {
            return TType.MAP;
        } else if (sourceType == byte[].class) {
            return TType.STRING;
        } else if (sourceType.isEnum()) {
            return TType.I32;
        } else if (sourceType == boolean.class
                || sourceType == Boolean.class) {
            return TType.BOOL;
        } else if (sourceType == byte.class
                || sourceType == Byte.class) {
            return TType.BYTE;
        } else if (sourceType == short.class
                || sourceType == Short.class) {
            return TType.I16;
        } else if (sourceType == int.class
                || sourceType == Integer.class) {
            return TType.I32;
        } else if (sourceType == long.class
                || sourceType == Long.class
                || sourceType == Date.class) {
            return TType.I64;
        } else if (sourceType == double.class
                || sourceType == Double.class
                || sourceType == float.class
                || sourceType == Float.class) {
            return TType.DOUBLE;
        } else if (sourceType == String.class
                || sourceType == UUID.class) {
            return TType.STRING;
        } else if (sourceType instanceof Class
                && sourceType != String.class) {
            return TType.STRUCT;
        } else {
            throw new UnsupportedOperationException(String.format("Not supported type:%s.", sourceType));
        }
    }
}
