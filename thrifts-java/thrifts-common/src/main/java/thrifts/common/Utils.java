package thrifts.common;

import thrifts.common.annotations.ThriftSModel;

public final class Utils {
    private static String version;

    static {
        version = "0.6";
    }

    public static String getVersion()
    {
        return version;
    }

    public static SerializerMode GetSerializerMode(Class<? extends Object> targetType) {
        return SerializerMode.Thrift;

        /*
        //值类型和字符串使用protobuf序列化
        if(targetType.isPrimitive() || targetType == String.class)
        {
            return SerializerMode.ProtoBuf;
        }

        //取泛型参数
//        targetType.getge
//        if(targetType is ParameterizedType)
//        if(((ParameterizedType)targetType.getGenericType .getGenericSuperclass()).getActualTypeArguments()[0])

        if(targetType.isAnnotationPresent(ThriftSModel.class)) {
            ThriftSModel thriftSModel = targetType.getAnnotation(ThriftSModel.class);
            if (thriftSModel.SerializerMode() != SerializerMode.Auto) {
                return thriftSModel.SerializerMode();
            }
        }

        //由于JProto序列化不需要在class附加Annotation，因此不能像C#那样依据Attribute推断
        //if(targetType.isAnnotationPresent(Protobuf.class)){
        //    return SerializerMode.ProtoBuf;
        //}

        return SerializerMode.Json;*/
    }
}
