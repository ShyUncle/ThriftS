using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThriftS.Service.Http
{
    /// <summary>
    /// Class HttpServer.
    /// </summary>
    internal abstract class HttpServer
    {
        /// <summary>
        /// The listener
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// The is_active
        /// </summary>
        private bool isActive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServer"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        public HttpServer(int port)
        {
            this.Port = port;
        }

        /// <summary>
        /// The port
        /// </summary>
        protected int Port { get; set; }

        /// <summary>
        /// start this instance.
        /// </summary>
        public void Start()
        {
            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.listener.Start();

            while (this.isActive)
            {
                TcpClient s = this.listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                var task = new Task(processor.Process);
                task.Start();

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// stop this instance.
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
        }

        /// <summary>
        /// Handles the get request.
        /// </summary>
        /// <param name="p">The application.</param>
        public abstract void HandleGETRequest(HttpProcessor p);

        /// <summary>
        /// Handles the post request.
        /// </summary>
        /// <param name="p">The application.</param>
        /// <param name="inputData">The input data.</param>
        public abstract void HandlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
}
