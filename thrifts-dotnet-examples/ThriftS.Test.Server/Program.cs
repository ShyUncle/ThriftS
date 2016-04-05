using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Service;
using ThriftS.Test.Contract;

namespace ThriftS.Test.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ThriftSEnvirnment.Logger = new MyThriftSLogger();

                var server = new ThriftSServer();
                // server.RegisterService(typeof(IEmployeeService), typeof(EmployeeService));
                server.Start();
                Console.ReadLine();

                server.Stop();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败");
                Console.WriteLine(ex);
                Console.ReadLine();
            }
        }
    }

    public class MyThriftSLogger : IThriftSLogger
    {
        public virtual void Debug(string format, params object[] args)
        {
            //Console.WriteLine(string.Concat(DateTime.Now, " Debug ", string.Format(format, args)));
        }

        public virtual void Info(string format, params object[] args)
        {
            Console.WriteLine(string.Concat(DateTime.Now, " Info ", string.Format(format, args)));
        }

        public virtual void Error(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Concat(DateTime.Now, " Error ", string.Format(format, args)));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public virtual void Error(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Concat(DateTime.Now, " Error ", exception));
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
