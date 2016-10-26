package org.zeeman.thrifts.demo.server;

import org.zeeman.thrifts.demo.contract.IEmployeeService;
import org.zeeman.thrifts.demo.contract.Member;
import org.zeeman.thrifts.common.annotations.ThriftSOperation;

import java.util.ArrayList;

public class EmployeeService implements IEmployeeService {
    public void saveMember(Member member){
        System.out.println(member.getMemberId());
    }

    public String bigString(String text) {
        return text;
    }

    public Member getMember() {
        Member member = new Member();
        member.setMemberId("0001");
        member.setAge(66);
        return member;
    }

    public ArrayList<Member> getMemberList() {
        ArrayList<Member> list = new ArrayList<Member>();

        Member member = new Member();
        member.setMemberId("list.0001");
        member.setAge(66);
        list.add(member);

        return list;
    }

    @ThriftSOperation
    public ArrayList<Member> saveList(int id, ArrayList<Member> list, String memo){
        return list;
    }
}
