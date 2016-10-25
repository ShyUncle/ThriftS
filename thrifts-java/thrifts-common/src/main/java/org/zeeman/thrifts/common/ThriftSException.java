package org.zeeman.thrifts.common;

public class ThriftSException extends Exception {
    public ThriftSException(String message) {
        super(message);
    }

    public ThriftSException(String message, Exception innerException) {
        super(message,innerException);
    }
}
