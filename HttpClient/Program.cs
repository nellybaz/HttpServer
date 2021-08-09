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
            HttpServerCore httpServerCore = new HttpServerCore(staticPath);
            httpServerCore.AddMiddleWare(new Middlewares.DefaultBody().Run);
            httpServerCore.AddMiddleWare(new Middlewares.BasicAuth().Run);
            httpServerCore.Run(port);
        }
    }
}
