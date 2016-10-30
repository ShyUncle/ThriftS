package org.zeeman.thrifts.service;

import org.apache.thrift.TMultiplexedProcessor;
import org.apache.thrift.protocol.TBinaryProtocol;
import org.apache.thrift.server.TThreadPoolServer;
import org.apache.thrift.transport.TFramedTransport;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TTransportException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.zeeman.thrifts.common.ThriftSException;
import org.zeeman.thrifts.common.Utils;
import org.zeeman.thrifts.common.annotations.ThriftSContract;
import org.zeeman.thrifts.common.annotations.ThriftSOperation;
import org.zeeman.thrifts.idl.ThriftSHandler;

import java.lang.reflect.Method;
import java.util.TreeMap;

public class ThriftSServer {
    private final static Logger LOGGER = LoggerFactory.getLogger(ThriftSServer.class);

    private TMultiplexedProcessor multiplexedProcessor = new TMultiplexedProcessor();

    private TThreadPoolServer threadPoolServer = null;

    private ThriftSHandlerSerializer handlerSerializer = new ThriftSHandlerSerializerImpl();

    private int port = 0;

    public int getPort() {
        return this.port;
    }

    public void setPort(int value) {
        this.port = value;
    }

    public ThriftSHandlerSerializer getHandlerSerializer() {
        return handlerSerializer;
    }

    public void setHandlerSerializer(ThriftSHandlerSerializer handlerSerializer) {
        this.handlerSerializer = handlerSerializer;
    }

    public ThriftSServer() {

    }

    public void registerService(Class<?> contractType, Class<?> handlerType) throws ThriftSException {
        if (contractType == null) {
            throw new IllegalArgumentException("Illegal argument: contractType");
        }

        if (handlerType == null) {
            throw new IllegalArgumentException("Illegal argument: handlerType");
        }

        ThriftSContract contractAnnotation = contractType.getAnnotation(ThriftSContract.class);
        if (contractAnnotation == null) {
            throw new ThriftSException(String.format("Missing annotation(ThriftXContract) in '%s'.", contractType.getName()));
        }

        //get service name
        String serviceName = Utils.getServiceName(contractType);
        serviceName = serviceName.toLowerCase();
        LOGGER.debug("Register service: ", serviceName);

        LocalCache.ServiceMap.put(serviceName, new TreeMap<String, ServiceMetaInfo>(String.CASE_INSENSITIVE_ORDER));

        //foreach methods
        Method[] methods = contractType.getDeclaredMethods();
        for (Method method : methods) {
            LOGGER.debug("declared method name : " + method.getName());

            ThriftSOperation methodAnnotation = method.getAnnotation(ThriftSOperation.class);
            if (methodAnnotation == null) {
                continue;
            }

            String methodName = method.getName().toLowerCase();

            ServiceMetaInfo serviceMetaInfo = new ServiceMetaInfo();
            serviceMetaInfo.setServiceName(serviceName);
            serviceMetaInfo.setMethodName(methodName);
            serviceMetaInfo.setServiceHandlerType(handlerType);
            serviceMetaInfo.setMethod(method);
            serviceMetaInfo.setAnnotation(methodAnnotation);

            LocalCache.ServiceMap.get(serviceName).put(methodName, serviceMetaInfo);
        }

//        LocalCache.ServiceMap = Collections.unmodifiableMap(LocalCache.ServiceMap);
    }

    public void start(int httpPort, int thriftPort, int minThreads, int maxThreads, int clientTimeout)
            throws TTransportException {

        this.setPort(thriftPort);
        int clientTimeoutMS = clientTimeout * 1000;

        TServerSocket serverTransport = new TServerSocket(thriftPort, clientTimeoutMS);

//        var logDelegate = new TServer.logDelegate((message) => ThriftSEnvirnment.Logger.Error(message));

        TThreadPoolServer.Args args = new TThreadPoolServer.Args(serverTransport);
        args.processor(this.multiplexedProcessor)
                .transportFactory(new TFramedTransport.Factory())
                .protocolFactory(new TBinaryProtocol.Factory(true, true))
                .minWorkerThreads(minThreads)
                .maxWorkerThreads(maxThreads);

        this.threadPoolServer = new TThreadPoolServer(args);

        this.multiplexedProcessor.registerProcessor("ThriftSHandler", new ThriftSHandler.Processor(new ThriftSHandlerProcessor(this.handlerSerializer)));

        LOGGER.info(
                "Starting thriftS server. ThriftPort:{}, HttpPort:{}, Version:{}",
                thriftPort,
                httpPort,
                Utils.getVersion());

        this.threadPoolServer.serve();
//        var task = new Task(this.threadPoolServer.Serve);
//        task.Start();
//
//        if (httpPort > 0)
//        {
//            this.httpServer = new SimpleHttpServer(httpPort);
//            var httpTask = new Task(this.httpServer.Start);
//            httpTask.Start();
//        }
//
//        ThriftSEnvirnment.MinThreadPoolSize = minThreads;
//        ThriftSEnvirnment.MaxThreadPoolSize = maxThreads;
//        ThriftSEnvirnment.ClientTimeout = clientTimeout;

    }
}
