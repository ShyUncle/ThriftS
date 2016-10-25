package org.zeeman.thrifts.service;

import org.apache.thrift.TMultiplexedProcessor;
import org.apache.thrift.protocol.TBinaryProtocol;
import org.apache.thrift.server.TThreadPoolServer;
import org.apache.thrift.transport.TFramedTransport;
import org.apache.thrift.transport.TServerSocket;
import org.apache.thrift.transport.TTransportException;
import thrifts.common.Utils;
import thrifts.common.annotations.ThriftSContract;
import thrifts.common.annotations.ThriftSOperation;
import thrifts.idl.ThriftSHandler;

import java.lang.reflect.Method;
import java.util.TreeMap;

public class ThriftSServer {
    private TMultiplexedProcessor multiplexedProcessor = new TMultiplexedProcessor();

    private TThreadPoolServer threadPoolServer = null;

    private int port = 0;

    public int getPort()
    {
        return this.port;
    }

    public void setPort(int value)
    {
        this.port = value;
    }

    public ThriftSServer()
    {

    }

    public void registerService(Class<?> contractType, Class<?> handlerType)
    {
        if(contractType == null)
        {
            // throw
        }

        if(handlerType == null)
        {
            // throw
        }

        ThriftSContract contractAnnotation = contractType.getAnnotation(ThriftSContract.class);
        if(contractAnnotation==null)
        {
            //throw new ThriftXException(string.Format("Missing ThriftXContractAttribute in '{0}'.", contractType));
        }
        //Todo: 未定义annotation

        //取契约名称
        String serviceName = contractAnnotation.serviceName();
        if(serviceName == null || serviceName.isEmpty())
        {
            serviceName = contractType.getName();
        }
        serviceName = serviceName.toLowerCase();
        System.out.println(serviceName);

        LocalCache.ServiceMap.put(serviceName, new TreeMap<String, ServiceMetaInfo>(String.CASE_INSENSITIVE_ORDER));

        //遍历契约方法
        Method[] methods = contractType.getDeclaredMethods();
        for (Method method : methods) {
            System.out.println("declared method name : " + method.getName());

            ThriftSOperation methodAnnotation = method.getAnnotation(ThriftSOperation.class);
            if(methodAnnotation==null)
            {
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

        this.multiplexedProcessor.registerProcessor("ThriftSHandler", new ThriftSHandler.Processor(new ThriftSHandlerProcessor()));

        ThriftSEnvirnment.getLogger().Info(
                "Starting thriftS server. ThriftPort:%s, HttpPort:%s, Version:%s",
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
