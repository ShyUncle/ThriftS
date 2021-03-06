package org.zeeman.thrifts.client;

import org.junit.Assert;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.zeeman.thrifts.common.ThriftSException;

/**
 * Created by zeeman on 2016/10/30.
 */
public class ThriftSClientTest {
    private final static Logger LOGGER = LoggerFactory.getLogger(ThriftSClientTest.class);

    @Test
    public void StringTest() throws ThriftSException {
        ThriftSClient thriftSClient = new ThriftSClient("127.0.0.1", 8384);
        EmployeeServiceMock employeeServiceMock = (EmployeeServiceMock) thriftSClient.createProxy(EmployeeServiceMock.class);
        Assert.assertNotNull(employeeServiceMock);

        String result = employeeServiceMock.bigString("string test");
        LOGGER.info(result);
    }
}
