package thrifts.demo.server;

import thrifts.demo.contract.IEmployeeService;
import thrifts.demo.contract.Member;
import thrifts.service.ThriftSServer;
import thrifts.serializer.ThriftSerializer;
import com.fasterxml.classmate.ResolvedType;
import com.fasterxml.classmate.TypeResolver;

import java.util.ArrayList;
import java.util.HashMap;

public class ServerStart {
    public static void main(String[] args) {
        try {

            //Type t = resolveGenericType(List.class,int.class);

            ThriftSServer server = new ThriftSServer();
            server.registerService(IEmployeeService.class, EmployeeService.class);
            server.start(80, 8384, 5, 20, 120);

            HashMap<Integer,String> dic = new HashMap<Integer,String>();
            dic.put(1, "赵");
            dic.put(2, "钱");
            dic.put(3, "孙");
            byte[] dicbytes = ThriftSerializer.Serialize(dic);
//            Type listType3 = new TypeToken<HashMap<Integer,String>>(){}.getType();

            TypeResolver typeResolver = new TypeResolver();
            ResolvedType type = typeResolver.resolve(HashMap.class, Integer.class, String.class);

            Object dic2 = ThriftSerializer.Deserialize(type, dicbytes);

            byte[] i32bytes = ThriftSerializer.Serialize(32);
            Object i32value = ThriftSerializer.Deserialize(typeResolver.resolve(int.class), i32bytes);

            byte[] stringbytes = ThriftSerializer.Serialize("hello");
            Object stringvalue = ThriftSerializer.Deserialize(typeResolver.resolve(String.class), stringbytes);

            Member member = new Member();
            member.setMemberId("xxx");
            byte[] memberbytes = ThriftSerializer.Serialize(member);
            Object member2 = ThriftSerializer.Deserialize(typeResolver.resolve(Member.class),memberbytes);

            ArrayList<Member> mems = new ArrayList<Member>();
            mems.add(member);
            byte[] listbytes = ThriftSerializer.Serialize(mems);
//            Type listType = new TypeToken<ArrayList<Member>>(){}.getType();
            Object mems3 = ThriftSerializer.Deserialize(typeResolver.resolve(ArrayList.class,Member.class), listbytes);

            HashMap<Integer,ArrayList<Member>> dic3 =new HashMap<Integer,ArrayList<Member>>();
            dic3.put(1, mems);
            dic3.put(2, mems);
            dic3.put(3, mems);
            byte[] dicbytes2 = ThriftSerializer.Serialize(dic3);
//            Type listType2 = new TypeToken<HashMap<Integer,ArrayList<Member>>>(){}.getType();
            ResolvedType listType2 = typeResolver.resolve(HashMap.class, Integer.class,typeResolver.resolve(ArrayList.class,Member.class));
            Object dic4 = ThriftSerializer.Deserialize(listType2, dicbytes2);

            System.out.println("ok");
            // map 套 map的情况

        }catch (Exception exception)
        {
            exception.printStackTrace();
        }
    }
}
