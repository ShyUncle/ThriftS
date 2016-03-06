using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThriftS.Service.Http
{
    /// <summary>
    /// Class HttpProcessor.
    /// </summary>
    internal class HttpProcessor
    {
        /// <summary>
        /// The bu f_ size
        /// </summary>
        private static int bufferSize = 4096;

        /// <summary>
        /// The maximum x_ position t_ size
        /// </summary>
        private static int maxPostSize = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// The input stream
        /// </summary>
        private Stream inputStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProcessor"/> class.
        /// </summary>
        /// <param name="s">The arguments.</param>
        /// <param name="srv">The SRV.</param>
        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.Socket = s;
            this.Server = srv;
            this.HttpHeaders = new Hashtable();
        }

        /// <summary>
        /// The socket
        /// </summary>
        public TcpClient Socket { get; set; }

        /// <summary>
        /// The SRV
        /// </summary>
        public HttpServer Server { get; set; }

        /// <summary>
        /// The output stream
        /// </summary>
        public StreamWriter OutputStream { get; set; }

        /// <summary>
        /// The http_method
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// The http_url
        /// </summary>
        public string HttpUrl { get; set; }

        /// <summary>
        /// The http_protocol_versionstring
        /// </summary>
        public string HttpProtocolVersion { get; set; }

        /// <summary>
        /// The HTTP headers
        /// </summary>
        public Hashtable HttpHeaders { get; set; }

        /// <summary>
        /// Processes this instance.
        /// </summary>
        public void Process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            this.inputStream = new BufferedStream(this.Socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            this.OutputStream = new StreamWriter(new BufferedStream(this.Socket.GetStream()));
            try
            {
                this.ParseRequest();
                this.ReadHeaders();
                if (this.HttpMethod.Equals("GET"))
                {
                    this.HandleGETRequest();
                }
                else if (this.HttpMethod.Equals("POST"))
                {
                    this.HandlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                this.WriteFailure();
            }

            this.OutputStream.Flush();
            //// bs.Flush(); // flush any remaining output
            this.inputStream = null;
            this.OutputStream = null; // bs = null;            
            this.Socket.Close();
        }

        /// <summary>
        /// Parses the request.
        /// </summary>
        /// <exception cref="System.Exception">invalid http request line</exception>
        public void ParseRequest()
        {
            string request = this.StreamReadLine(this.inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }

            this.HttpMethod = tokens[0].ToUpper();
            this.HttpUrl = tokens[1];
            this.HttpProtocolVersion = tokens[2];

            // Console.WriteLine("starting: " + request);
        }

        /// <summary>
        /// Reads the headers.
        /// </summary>
        /// <exception cref="System.Exception">invalid http header line:  + line</exception>
        public void ReadHeaders()
        {
            // Console.WriteLine("readHeaders()");
            string line;
            while ((line = this.StreamReadLine(this.inputStream)) != null)
            {
                if (line.Equals(string.Empty))
                {
                    // Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }

                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);

                // Console.WriteLine("header: {0}:{1}", name, value);
                this.HttpHeaders[name] = value;
            }
        }

        /// <summary>
        /// Handles the get request.
        /// </summary>
        public void HandleGETRequest()
        {
            this.Server.HandleGETRequest(this);
        }

        /// <summary>
        /// Handles the post request.
        /// </summary>
        /// <exception cref="System.Exception">
        /// client disconnected during post
        /// </exception>
        public void HandlePOSTRequest()
        {
            //// this post data processing just reads everything into a memory stream.
            //// this is fine for smallish things, but for large stuff we should really
            //// hand an input stream to the request processor. However, the input stream 
            //// we hand him needs to let him see the "end of the stream" at this content 
            //// length, because otherwise he won't know when he's seen it all! 

            // Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.HttpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.HttpHeaders["Content-Length"]);
                if (content_len > maxPostSize)
                {
                    throw new Exception(
                        string.Format(
                        "POST Content-Length({0}) too big for this simple server",
                          content_len));
                }

                byte[] buf = new byte[bufferSize];
                int to_read = content_len;
                while (to_read > 0)
                {
                    //// Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(bufferSize, to_read));

                    // Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }

                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }

                ms.Seek(0, SeekOrigin.Begin);
            }

            // Console.WriteLine("get post data end");
            this.Server.HandlePOSTRequest(this, new StreamReader(ms));
        }

        /// <summary>
        /// Writes the success.
        /// </summary>
        /// <param name="content_type">The content_type.</param>
        public void WriteSuccess(string content_type = "text/html")
        {
            // this is the successful HTTP response line
            this.OutputStream.WriteLine("HTTP/1.0 200 OK");

            // these are the HTTP headers...          
            this.OutputStream.WriteLine("Content-Type: " + content_type);
            this.OutputStream.WriteLine("Connection: close");

            // ..add your own headers here if you like

            // this terminates the HTTP headers.. everything after this is HTTP body..
            this.OutputStream.WriteLine(string.Empty); 
        }

        /// <summary>
        /// Writes the failure.
        /// </summary>
        public void WriteFailure()
        {
            // this is an http 404 failure response
            this.OutputStream.WriteLine("HTTP/1.0 404 File not found");

            // these are the HTTP headers
            this.OutputStream.WriteLine("Connection: close");

            //// ..add your own headers here

            this.OutputStream.WriteLine(string.Empty); // this terminates the HTTP headers.
        }

        /// <summary>
        /// Streams the read line.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>System.String.</returns>
        private string StreamReadLine(Stream inputStream)
        {
            int next_char;
            string data = string.Empty;
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n')
                {
                    break;
                }

                if (next_char == '\r')
                {
                    continue;
                }

                if (next_char == -1)
                {
                    Thread.Sleep(1);
                    continue;
                }

                data += Convert.ToChar(next_char);
            }

            return data;
        }
    }
}
