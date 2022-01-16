using BasicWebServer.Server.Common;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Routing
{
    public class RoutingTable : IRoutingTable
    {
        private readonly Dictionary<Method, Dictionary<string, Response>> routes;

        public RoutingTable()
            => routes = new Dictionary<Method, Dictionary<string, Response>>()
            {
                [Method.GET] = new Dictionary<string, Response>(),
                [Method.POST] = new Dictionary<string, Response>(),
                [Method.PUT] = new Dictionary<string, Response>(), 
                [Method.DELETE] = new Dictionary<string, Response>(),

            };
        public IRoutingTable Map(string url, Method method, Response response)
            => method switch
            {
                Method.GET => MapGet(url,response),
                Method.POST => MapPost(url,response),
                _ => throw new InvalidOperationException ($"Method '{method}' is not supported.")
            };

        public IRoutingTable Map(string url, MethodAccessException method, Response response)
        {
            throw new NotImplementedException();
        }

        public IRoutingTable MapGet(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            routes[Method.GET][url] = response;
            
            return this;
        }

        public IRoutingTable MapPost(string url, Response response)
        {
            Guard.AgainstNull(url, nameof(url));
            Guard.AgainstNull(response, nameof(response));

            routes[Method.POST][url] = response;

            return this;
        }

        public Response MatchRequest(Request request)
        {
            var requestMethod = request.Method;
            var requestUrl = request.Url;

            if ((!routes.ContainsKey(requestMethod)) || !routes[requestMethod].ContainsKey(requestUrl))
            {
                return new NotFoundResponse();
            }
            return routes[requestMethod][requestUrl];
        }
    }
}
