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
        //ֵ���ͺ��ַ���ʹ��protobuf���л�
        if(targetType.isPrimitive() || targetType == String.class)
        {
            return SerializerMode.ProtoBuf;
        }

        //ȡ���Ͳ���
//        targetType.getge
//        if(targetType is ParameterizedType)
//        if(((ParameterizedType)targetType.getGenericType .getGenericSuperclass()).getActualTypeArguments()[0])

        if(targetType.isAnnotationPresent(ThriftSModel.class)) {
            ThriftSModel thriftSModel = targetType.getAnnotation(ThriftSModel.class);
            if (thriftSModel.SerializerMode() != SerializerMode.Auto) {
                return thriftSModel.SerializerMode();
            }
        }

        //����JProto���л�����Ҫ��class����Annotation����˲�����C#��������Attribute�ƶ�
        //if(targetType.isAnnotationPresent(Protobuf.class)){
        //    return SerializerMode.ProtoBuf;
        //}

        return SerializerMode.Json;*/
    }
}
