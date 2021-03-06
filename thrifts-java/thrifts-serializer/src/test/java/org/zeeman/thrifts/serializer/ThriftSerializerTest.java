package org.zeeman.thrifts.serializer;

import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;
import org.junit.Assert;
import org.junit.Test;
import org.zeeman.thrifts.common.ThriftSException;

import java.util.ArrayList;
import java.util.HashMap;

/**
 * Created by zeeman on 2016/10/25.
 */
public class ThriftSerializerTest {
    @Test
    public void i32Test() throws ThriftSException {
        TypeResolver typeResolver = new TypeResolver();

        final int intMock = 32;
        byte[] i32bytes = ThriftSerializer.Serialize(intMock);
        Object i32value = ThriftSerializer.Deserialize(typeResolver.resolve(int.class), i32bytes);

        Assert.assertEquals(intMock, (int) (Integer) i32value);
    }

    @Test
    public void stringTest() throws ThriftSException {
        TypeResolver typeResolver = new TypeResolver();

        final String stringMock = "hello";
        byte[] stringbytes = ThriftSerializer.Serialize(stringMock);
        Object stringvalue = ThriftSerializer.Deserialize(typeResolver.resolve(String.class), stringbytes);

        Assert.assertEquals(stringMock, (String) stringvalue);
    }

    @Test
    public void objectTest() throws ThriftSException {
        TypeResolver typeResolver = new TypeResolver();

        final String memberIdMock = "xxx";

        MemberMock memberMock = new MemberMock();
        memberMock.setMemberId(memberIdMock);
        byte[] memberbytes = ThriftSerializer.Serialize(memberMock);
        Object actual = ThriftSerializer.Deserialize(typeResolver.resolve(MemberMock.class), memberbytes);

        Assert.assertNotNull(actual);
        Assert.assertEquals(memberIdMock, ((MemberMock) actual).getMemberId());
    }

    @Test
    public void arrayTest() throws ThriftSException {
        TypeResolver typeResolver = new TypeResolver();

        MemberMock memberMock = new MemberMock();
        memberMock.setMemberId("xxx");

        ArrayList<MemberMock> mems = new ArrayList<MemberMock>();
        mems.add(memberMock);
        byte[] listbytes = ThriftSerializer.Serialize(mems);

        //Type listType = new TypeToken<ArrayList<MemberMock>>(){}.getType();
        Object actual = ThriftSerializer.Deserialize(typeResolver.resolve(ArrayList.class, MemberMock.class), listbytes);
        Assert.assertNotNull(actual);

        ArrayList<MemberMock> actualMembers = (ArrayList<MemberMock>)actual;
        Assert.assertEquals(actualMembers.size(), 1);
    }

    @Test
    public void simpleMapTest() throws ThriftSException {
        TypeResolver typeResolver = new TypeResolver();

        HashMap<Integer, String> dic = new HashMap<Integer, String>();
        dic.put(1, "赵");
        dic.put(2, "钱");
        dic.put(3, "孙");
        byte[] dicbytes = ThriftSerializer.Serialize(dic);

        //Type listType3 = new TypeToken<HashMap<Integer,String>>(){}.getType();
        ResolvedType type = typeResolver.resolve(HashMap.class, Integer.class, String.class);

        Object actual = ThriftSerializer.Deserialize(type, dicbytes);

        Assert.assertNotNull(actual);

        HashMap<Integer, String> actualDic = (HashMap<Integer, String>)actual;
        Assert.assertEquals(actualDic.size(), 3);
        Assert.assertEquals(actualDic.get(1), "赵");
        Assert.assertEquals(actualDic.get(2), "钱");
        Assert.assertEquals(actualDic.get(3), "孙");
    }

    @Test
    public void mixMapTest() throws ThriftSException {
        //Type t = resolveGenericType(List.class,int.class);

        TypeResolver typeResolver = new TypeResolver();

        MemberMock memberMock = new MemberMock();
        memberMock.setMemberId("xxx");

        ArrayList<MemberMock> mems = new ArrayList<MemberMock>();
        mems.add(memberMock);

        HashMap<Integer, ArrayList<MemberMock>> dic3 = new HashMap<Integer, ArrayList<MemberMock>>();
        dic3.put(1, mems);
        dic3.put(2, mems);
        dic3.put(3, mems);
        byte[] dicbytes2 = ThriftSerializer.Serialize(dic3);
        ResolvedType listType2 = typeResolver.resolve(HashMap.class, Integer.class, typeResolver.resolve(ArrayList.class, MemberMock.class));
        Object actual = ThriftSerializer.Deserialize(listType2, dicbytes2);

        Assert.assertNotNull(actual);
        HashMap<Integer, ArrayList<MemberMock>> actualMap = (HashMap<Integer, ArrayList<MemberMock>>)actual;
        Assert.assertEquals(actualMap.size(), 3);
    }

    @Test
    public void nestedMapTest() throws ThriftSException {
        //todo: map in map
    }
}
