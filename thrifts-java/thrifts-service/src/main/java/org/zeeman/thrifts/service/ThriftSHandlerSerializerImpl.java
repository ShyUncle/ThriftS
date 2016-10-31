package org.zeeman.thrifts.service;

import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
import org.apache.commons.lang3.StringUtils;
import org.zeeman.thrifts.common.ContentTypes;
import org.zeeman.thrifts.common.ThriftSException;
import org.zeeman.thrifts.serializer.ThriftSerializer;
//import com.google.gson.Gson;
//import com.google.gson.GsonBuilder;
//import com.baidu.bjf.remoting.protobuf.Codec;
//import com.baidu.bjf.remoting.protobuf.ProtobufProxy;

import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by Nina on 2016/10/30.
 */
public class ThriftSHandlerSerializerImpl implements ThriftSHandlerSerializer {
    //private static Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss").create();

    public byte[] Serialize(String contentType, Object instance) throws ThriftSException {

        if (contentType.equalsIgnoreCase(ContentTypes.Protobuf)) {
/*//                        LinkedBuffer buffer = LinkedBuffer.allocate(LinkedBuffer.DEFAULT_BUFFER_SIZE);
//                        //RuntimeSchema.getSchema(ArrayList.class,String.class);
//                        //CollectionSchema<String>.
//                        Schema schema = RuntimeSchema.createFrom(resultType);//(result.getClass(), String.class);
//                        byte[] bytes =  ProtobufIOUtil.toByteArray(result, schema, buffer);
//                        xResult.setData(bytes);
                        //Protobuf序列化
                        Class<? extends Object> T = result.getClass();
                        Codec protoCodec = ProtobufProxy.create(T);
//                        Codec protoCodec = ProtobufProxy.create(resultType);
                        xResult.setContentType(ContentTypes.Protobuf);
                        try {
                            xResult.setData(protoCodec.encode(result));
                        } catch (IOException e) {
                            e.printStackTrace();
                        }*/
            return null;
        } else if (contentType.equalsIgnoreCase(ContentTypes.Thrift)) {
            return ThriftSerializer.Serialize(instance);
        }/* else {
                        //Json序列化
                        xResult.setContentType(ContentTypes.Json);
                        xResult.setData(gson.toJson(result).getBytes());
                    }*/
        return null;
    }

    public Object Deserialize(String contentType, Type type, byte[] buffer) throws ThriftSException {
        //                        if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Thrift)) {
//                            if(parameterType == String.class) {
//                                invokeParameters.put(parameterType.getName(), requestParameter.getValue());
//                            }
//                        }else
                         /*if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Protobuf)){
                            // 简单类型用jproto有问题
                            Codec protoCodec = ProtobufProxy.create(parameterType);
                            invokeParameters.put(parameterType.getName(),protoCodec.decode(requestParameter.getValue()));
                        }else*/
                        /*if (requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Json)) {
                            ByteArrayInputStream inputStream = new ByteArrayInputStream(requestParameter.getValue());
                            try {
                                InputStreamReader reader = new InputStreamReader(inputStream);
                                invokeParameters.put(parameterType.getName(), gson.fromJson(reader, parameterType));
                            } finally {
                                inputStream.close();
                            }
                        } else */

        if (contentType.equalsIgnoreCase(ContentTypes.Thrift)) {

            TypeResolver typeResolver = new TypeResolver();
            ResolvedType ptype = null;

            if (type instanceof ParameterizedType) {
                ParameterizedType typeTemp = (ParameterizedType) type;
                if (typeTemp.getRawType() == ArrayList.class) {
                    ptype = typeResolver.resolve(ArrayList.class, typeTemp.getActualTypeArguments());
                } else if (typeTemp.getRawType() == HashMap.class) {
                    ptype = typeResolver.resolve(HashMap.class, typeTemp.getActualTypeArguments());
                }
                //parameterType =typeResolver.resolve(((ParameterizedType) methodParameterTypesP[i]).getRawType(), ((ParameterizedType) methodParameterTypesP[i]).getActualTypeArguments());
            } else {
                ptype = typeResolver.resolve(type);
            }

           return ThriftSerializer.Deserialize(ptype, buffer);

//                             if(methodParameterTypesP[i] instanceof ParameterizedType){
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, ((ParameterizedType)methodParameterTypesP[i]).getActualTypeArguments());
//                             }
//                             else{
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, null);
//                             }



//                             Type resultTypeP = methodParameterTypesP
//                             if (resultTypeP instanceof ParameterizedType) {
////                            ParameterizedType aType = (ParameterizedType) resultTypeP;
////                            Object o = ThriftSerializer.Deserialize(re, resultType, aType.getActualTypeArguments());
//                             }
        } else {
            throw new ThriftSException(String.format("not supported content type: %s", contentType));
        }
    }
}
