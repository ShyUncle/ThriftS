package org.zeeman.thrifts.client;

import org.zeeman.thrifts.common.Utils;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Proxy;

/**
 * ThriftS client
 */
public class ThriftSClient {
    private String host;
    private int port;
    private int timeout;

    /**
     * Initializes a new instance of the ThriftSClient class.
     *
     * @param host
     * @param port
     */
    public ThriftSClient(String host, int port) {
        this(host, port, 120);
    }

    /**
     * Initializes a new instance of the ThriftSClient class.
     *
     * @param host
     * @param port
     * @param timeout
     */
    public ThriftSClient(String host, int port, int timeout) {
        this.setHost(host);
        this.setPort(port);
        this.setTimeout(timeout);
    }

    public String getHost() {
        return host;
    }

    public void setHost(String host) {
        this.host = host;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    public int getTimeout() {
        return timeout;
    }

    public void setTimeout(int timeout) {
        this.timeout = timeout;
    }

    /**
     * Create proxy
     *
     * @param serviceInterface
     * @return
     */
    public Object createProxy(Class serviceInterface) {
        String serviceName = "";//Utils.getServiceName(typeof(T));
        String serviceShortName = serviceName;

        InvocationHandler handler = new ThriftSRealProxy(this.getHost(),
                this.getPort(), serviceName, serviceShortName, this.getTimeout());

        return Proxy.newProxyInstance(handler.getClass().getClassLoader(),
                new Class[]{serviceInterface}, handler);
        /*
        if (typeof(T).IsDefined(typeof(ThriftSContractAttribute), false) == false)
        {
            throw new ThriftSException(string.Format("Missing ThriftSContractAttribute in '{0}'.", typeof(T)));
        }

        var serviceName = Utils.GetServiceName(typeof(T));
        var serviceShortName = serviceName;
        if (string.IsNullOrEmpty(serviceName))
        {
            serviceName = typeof(T).FullName;
            serviceShortName = typeof(T).Name;
        }
        */
    }

}
