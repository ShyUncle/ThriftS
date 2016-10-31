package org.zeeman.thrifts.demo.contract;

import org.zeeman.thrifts.common.annotations.ThriftSContract;
import org.zeeman.thrifts.common.annotations.ThriftSOperation;

import java.util.ArrayList;

@ThriftSContract(serviceName = "EmployeeService")
public interface EmployeeService {
    @ThriftSOperation
    void saveMember(Member member);

    @ThriftSOperation
    String bigString(String text);

    @ThriftSOperation
    Member getMember();

    @ThriftSOperation
    ArrayList<Member> getMemberList();

    @ThriftSOperation
    ArrayList<Member> saveList(int id, ArrayList<Member> list, String memo);
}
