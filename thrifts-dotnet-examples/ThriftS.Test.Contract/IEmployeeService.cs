using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common.Attributes;

namespace ThriftS.Test.Contract
{
    [ThriftSContract(ServiceName = "contract.IEmployeeService")]
    public interface IEmployeeService
    {
        [ThriftSOperation(IsOneWay = true, Name = "saveEmployee")]
        void SaveEmployee(Employee emp);

        [ThriftSOperation]
        void SaveMember(Member member);

        [ThriftSOperation(Description = "get manager list")]
        List<Manager> GetList(int id, string name, DateTime indate, bool sex);

        [ThriftSOperation]
        string BigString(string text);

        [ThriftSOperation]
        byte[] BigBytes(byte[] buffer);

        //non attr test
        void NonAttr();

        [ThriftSOperation]
        object NullObject();

        [ThriftSOperation]
        Dictionary<string, Employee> KeyValues(Dictionary<string, Employee> input);

        [ThriftSOperation]
        void SaveNull(Employee emp1, Employee emp2, Employee emp3);

        [ThriftSOperation]
        Member GetMember();

        [ThriftSOperation]
        List<Member> GetMemberList();

        [ThriftSOperation]
        List<Member> SaveList(int id, List<Member> list, string memo);
    }
}
