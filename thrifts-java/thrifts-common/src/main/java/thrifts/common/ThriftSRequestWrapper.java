package thrifts.common;

import thrifts.idl.ThriftSParameter;
import thrifts.idl.ThriftSRequest;

import java.util.HashMap;
import java.util.List;

public class ThriftSRequestWrapper {

    private ThriftSRequest innerRequest;

    private static String keyRequestUri = "request_uri";

    public ThriftSRequestWrapper(ThriftSRequest request) {
        this.innerRequest = request;

        if (this.innerRequest.getHeaders() == null) {
            this.innerRequest.setHeaders(new HashMap<String, String>());
        }
    }

    public List<ThriftSParameter> getParameters(){
        return this.innerRequest.getParameters();
    }

    public String getUri() {
        return this.innerRequest.getHeaders().get(keyRequestUri);
    }

    public void setUri(String value) {
        this.innerRequest.getHeaders().put(keyRequestUri, value);
    }

}
