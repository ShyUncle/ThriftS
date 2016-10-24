package thrifts.demo.contract;

import thrifts.common.annotations.ThriftSContract;
import thrifts.common.annotations.ThriftSOperation;

import java.util.ArrayList;

@ThriftSContract
public interface IEmployeeService {
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
