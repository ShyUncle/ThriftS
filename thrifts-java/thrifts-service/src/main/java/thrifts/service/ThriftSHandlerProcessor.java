package thrifts.service;

import com.baidu.bjf.remoting.protobuf.Codec;
import com.baidu.bjf.remoting.protobuf.ProtobufProxy;
import thrifts.common.*;
import thrifts.idl.*;
import thrifts.serializer.ThriftSerializer;
import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import org.apache.thrift.TException;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.lang.reflect.ParameterizedType;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;

class ThriftSHandlerProcessor implements ThriftSHandler.Iface {
    private static Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd HH:mm:ss").create();

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
                || request.MethodName == null || request.MethodName.isEmpty())
        {
            throw new BadRequestException(request, "The ServiceName or MethodName must be not null.");
        }

        if(LocalCache.ServiceMap.containsKey(request.getServiceName()) == false)
        {
            throw new BadRequestException(request,"Service not found.");
        }

        if(LocalCache.ServiceMap.get(request.getServiceName()).containsKey(request.getMethodName()) == false){
            throw new BadRequestException(request,"Method not found.");
        }

        ThriftSRequestWrapper thriftXRequestWrapper = new ThriftSRequestWrapper(request);
        ThriftSEnvirnment.getLogger().Debug("Accept request: %s", thriftXRequestWrapper.getUri());

        ServiceMetaInfo serviceMetadata = LocalCache.ServiceMap.get(request.getServiceName()).get(request.MethodName);

        ThriftSResponse xResponse = new ThriftSResponse();
        xResponse.setHeaders(new HashMap<String, String>());
        try {
            Object result = null;

            //ʵ��������
            Object service = null;
            try {
                service = serviceMetadata.getServiceHandlerType().newInstance();
            } catch (InstantiationException e) {
                e.printStackTrace();
            }

            //��ȡ���ò���
            // Class<?>[] methodParameterTypes = serviceMetadata.getMethod().getParameterTypes();
            Type[] methodParameterTypesP = serviceMetadata.getMethod().getGenericParameterTypes();
            LinkedHashMap<String,Object> invokeParameters = new LinkedHashMap<String, Object>();
            for(int i=0;i<methodParameterTypesP.length;i++) {
                Class<?> parameterType = null;

                if(methodParameterTypesP[i] instanceof ParameterizedType){
                    parameterType = (Class<?>)((ParameterizedType)methodParameterTypesP[i]).getRawType();
                }
                else{
                    parameterType = (Class<?>)methodParameterTypesP[i];
                }

                // �ڿ�ƽ̨��ʱ�����˺Ϳͻ��˵Ĳ�����������ȫ��һ�������Ψһ�����ǲ���˳�����������ĵĲ������Է����Ϊ��׼��
                ThriftSParameter requestParameter = thriftXRequestWrapper.getParameters().get(i);
                //requestParameter.Name = parameter.getName();
                try {
                    if (requestParameter.getCompression() == ThriftSCompression.Gzip) {

                    }

                    if(requestParameter.isHasValue() == false){
                        invokeParameters.put(parameterType.getName(),null);
                    }
                    else{

//                        if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Thrift)) {
//                            if(parameterType == String.class) {
//                                invokeParameters.put(parameterType.getName(), requestParameter.getValue());
//                            }
//                        }else
                         if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Protobuf)){
                            // ��������jproto������
                            Codec protoCodec = ProtobufProxy.create(parameterType);
                            invokeParameters.put(parameterType.getName(),protoCodec.decode(requestParameter.getValue()));
                        }
                        else if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Json)) {
                             ByteArrayInputStream inputStream = new ByteArrayInputStream(requestParameter.getValue());
                             try {
                                 InputStreamReader reader = new InputStreamReader(inputStream);
                                 invokeParameters.put(parameterType.getName(), gson.fromJson(reader, parameterType));
                             } finally {
                                 inputStream.close();
                             }
                         }
                        else if(requestParameter.getContentType().equalsIgnoreCase(ContentTypes.Thrift)) {

                             TypeResolver typeResolver = new TypeResolver();
                             ResolvedType ptype = null;

                             if(methodParameterTypesP[i] instanceof ParameterizedType){
                                 ParameterizedType typeTemp = (ParameterizedType) methodParameterTypesP[i];
                                 if(typeTemp.getRawType() == ArrayList.class){
                                     ptype = typeResolver.resolve(ArrayList.class,typeTemp.getActualTypeArguments());
                                 }
                                 else if(typeTemp.getRawType() == HashMap.class){
                                     ptype = typeResolver.resolve(HashMap.class,typeTemp.getActualTypeArguments());
                                 }
                                 //parameterType =typeResolver.resolve(((ParameterizedType) methodParameterTypesP[i]).getRawType(), ((ParameterizedType) methodParameterTypesP[i]).getActualTypeArguments());
                             }
                             else{
                                 ptype = typeResolver.resolve(methodParameterTypesP[i]);
                             }

                             Object val = ThriftSerializer.Deserialize(ptype, requestParameter.getValue());

//                             if(methodParameterTypesP[i] instanceof ParameterizedType){
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, ((ParameterizedType)methodParameterTypesP[i]).getActualTypeArguments());
//                             }
//                             else{
//                                 val = ThriftSerializer.Deserialize(requestParameter.getValue(), parameterType, null);
//                             }
                             invokeParameters.put(parameterType.getName(),val);


//                             Type resultTypeP = methodParameterTypesP
//                             if (resultTypeP instanceof ParameterizedType) {
////                            ParameterizedType aType = (ParameterizedType) resultTypeP;
////                            Object o = ThriftSerializer.Deserialize(re, resultType, aType.getActualTypeArguments());
//                             }
                        }
                        else{
                            //throw new Unsupported
                        }
                    }
                }
                catch (Exception e){

                }

            }

            //���÷���
            try {
                result = serviceMetadata.getMethod().invoke(service,invokeParameters.values().toArray());
                System.out.println(result);
            }catch (Exception e){
                e.printStackTrace();
            }

            //��鷵��ֵ����
            //����Java�ķ��Ͳ�������Ҫͨ�������ķ����ȡ���Ͳ���
            Class<?> resultType = serviceMetadata.getMethod().getReturnType();


            //����Ƿ��Ͷ��壬��ȡ���Ͳ���
//            Type genericReturnType = serviceMetadata.getMethod().getGenericReturnType();
//            if(genericReturnType instanceof ParameterizedType){
//                resultType = (Class<?>)((ParameterizedType) genericReturnType).getActualTypeArguments()[0];
//            }

            //���ڷ���ֵ
            if(resultType != void.class) {
                if (result != null) {

                    //��ȡ���л���ʽ
                    SerializerMode mode = Utils.GetSerializerMode(resultType);
                    ThriftSResult xResult = new ThriftSResult();
                    xResult.setCompression(ThriftSCompression.None);

                    //byte[] listbytes = ThriftSerializer.Serialize(result);
                    //Object mems3 = ThriftSerializer.Deserialize(resultType, listbytes);

                    if (mode == SerializerMode.ProtoBuf) {
//                        LinkedBuffer buffer = LinkedBuffer.allocate(LinkedBuffer.DEFAULT_BUFFER_SIZE);
//                        //RuntimeSchema.getSchema(ArrayList.class,String.class);
//                        //CollectionSchema<String>.
//                        Schema schema = RuntimeSchema.createFrom(resultType);//(result.getClass(), String.class);
//                        byte[] bytes =  ProtobufIOUtil.toByteArray(result, schema, buffer);
//                        xResult.setData(bytes);
                        //Protobuf���л�
                        Class<? extends Object> T = result.getClass();
                        Codec protoCodec = ProtobufProxy.create(T);
//                        Codec protoCodec = ProtobufProxy.create(resultType);
                        xResult.setContentType(ContentTypes.Protobuf);
                        try {
                            xResult.setData(protoCodec.encode(result));
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    } else if(mode == SerializerMode.Thrift) {
                        xResult.setContentType(ContentTypes.Thrift);
                        xResult.setData(ThriftSerializer.Serialize(result));
                    } else {
                        //Json���л�
                        //todo: json-smart
                        xResult.setContentType(ContentTypes.Json);
                        xResult.setData(gson.toJson(result).getBytes());
                    }
                    xResponse.setResult(xResult);
                }
            }
        }catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (ThriftSException e) {
            e.printStackTrace();
        }

        return xResponse;
    }
}
