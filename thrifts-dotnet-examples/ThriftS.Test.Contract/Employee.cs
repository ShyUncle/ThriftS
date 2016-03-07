using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftS.Common.Attributes;

namespace ThriftS.Test.Contract
{
    [ThriftSModel]
    public class Member
    {
        [ThriftSMember(1)]
        public string MemberId { get; set; }

        [ThriftSMember(2)]
        public int Age { get; set; }

        [ThriftSMember(3)]
        public Address Address { get; set; }

        [ThriftSMember(4)]
        public List<Address> MultiAddresses { get; set; }
    }

    [ThriftSModel]
    public class Address
    {
        [ThriftSMember(1)]
        public string City { get; set; }

        [ThriftSMember(2)]
        public string PostCode { get; set; }

        [ThriftSMember(3)]
        public Tel Tel { get; set; }
    }

    [ThriftSModel]
    public class Tel
    {
        [ThriftSMember(1)]
        public int TelId { get; set; }
    }

    [ThriftSModel]
    public class Employee : Member
    {
        private int _EmployeeId;
        private string _EmployeeName;

        [ThriftSMember(101)]
        public int EmployeeId
        {
            get { return _EmployeeId; }
            set { this._EmployeeId = value; }
        }

        [ThriftSMember(102)]
        public string EmployeeName
        {
            get { return _EmployeeName; }
            set { this._EmployeeName = value; }
        }
    }

    public class Manager : Employee
    {
        public string Title { get; set; }

        public ManagerLevel Level { get; set; }
    }

    public enum ManagerLevel
    {
        ProjectManager = 1,

        DepartmentManager = 2
    }
}
