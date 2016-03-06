using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThriftS.Common;
using ThriftS.Common.Attributes;

namespace ThriftS.Service.Http
{
    /// <summary>
    /// Class SimpleHttpServer.
    /// </summary>
    internal class SimpleHttpServer : HttpServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHttpServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public SimpleHttpServer(int port)
            : base(port)
        {
        }

        /// <summary>
        /// Handles the get request.
        /// </summary>
        /// <param name="p">The application.</param>
        public override void HandleGETRequest(HttpProcessor p)
        {
            /*
            if (p.http_url.Equals("/Test.png"))
            {
                Stream fs = File.Open("../../Test.png", FileMode.Open);

                p.writeSuccess("image/png");
                fs.CopyTo(p.outputStream.BaseStream);
                p.outputStream.BaseStream.Flush();
            }*/

            // Console.WriteLine("request: {0}", p.HttpUrl);
            p.WriteSuccess();

            try
            {
                p.OutputStream.WriteLine(
                    "<html><header><title>ThriftS Server</title></header><body><h1>ThriftS Server</h1>");

                p.OutputStream.WriteLine("Application Name: " + AppDomain.CurrentDomain.SetupInformation.ApplicationName);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Current Directory: " +
                                         AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Command Line: " + Environment.CommandLine);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Startup Time: " + Process.GetCurrentProcess().StartTime.ToString());
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Process Id: " + Process.GetCurrentProcess().Id);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Threads Count: " + Process.GetCurrentProcess().Threads.Count);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Min Thread Pool Size: " + ThriftSEnvirnment.MinThreadPoolSize.ToString());
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Max Thread Pool Size: " + ThriftSEnvirnment.MaxThreadPoolSize.ToString());
                p.OutputStream.WriteLine("<br />");
                using (var memoryCounter = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName))
                {
                    // p.OutputStream.WriteLine("Used Memory: " + (Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)).ToString() + " MB");
                    p.OutputStream.WriteLine("Used Memory: " + Math.Round(memoryCounter.NextValue() / (1024 * 1024), 2).ToString() + " MB");
                    p.OutputStream.WriteLine("<br />");
                }

                using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                {
                    p.OutputStream.WriteLine("Used CPU: " + Math.Round(cpuCounter.NextValue(), 2).ToString() + " %");
                    p.OutputStream.WriteLine("<br />");
                }

                p.OutputStream.WriteLine("<br />");

                p.OutputStream.WriteLine("Server Name: " + Environment.MachineName);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Processor Count: " + Environment.ProcessorCount);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("OS Version: " + Environment.OSVersion);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("Server Time: " + DateTime.Now.ToString());
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("<br />");

                p.OutputStream.WriteLine("Framework Version: " +
                                         AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("ThriftS Version: " + Utils.Version);
                p.OutputStream.WriteLine("<br />");
                p.OutputStream.WriteLine("<!--ThriftS Author: XieSi-->");
                p.OutputStream.WriteLine("<br />");

                foreach (var contractEl in LocalCache.ServiceDictionary)
                {
                    p.OutputStream.WriteLine("<span style='color:blue'>Contract: ");
                    p.OutputStream.WriteLine(contractEl.Key);
                    p.OutputStream.WriteLine("</span>");
                    p.OutputStream.WriteLine("<br />");

                    foreach (var methodEl in contractEl.Value)
                    {
                        p.OutputStream.WriteLine("&nbsp;&nbsp;&nbsp;&nbsp;<span style='color:green'>");
                        p.OutputStream.WriteLine(methodEl.Value.Method);
                        p.OutputStream.WriteLine("</span><br />");
                    }
                }

                p.OutputStream.WriteLine("</body></html>");
            }
            catch (Exception exception)
            {
                p.OutputStream.WriteLine(exception);
            }

            //// p.OutputStream.WriteLine("url : {0}", p.HttpUrl);

            /*
            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form>");
             * */
        }

        /// <summary>
        /// Handles the post request.
        /// </summary>
        /// <param name="p">The application.</param>
        /// <param name="inputData">The input data.</param>
        public override void HandlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            // Console.WriteLine("POST request: {0}", p.HttpUrl);
            string data = inputData.ReadToEnd();

            p.WriteSuccess();
            /*
            p.OutputStream.WriteLine("<html><body><h1>thrifts server</h1>");
            p.OutputStream.WriteLine("<a href=/test>return</a><p>");
            p.OutputStream.WriteLine("postbody: <pre>{0}</pre>", data);
            */
        }
    }
}
