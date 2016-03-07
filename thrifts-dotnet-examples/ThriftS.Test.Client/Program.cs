using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Client;
using ThriftS.Common;
using ThriftS.Test.Contract;

namespace ThriftS.Test.Client
{
    class Program 
    {
        private static void Main(string[] args)
        {
            ThriftSClient client = new ThriftSClient("127.0.0.1", 8384);

            try
            {
                var proxy = client.CreateProxy<IEmployeeService>();

                Member mem = new Member();
                mem.MemberId = "z001";
                proxy.SaveMember(mem);

                var member = proxy.GetMember();
                Console.WriteLine(member.MemberId);

                var members = proxy.GetMemberList();
                foreach (var m in members)
                {
                    Console.WriteLine(m.MemberId);
                }

                members = proxy.SaveList(100, members, "zee");
                foreach (var m in members)
                {
                    Console.WriteLine(m.MemberId);
                }

                var employee = new Employee() { EmployeeId = 18168, EmployeeName = "呵呵" };
                Stopwatch sw = new Stopwatch();

                sw.Start();
                proxy.SaveNull(employee, null, null);
                sw.Stop();
                Console.WriteLine("{0} Save {1}ms", DateTime.Now, sw.ElapsedMilliseconds);

                sw.Restart();
                proxy.SaveEmployee(employee);
                sw.Stop();
                Console.WriteLine("Re Save {0}ms", sw.ElapsedMilliseconds);

                sw.Restart();
                var emps = proxy.GetList(70, "抗战纪念日", DateTime.Now, true);
                sw.Stop();
                Console.WriteLine("GetList {0}ms", sw.ElapsedMilliseconds);

                foreach (var emp in emps)
                {
                    Console.WriteLine("------------------");
                    Console.WriteLine("MemberId:" + emp.MemberId);
                    Console.WriteLine("EmployeeId:" + emp.EmployeeId);
                    Console.WriteLine("EmployeeName:" + emp.EmployeeName);
                    Console.WriteLine("Title:" + emp.Title);
                    Console.WriteLine("Level:" + emp.Level);
                }

                sw.Restart();
                var buf = proxy.BigBytes(Encoding.UTF8.GetBytes("hello"));
                sw.Stop();
                Console.WriteLine("BigBytes {0}ms", sw.ElapsedMilliseconds);
                Console.WriteLine("buf:" + Encoding.UTF8.GetString(buf));

                //proxy.NonAttr();

                Dictionary<string, Employee> dicReq = new Dictionary<string, Employee>();
                dicReq.Add("zeeman", new Employee() { EmployeeName = "zeeman huang" });
                sw.Restart();
                var dicRes = proxy.KeyValues(dicReq);
                sw.Stop();
                Console.WriteLine("KeyValues {0}ms", sw.ElapsedMilliseconds);
                Console.WriteLine("dicRes:" + dicRes);

                sw.Restart();
                var nullObj = proxy.NullObject();
                sw.Stop();
                Console.WriteLine("NullObject {0}ms", sw.ElapsedMilliseconds);
                Console.WriteLine("nullObj:" + nullObj);

                Console.ReadKey();
            }
            catch (ThriftSException ex)
            {
                Console.WriteLine("{0} ThriftSException异常", DateTime.Now);
                Console.WriteLine(ex.Message);
                Console.WriteLine("==================");
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} 异常", DateTime.Now);
                Console.WriteLine(ex);
            }

            Console.WriteLine("ok," + DateTime.Now.ToString());
            Console.ReadLine();
        }
    }
}
