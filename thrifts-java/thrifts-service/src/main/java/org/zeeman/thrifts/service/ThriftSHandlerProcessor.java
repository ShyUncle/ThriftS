package org.zeeman.thrifts.service;

//import com.baidu.bjf.remoting.protobuf.Codec;
//import com.baidu.bjf.remoting.protobuf.ProtobufProxy;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.zeeman.thrifts.common.*;
import org.zeeman.thrifts.idl.*;
import org.zeeman.thrifts.serializer.ThriftSerializer;
import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
//import com.google.gson.Gson;
//import com.google.gson.GsonBuilder;
import org.apache.thrift.TException;

import java.io.ByteArrayInputStream;
import java.io.InputStreamReader;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;

class ThriftSHandlerProcessor implements ThriftSHandler.Iface {
    private final static Logger LOGGER = LoggerFactory.getLogger(ThriftSHandlerProcessor.class);

    //private static Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss").create();

    public ThriftSHandlerProcessor() {

    }

    public ThriftSResponse Ping(ThriftSRequest request) throws BadRequestException, InternalServerException, TException {
        return null;
    }

    public ThriftSResponse Hello(ThriftSRequest request) throws BadRequestException, InternalServerException, TException {
        HashMap<String, String> serverProperties = new HashMap<String, String>();
        serverProperties.put("thriftx_server_version", Utils.getVersion());

        ThriftSResponse xresponse = new ThriftSResponse();
        xresponse.setHeaders(serverProperties);

        return xresponse;
    }

    public ThriftSResponse Process(ThriftSRequest request) throws BadRequestException, InternalServerException, InvocationException, TException {
        if (request == null
                || request.ServiceName == null || request.ServiceName.isEmpty()
                || request.MethodName == null || request.MethodName.isEmpty()) {
            throw new BadRequestException(request, "The ServiceName or MethodName must be not null.");
        }

        if (LocalCache.ServiceMap.containsKey(request.getServiceName()) == false) {
            throw new BadRequestException(request, "Service not found.");
        }

        if (LocalCache.ServiceMap.get(request.getServiceName()).containsKey(request.getMethodName()) == false) {
            throw new BadRequestException(request, "Method not found.");
        }

        ThriftSRequestWrapper thriftSRequestWrapper = new ThriftSRequestWrapper(request);
        LOGGER.debug("Accept request: {}", thriftSRequestWrapper.getUri());

        ServiceMetaInfo serviceMetadata = LocalCache.ServiceMap.get(request.getServiceName()).get(request.MethodName);

        ThriftSResponse xResponse = new ThriftSResponse();
        xResponse.setHeaders(new HashMap<String, String>());
        try {
            Object result = null;

            //实例化服务
            Object service = null;
            try {
                service = serviceMetadata.getServiceHandlerType().newInstance();
            } catch (InstantiationException e) {
                e.printStackTrace();
            }

            //获取调用参数
            // Class<?>[] methodParameterTypes = serviceMetadata.getMethod().getParameterTypes();
            Type[] methodParameterTypesP = serviceMetadata.getMethod().getGenericParameterTypes();
            LinkedHashMap<String, Object> invokeParameters = new LinkedHashMap<String, Object>();
            for (int i = 0; i < methodParameterTypesP.length; i++) {
                Class<?> parameterType = null;

                if (methodParameterTypesP[i] instanceof ParameterizedType) {
                    parameterType = (Class<?>) ((ParameterizedType) methodParameterTypesP[i]).getRawType();
                } else {
                    parameterType = (Class<?>) methodParameterTypesP[i];
                }

                // 在跨平台的时候服务端和客户端的参数名可能完全不一样，因此唯一依据是参数顺序，请求上下文的参数名以服务端为基准。
                ThriftSParameter requestParameter = thriftSRequestWrapper.getParameters().get(i);
                //requestParameter.Name = parameter.getName();
                try {
                    if (requestParameter.getCompression() == ThriftSCompression.Gzip) {

                    }

                    if (requestParameter.isHasValue() == false) {
                        invokeParameters.put(parameterType.getName(), null);
                    } else {

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

                        if (requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Thrift)) {

                            TypeResolver typeResolver = new TypeResolver();
                            ResolvedType ptype = null;

                            if (methodParameterTypesP[i] instanceof ParameterizedType) {
                                ParameterizedType typeTemp = (ParameterizedType) methodParameterTypesP[i];
                                if (typeTemp.getRawType() == ArrayList.class) {
                                    ptype = typeResolver.resolve(ArrayList.class, typeTemp.getActualTypeArguments());
                                } else if (typeTemp.getRawType() == HashMap.class) {
                                    ptype = typeResolver.resolve(HashMap.class, typeTemp.getActualTypeArguments());
                                }
                                //parameterType =typeResolver.resolve(((ParameterizedType) methodParameterTypesP[i]).getRawType(), ((ParameterizedType) methodParameterTypesP[i]).getActualTypeArguments());
                            } else {
                                ptype = typeResolver.resolve(methodParameterTypesP[i]);
                            }

                            Object val = ThriftSerializer.Deserialize(ptype, requestParameter.getValue());

//                             if(methodParameterTypesP[i] instanceof ParameterizedType){
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, ((ParameterizedType)methodParameterTypesP[i]).getActualTypeArguments());
//                             }
//                             else{
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, null);
//                             }
                            invokeParameters.put(parameterType.getName(), val);


//                             Type resultTypeP = methodParameterTypesP
//                             if (resultTypeP instanceof ParameterizedType) {
////                            ParameterizedType aType = (ParameterizedType) resultTypeP;
////                            Object o = ThriftSerializer.Deserialize(re, resultType, aType.getActualTypeArguments());
//                             }
                        } else {
                            //throw new Unsupported
                        }
                    }
                } catch (Exception e) {

                }

            }

            //调用服务
            try {
                result = serviceMetadata.getMethod().invoke(service, invokeParameters.values().toArray());
                LOGGER.debug(result.toString());
            } catch (Exception e) {
                e.printStackTrace();
            }

            //检查返回值类型
            //由于Java的泛型擦除，需要通过方法的反射获取泛型参数
            Class<?> resultType = serviceMetadata.getMethod().getReturnType();


            //如果是泛型定义，则取泛型参数
//            Type genericReturnType = serviceMetadata.getMethod().getGenericReturnType();
//            if(genericReturnType instanceof ParameterizedType){
//                resultType = (Class<?>)((ParameterizedType) genericReturnType).getActualTypeArguments()[0];
//            }

            //存在返回值
            if (resultType != void.class) {
                if (result != null) {

                    //获取序列化方式
                    SerializerMode mode = Utils.getSerializerMode(resultType);
                    ThriftSResult xResult = new ThriftSResult();
                    xResult.setCompression(ThriftSCompression.None);

                    //byte[] listbytes = ThriftSerializer.Serialize(result);
                    //Object mems3 = ThriftSerializer.Deserialize(resultType, listbytes);

                    if (mode == SerializerMode.ProtoBuf) {
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
                    } else if (mode == SerializerMode.Thrift) {
                        xResult.setContentType(ContentTypes.Thrift);
                        xResult.setData(ThriftSerializer.Serialize(result));
                    }/* else {
                        //Json序列化
                        xResult.setContentType(ContentTypes.Json);
                        xResult.setData(gson.toJson(result).getBytes());
                    }*/
                    xResponse.setResult(xResult);
                }
            }
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (ThriftSException e) {
            e.printStackTrace();
        }

        return xResponse;
    }
}
