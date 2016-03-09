# ThriftS

_ThriftS_基于Apache Thrift开发，旨在为.NET和JAVA提供更简单、更高效、更轻量的RPC通信机制。Thrift作为多语言通信框架，直接使用会有较大的侵入性，而web services等传统的交互方式不能满足我们的性能要求，ThriftS提供了另一种选择。

Target Framework Version
----
* _ThriftS_.NET部分基于.NET Framework 4.0编译，可在.NET 4.0或更高版本的32位或64位环境中使用。除Thrift外它本身不依赖任何第三方类库，非常干净。
* _ThriftS_ Java部分基于JDK 1.6编译。

Features
----
* 无需编写[IDL](http://thrift.apache.org/docs/idl)代码，使用语言自身习惯开发。
* 依据原始编码规则实现序列化和反序列化机制，从而使数据实体不受TBase继承所约束。
* 无Thrift代码修改，不受Thrift版本更新影响。
* 使用二进制编码，支持大数据gzip压缩。
* 提供Http端口监控服务运行情况。
* C#客户端提供连接池支持。
* 后续支持AOP方法拦截和自定义序列化。

Quick Start
----

Contract
```c#
[ThriftSContract]
public interface IEmployeeService
{
    [ThriftSOperation]
    void SaveEmployee(Employee emp);
}
```

Service side
```c#
public class EmployeeService : IEmployeeService
{
    public void SaveEmployee(Employee emp)
    {
        Console.WriteLine("client save employee. Id: {0},Name:{1}.", emp.EmployeeId, emp.EmployeeName);
    }
}

var server = new ThriftSServer();
server.Start();
```

Service side configuration: ThriftS.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="hostConfiguration" type="ThriftS.Service.HostConfigurationSetion,ThriftS.Service"/>
  </configSections>
  <hostConfiguration defaultHost="default" >
    <hosts>
      <host name="default" thriftPort="8384" httpPort="18384" minThreadPoolSize="5" maxThreadPoolSize="200" clientTimeout="60" useBufferedSockets="false">
        <services>
          <service contract="ThriftS.Test.Contract.IEmployeeService"
                   contractAssembly="ThriftS.Test.Contract"
                   handler="ThriftS.Test.Server.EmployeeService"
                   handlerAssembly="ThriftS.Test.Server" />
        </services>
      </host>
    </hosts>
  </hostConfiguration>
</configuration>
```

Client side
```c#
ThriftSClient client = new ThriftSClient("127.0.0.1", 8384);
var proxy = client.CreateProxy<IEmployeeService>();
var employee = new Employee() { EmployeeId = 18168, EmployeeName = "zeeman" };
proxy.SaveEmployee(employee);
```

Performance
----
A key feature of _ThriftS_ is performance. 

License
----
Apache License

About me
----
Email: amwicfai@gmail.com
