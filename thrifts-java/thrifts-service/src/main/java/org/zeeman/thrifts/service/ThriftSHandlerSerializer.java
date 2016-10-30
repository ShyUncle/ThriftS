package org.zeeman.thrifts.service;

import org.zeeman.thrifts.common.ThriftSException;

import java.lang.reflect.Type;

/**
 * Created by zeeman on 2016/10/30.
 */
public interface ThriftSHandlerSerializer {
    byte[] Serialize(String contentType,  Object instance) throws ThriftSException;
    Object Deserialize(String contentType, Type type, byte[] buffer) throws ThriftSException;
}
