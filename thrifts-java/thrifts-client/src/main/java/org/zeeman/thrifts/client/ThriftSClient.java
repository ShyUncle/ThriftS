package org.zeeman.thrifts.client;

import org.apache.commons.lang3.StringUtils;
import org.zeeman.thrifts.common.ThriftSException;
import org.zeeman.thrifts.common.Utils;
import org.zeeman.thrifts.common.annotations.ThriftSContract;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Proxy;

/**
 * ThriftS client
 */
public class ThriftSClient {
    private final static Logger LOGGER = LoggerFactory.getLogger(ThriftSClient.class);

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
     * @param contractType
     * @return
     */
    public Object createProxy(Class<?> contractType) throws ThriftSException {
        ThriftSContract contractAnnotation = contractType.getAnnotation(ThriftSContract.class);
        if (contractAnnotation == null) {
            throw new ThriftSException(String.format("Missing annotation(ThriftXContract) in '%s'.", contractType.getName()));
        }

        String serviceName = Utils.getServiceName(contractType);
        String serviceShortName = serviceName;

        if (StringUtils.isEmpty(serviceName)) {
            //serviceName = typeof(T).FullName;
            serviceShortName = contractType.getName();
        }

        LOGGER.debug("find service: {}", serviceName);

        InvocationHandler handler = new ThriftSRealProxy(this.getHost(),
                this.getPort(), serviceName, serviceShortName, this.getTimeout());

        return Proxy.newProxyInstance(handler.getClass().getClassLoader(),
                new Class[]{contractType}, handler);

    }

}
