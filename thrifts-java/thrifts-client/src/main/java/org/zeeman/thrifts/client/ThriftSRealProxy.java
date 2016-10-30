package org.zeeman.thrifts.client;

import com.fasterxml.classmate.TypeResolver;
import org.apache.commons.lang3.StringUtils;
import org.apache.thrift.TException;
import org.apache.thrift.protocol.TBinaryProtocol;
import org.apache.thrift.protocol.TMultiplexedProtocol;
import org.apache.thrift.transport.TFramedTransport;
import org.apache.thrift.transport.TSocket;
import org.zeeman.thrifts.common.ContentTypes;
import org.zeeman.thrifts.common.ThriftSException;
import org.zeeman.thrifts.idl.*;
import org.zeeman.thrifts.serializer.ThriftSerializer;

import java.lang.reflect.InvocationHandler;
import java.lang.reflect.Method;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.HashMap;

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

    public Object invoke(Object object, Method method, Object[] args) throws Throwable {
        ThriftSRequest sRequest = new ThriftSRequest();
        sRequest.setServiceName(this.getServiceName());
        sRequest.setMethodName(method.getName());
        sRequest.setHeaders(new HashMap<String, String>());
        sRequest.setParameters(new ArrayList<ThriftSParameter>());

        /*
        srequest.Uri = string.Format("thrift://{0}:{1}/{2}/{3}", this.Host, this.Port, this.ServiceShortName, srequest.MethodName);
        srequest.Version = Utils.Version;
        srequest.ClientPid = this.ClientPid;
        srequest.ClientHostName = this.ClientHostName;
        srequest.ClientRuntime = "Java " + Environment.Version.ToString();
        * */

        if (args != null && args.length > 0) {
            for (byte i = 0; i < args.length; i++) {
                Object arg = args[i];

                ThriftSParameter parameter = null;
                if (arg == null) {
                    parameter = new ThriftSParameter();
                    parameter.setIndex(i);
                    parameter.setName(StringUtils.EMPTY);
                    parameter.setType(StringUtils.EMPTY);
                    parameter.setCompression(ThriftSCompression.None);
                    parameter.setContentType(ContentTypes.Binary);
                    parameter.setHasValue(false);
                } else {
                    parameter = new ThriftSParameter();
                    parameter.setIndex(i);
                    parameter.setName(StringUtils.EMPTY);
                    parameter.setType(StringUtils.EMPTY);
                    parameter.setCompression(ThriftSCompression.None);
                    parameter.setContentType(ContentTypes.Thrift);
                    parameter.setHasValue(true);
                    try {
                        parameter.setValue(ThriftSerializer.Serialize(arg));
                    } catch (ThriftSException e) {
                        throw e;
                    }
                }

                if (parameter.getValue() != null
                        && parameter.getValue().length > 10 * 1024) {
                    //parameter.Value = Utils.GzipCompress(parameter.Value);
                    //parameter.Compression = ThriftSCompression.Gzip;
                }

                sRequest.getParameters().add(parameter);
            }
        }

        TFramedTransport transport = null;
        ThriftSResponse sResponse = null;
        try {
            TSocket socket = new TSocket(this.getHost(), this.getPort(), this.getTimeout() * 1000);
            socket.getSocket().setTcpNoDelay(true);
            transport = new TFramedTransport(socket);
            TMultiplexedProtocol protocol = new TMultiplexedProtocol(new TBinaryProtocol(transport), "ThriftSHandler");
            ThriftSHandler.Client client = new ThriftSHandler.Client(protocol);

            transport.open();
            sResponse = client.Process(sRequest);
        } catch (SocketException e) {
            throw e;
        } catch (BadRequestException e) {
            throw e;
        } catch (InvocationException e) {
            throw e;
        } catch (InternalServerException e) {
            throw e;
        } catch (TException e) {
            throw e;
        } finally {
            if (transport != null && transport.isOpen()) {
                transport.close();
            }
        }

        if (sResponse != null
                && sResponse.getResult() != null) {
            /*
            if (sresponse.Result.Compression == ThriftSCompression.Gzip)
            {
                sresponse.Result.Data = Utils.GzipUnCompress(sresponse.Result.Data);
            }
            * */

            if (sResponse.getResult().getContentType().equalsIgnoreCase(ContentTypes.Thrift)) {
                TypeResolver typeResolver = new TypeResolver();
                return ThriftSerializer.Deserialize(typeResolver.resolve(method.getReturnType()), sResponse.getResult().getData());
            } else {
                //throw new NotSupportedException(string.Format("Not supported content type: {0}", sresponse.Result.ContentType));
            }
        }

        return null;
    }
}
