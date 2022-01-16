
using System.Text;

namespace BasicWebServer.Server.HTTP
{
    public class Response
    {
        public StatusCode StatusCode { get; init; }

        public HeaderCollection Headers { get; set; }

        public string Body { get; set; }

        public Action<Request,Response> PreRenderAction { get; set; }

        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
            Headers = new HeaderCollection();
            
            Headers.Add(Header.Server, "My Web Server");
            Headers.Add(Header.Date, $"{DateTime.UtcNow:R}");
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine($"HTTP/1.1 {(int)StatusCode} {StatusCode}");

            foreach (var header in Headers)
            {
                result.AppendLine(header.ToString());
            }
            result.AppendLine();
            if (!string.IsNullOrEmpty(Body))
            {
                result.Append(Body);
            }

            return result.ToString();
        }
    }
}
