package org.zeeman.thrifts.client;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Method;

/**
 * ThriftS real proxy
 */
public class ThriftSRealProxy implements InvocationHandler {

    private String host;
    private int port;
    private String serviceName;
    private String serviceShortName;
    private int timeout;
    private int clientPid;
    private String clientIp;
    private String clientHostName;

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

    public String getServiceName() {
        return serviceName;
    }

    public void setServiceName(String serviceName) {
        this.serviceName = serviceName;
    }

    public String getServiceShortName() {
        return serviceShortName;
    }

    public void setServiceShortName(String serviceShortName) {
        this.serviceShortName = serviceShortName;
    }

    public int getTimeout() {
        return timeout;
    }

    public void setTimeout(int timeout) {
        this.timeout = timeout;
    }

    public int getClientPid() {
        return clientPid;
    }

    public void setClientPid(int clientPid) {
        this.clientPid = clientPid;
    }

    public String getClientIp() {
        return clientIp;
    }

    public void setClientIp(String clientIp) {
        this.clientIp = clientIp;
    }

    public String getClientHostName() {
        return clientHostName;
    }

    public void setClientHostName(String clientHostName) {
        this.clientHostName = clientHostName;
    }

    public ThriftSRealProxy(String host, int port, String serviceName, String serviceShortName, int timeout) {
        this.setHost(host);
        this.setPort(port);
        this.setServiceName(serviceName);
        this.setServiceShortName(serviceShortName);
        this.setTimeout(timeout);
        //this.setClientPid();
        //this.setClientHostName();
    }

    public Object invoke(Object object, Method method, Object[] args)
            throws Throwable
    {
        return "this is rpc result";
    }
}
