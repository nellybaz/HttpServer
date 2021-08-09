using System;
using HttpServer.Library;
namespace HttpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var staticPath = "/Users/nbassey/Development/owc/http-server/public";
            int port = 5000;
            new HttpServerCore(staticPath).Run(port);
        }
    }
}
