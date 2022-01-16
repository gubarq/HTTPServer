using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BasicWebServer.Server
{
    
   public class HttpServer
    {
        private readonly IPAddress ipAddress;
        private readonly int port;
        private readonly TcpListener serverListener;

        private readonly RoutingTable routingTable;

        public HttpServer(string ipAddress, int port, Action<IRoutingTable> routingTableConfiguration)
        {
            this.ipAddress = IPAddress.Any;
            this.port = port;

            serverListener = new TcpListener(this.ipAddress,port);

            routingTableConfiguration(routingTable = new RoutingTable());
        }
        public HttpServer(int port, Action<IRoutingTable> routingTable)
            :this("127.0.0.1", port , routingTable)
        {
        }
        public HttpServer(Action<IRoutingTable> routingTable)
           : this(8080, routingTable)
        {
        }

        public void Start()
        {
            serverListener.Start();
            Console.WriteLine($"Server started on port {port}...");
            Console.WriteLine("Listening for requests...");

            while (true)
            {
                var connection = serverListener.AcceptTcpClient();
                var networkStream = connection.GetStream();

                var requestText = ReadRequset(networkStream);

                Console.WriteLine(requestText);
                
                var request = Request.Parse(requestText);

                var response = routingTable.MatchRequest(request);

                if (response.PreRenderAction != null)
                    response.PreRenderAction(request,response);

                WriteResponse(networkStream, response);

                connection.Close();

            }
        }
        private void WriteResponse(NetworkStream networkStream, Response response)
        {
         
            var responesBytes = Encoding.UTF8.GetBytes(response.ToString());

            networkStream.Write(responesBytes);
        }

        private string ReadRequset(NetworkStream networkStream)
        {
            var bufferLength = 10000000;
            var buffer = new byte[bufferLength];

            var totalBytes = 0;

            var requsetBuilder = new StringBuilder();

            do
            {
                var bytesRead = networkStream.Read(buffer, 0, bufferLength);

                totalBytes += bytesRead;
                if (totalBytes > 10 * 10000000)
                {
                    throw new InvalidOperationException("Request is too large");
                }

                requsetBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
            } while (networkStream.DataAvailable);

            return requsetBuilder.ToString();
        }
    }
   
}
