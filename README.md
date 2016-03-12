# ThriftS

RPC is a classical server-client model, where client sends a request to server, and server replies results to client. _ThriftS_ is a micro RPC framework based on Apache Thrift, it is designed to be as simple as possible, and it should be lightweight. Target language is c# and java.

Runtime
----
* For .NET, writting in c#, will run on .NET Framework 4.0, no dependencies except Thrift.
* For Java，will run on JDK 1.6, because Java generics erase mechanism itself, uses[classmate](https://github.com/FasterXML/java-classmate) to support generics serialization.

Features
----
* No [interface description language](http://thrift.apache.org/docs/idl).
* Based on the original encoding rules to implement the serialization and deserialization.
* No Thrift code changed.
* Using binary encoding, support for large data gzip compression.
* Provide monitoring service uses Http.
* Support connection pool in c# client.

Quick Start
----
I think it's good time for an example.

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
