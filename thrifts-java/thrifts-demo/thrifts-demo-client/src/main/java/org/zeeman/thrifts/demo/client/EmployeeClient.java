package org.zeeman.thrifts.demo.client;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.zeeman.thrifts.client.ThriftSClient;
import org.zeeman.thrifts.common.ThriftSException;
import org.zeeman.thrifts.demo.contract.EmployeeService;

/**
 * Created by zeeman on 2016/10/25.
 */
public class EmployeeClient {
    private final static Logger LOGGER = LoggerFactory.getLogger(EmployeeClient.class);

    public static void StringTest() throws ThriftSException {
        ThriftSClient thriftSClient = new ThriftSClient("127.0.0.1", 8384);
        EmployeeService employeeService = (EmployeeService) thriftSClient.createProxy(EmployeeService.class);

        String result = employeeService.bigString("string test");
        LOGGER.info(result);
    }
}
