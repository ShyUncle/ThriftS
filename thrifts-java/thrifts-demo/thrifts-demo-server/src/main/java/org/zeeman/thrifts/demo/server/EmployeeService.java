package org.zeeman.thrifts.demo.server;

import org.zeeman.thrifts.demo.contract.IEmployeeService;
import org.zeeman.thrifts.demo.contract.Member;
import org.zeeman.thrifts.common.annotations.ThriftSOperation;

import java.util.ArrayList;

public class EmployeeService implements IEmployeeService {
    @Override
    public void saveMember(Member member){
        System.out.println(member.getMemberId());
    }

    @Override
    public String bigString(String text) {
        return text;
    }

    @Override
    public Member getMember() {
        Member member = new Member();
        member.setMemberId("0001");
        member.setAge(66);
        return member;
    }

    @Override
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
