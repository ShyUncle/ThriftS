using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Service;
using ThriftS.Test.Contract;

namespace ThriftS.Test.Server
{
    public class EmployeeService : IEmployeeService
    {
        public void SaveEmployee(Employee emp)
        {
            Console.WriteLine("tracker id: " + ThriftSContext.Current.Request.TrackerId);
            Console.WriteLine("client save employee. Id: {0},Name:{1}.", emp.EmployeeId, emp.EmployeeName);
        }

        public void SaveMember(Member member)
        {
            Console.WriteLine("client save member. Id: {0}.", member.MemberId);
        }

        public List<Manager> GetList(int id, string name, DateTime indate, bool sex)
        {
            if (id < 10)
            {
                throw new ArgumentOutOfRangeException("id");
            }

            Console.WriteLine("client get employee list." + id.ToString() + "," + name + "," + indate + "," + sex);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                sb.Append("thrift是一个软件框架，用来进行可扩展且跨语言的服务的开发。");
            }

            return new List<Manager>()
            {
                new Manager()
                {
                    MemberId = "m1",
                    EmployeeId = 1,
                    EmployeeName = "老马",
                    Title = "董事长," + sb.ToString(),
                    Level = ManagerLevel.ProjectManager
                },
                new Manager()
                {
                    MemberId = "m2",
                    EmployeeId = 2,
                    EmployeeName = "小陆",
                    Title = "总经理," + sb.ToString(),
                    Level = ManagerLevel.DepartmentManager
                }
            };
        }

        public string BigString(string text)
        {
            return text;
        }

        public byte[] BigBytes(byte[] buffer)
        {
            return buffer;
        }


        public void NonAttr()
        {
        }

        public Dictionary<string, Employee> KeyValues(Dictionary<string, Employee> input)
        {
            return input;
        }


        public object NullObject()
        {
            return null;
        }


        public void SaveNull(Employee emp1, Employee emp2, Employee emp3)
        {
            if (emp1 == null)
            {
                Console.WriteLine("client save employee. emp1 is null");
            }
            else
            {
                Console.WriteLine("client save employee. Id: {0},Name:{1}.", emp1.EmployeeId, emp1.EmployeeName);
            }

            if (emp2 == null)
            {
                Console.WriteLine("client save employee. emp2 is null");
            }
            else
            {
                Console.WriteLine("client save employee. Id: {0},Name:{1}.", emp2.EmployeeId, emp2.EmployeeName);
            }

            if (emp3 == null)
            {
                Console.WriteLine("client save employee. emp3 is null");
            }
            else
            {
                Console.WriteLine("client save employee. Id: {0},Name:{1}.", emp3.EmployeeId, emp3.EmployeeName);
            }
        }


        public Member GetMember()
        {
            var member = new Member()
            {
                MemberId = "M01",
                Age = 20
            };
            return member;
        }


        public List<Member> GetMemberList()
        {
            var result = new List<Member>();
            result.Add(new Member()
            {
                MemberId = "M01",
                Age = 20
            });
            return result;
        }


        public List<Member> SaveList(int id, List<Member> list, string memo)
        {
            return list;
        }
    }
}
