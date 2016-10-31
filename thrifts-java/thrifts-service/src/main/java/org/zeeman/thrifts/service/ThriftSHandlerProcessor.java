package org.zeeman.thrifts.service;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.zeeman.thrifts.common.*;
import org.zeeman.thrifts.idl.*;
import org.apache.thrift.TException;

import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.util.HashMap;
import java.util.LinkedHashMap;

class ThriftSHandlerProcessor implements ThriftSHandler.Iface {
    private final static Logger LOGGER = LoggerFactory.getLogger(ThriftSHandlerProcessor.class);

    private ThriftSHandlerSerializer handlerSerializer;

    public ThriftSHandlerProcessor(ThriftSHandlerSerializer handlerSerializer) {
        this.handlerSerializer = handlerSerializer;
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
                || StringUtils.isEmpty(request.ServiceName)
                || StringUtils.isEmpty(request.MethodName)) {
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
                        Object val = this.handlerSerializer.Deserialize(
                                requestParameter.getContentType(),
                                methodParameterTypesP[i],
                                requestParameter.getValue()
                        );

                        invokeParameters.put(parameterType.getName(), val);
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

                    if (mode == SerializerMode.ProtoBuf) {
                        xResult.setContentType(ContentTypes.Protobuf);
                    } else if (mode == SerializerMode.Thrift) {
                        xResult.setContentType(ContentTypes.Thrift);
                    } else {
                        xResult.setContentType(ContentTypes.Json);
                    }
                    xResult.setData(this.handlerSerializer.Serialize(xResult.getContentType(), result));

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
