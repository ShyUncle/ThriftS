package org.zeeman.thrifts.serializer;

import org.zeeman.thrifts.common.SerializerMode;
import org.zeeman.thrifts.common.annotations.ThriftSMember;
import org.zeeman.thrifts.common.annotations.ThriftSModel;

/**
 * Created by zeeman on 2016/10/25.
 */
@ThriftSModel(SerializerMode = SerializerMode.ProtoBuf)
public class MemberMock {
    //@Protobuf(fieldType = FieldType.STRING, order = 1, required = true)
    @ThriftSMember(tag = 1)
    private String memberId;

    @ThriftSMember(tag = 2)
    private int age;

    public MemberMock() {
    }

    public String getMemberId() {
        return memberId;
    }

    public void setMemberId(String memberId) {
        this.memberId = memberId;
    }

    public int getAge() {
        return age;
    }

    public void setAge(int age) {
        this.age = age;
    }
}