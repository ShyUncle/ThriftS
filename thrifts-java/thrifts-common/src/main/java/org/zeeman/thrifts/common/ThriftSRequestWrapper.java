package org.zeeman.thrifts.common;

import org.apache.commons.lang3.StringUtils;
import org.zeeman.thrifts.idl.ThriftSParameter;
import org.zeeman.thrifts.idl.ThriftSRequest;

import java.util.HashMap;
import java.util.List;

public class ThriftSRequestWrapper {

    private ThriftSRequest innerRequest;

    /**
     * 请求Id
     */
    private final static String KEY_REQUEST_ID = "request_id";

    /**
     * 请求资源路径
     */
    private final static String KEY_REQUEST_URI = "request_uri";

    /**
     * 跟踪Id, 用于串联服务调用栈
     */
    private final static String KEY_TRACKER_ID = "tracker_id";

    /**
     * 组件版本
     */
    private final static String KEY_VERSION = "client_version";

    /**
     * 客户端IP地址
     */
    private final static String KEY_CLIENT_IP = "client_ip";

    /**
     * 客户端主机名
     */
    private final static String KEY_CLIENT_HOST_NAME = "client_hostname";

    /**
     * 客户端进程号
     */
    private final static String KEY_CLIENT_PID = "client_pid";

    /**
     * 客户端运行时(Java JRE或者.NET CLR版本)
     */
    private final static String KEY_CLIENT_RUNTIME = "client_runtime";

    /**
     * 接受的ContentType
     */
    private final static String KEY_ACCEPT = "accept";

    public ThriftSRequestWrapper(ThriftSRequest request) {
        this.innerRequest = request;

        if (this.innerRequest.getHeaders() == null) {
            this.innerRequest.setHeaders(new HashMap<String, String>());
        }
    }

    public List<ThriftSParameter> getParameters(){
        return this.innerRequest.getParameters();
    }

    public String getRequestId() {
        if (this.innerRequest.getHeaders().containsKey(KEY_REQUEST_ID) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_REQUEST_ID);
    }

    public void setRequestId(String value) {
        this.innerRequest.getHeaders().put(KEY_REQUEST_ID, value);
    }

    public String getUri() {
        if (this.innerRequest.getHeaders().containsKey(KEY_REQUEST_URI) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_REQUEST_URI);
    }

    public void setUri(String value) {
        this.innerRequest.getHeaders().put(KEY_REQUEST_URI, value);
    }

    public String getTrackerId() {
        if (this.innerRequest.getHeaders().containsKey(KEY_TRACKER_ID) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_TRACKER_ID);
    }

    public void setTrackerId(String value) {
        this.innerRequest.getHeaders().put(KEY_TRACKER_ID, value);
    }

    public String getVersion() {
        if (this.innerRequest.getHeaders().containsKey(KEY_VERSION) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_VERSION);
    }

    public void setVersion(String value) {
        this.innerRequest.getHeaders().put(KEY_VERSION, value);
    }

    public String getClientIP() {
        if (this.innerRequest.getHeaders().containsKey(KEY_CLIENT_IP) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_CLIENT_IP);
    }

    public void setClientIP(String value) {
        this.innerRequest.getHeaders().put(KEY_CLIENT_IP, value);
    }

    public String getClientHostName() {
        if (this.innerRequest.getHeaders().containsKey(KEY_CLIENT_HOST_NAME) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_CLIENT_HOST_NAME);
    }

    public void setClientHostName(String value) {
        this.innerRequest.getHeaders().put(KEY_CLIENT_HOST_NAME, value);
    }

    public String getClientPid() {
        if (this.innerRequest.getHeaders().containsKey(KEY_CLIENT_PID) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_CLIENT_PID);
    }

    public void setClientPid(String value) {
        this.innerRequest.getHeaders().put(KEY_CLIENT_PID, value);
    }

    public String getClientRuntime() {
        if (this.innerRequest.getHeaders().containsKey(KEY_CLIENT_RUNTIME) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_CLIENT_RUNTIME);
    }

    public void setClientRuntime(String value) {
        this.innerRequest.getHeaders().put(KEY_CLIENT_RUNTIME, value);
    }

    public String getAccept() {
        if (this.innerRequest.getHeaders().containsKey(KEY_ACCEPT) == false)
        {
            return StringUtils.EMPTY;
        }
        return this.innerRequest.getHeaders().get(KEY_ACCEPT);
    }

    public void setAccept(String value) {
        this.innerRequest.getHeaders().put(KEY_ACCEPT, value);
    }
}
