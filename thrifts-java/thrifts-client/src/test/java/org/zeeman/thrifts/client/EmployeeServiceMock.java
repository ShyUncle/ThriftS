package org.zeeman.thrifts.client;

import org.zeeman.thrifts.common.annotations.ThriftSContract;
import org.zeeman.thrifts.common.annotations.ThriftSOperation;

@ThriftSContract
public interface EmployeeServiceMock {
    @ThriftSOperation
    String getString();
}
