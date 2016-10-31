package org.zeeman.thrifts.common;

import org.apache.commons.lang3.StringUtils;
import org.zeeman.thrifts.common.annotations.ThriftSContract;

import java.lang.management.ManagementFactory;
import java.lang.management.RuntimeMXBean;
import java.net.InetAddress;
import java.net.UnknownHostException;

public final class Utils {
    private static String version;

    static {
        version = "0.6";
    }

    public static String getVersion() {
        return version;
    }

    public static String getServiceName(Class<? extends Object> contractType) {
        String serviceName = StringUtils.EMPTY;

        ThriftSContract contractAnnotation = contractType.getAnnotation(ThriftSContract.class);
        if (contractAnnotation != null) {
            serviceName = contractAnnotation.serviceName();
        }

        if (serviceName == null || serviceName.isEmpty()) {
            serviceName = contractType.getName();
        }

        return serviceName;
    }

    public static SerializerMode getSerializerMode(Class<? extends Object> targetType) {
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

    public static Integer getPid() {
        //获取进程的PID
        RuntimeMXBean runtime = ManagementFactory.getRuntimeMXBean();
        String name = runtime.getName(); // format: "pid@hostname"
        try {
            return Integer.parseInt(name.substring(0, name.indexOf('@')));
        } catch (Exception e) {
            return -1;
        }
    }

    public static String getHostName() {
        try {
            return InetAddress.getLocalHost().getHostName();
        } catch (UnknownHostException e) {
            return StringUtils.EMPTY;
        }
    }
}
