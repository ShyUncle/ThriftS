package org.zeeman.thrifts.client;

import org.junit.Assert;
import org.junit.Test;

/**
 * Created by zeeman on 2016/10/30.
 */
public class ThriftSClientTest {
    @Test
    public void StringTest() {
        ThriftSClient thriftSClient = new ThriftSClient("127.0.0.1", 8384);
        EmployeeServiceMock employeeServiceMock = (EmployeeServiceMock) thriftSClient.createProxy(EmployeeServiceMock.class);
        Assert.assertNotNull(employeeServiceMock);

        String result = employeeServiceMock.getString();
        System.out.print(result);
    }
}
