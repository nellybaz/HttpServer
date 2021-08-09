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
            HttpServerCore httpServer = new HttpServerCore(staticPath);
            httpServer.AddMiddleWare(new Middlewares.DefaultBody().Run);
            httpServer.AddMiddleWare(new Middlewares.BasicAuth().Run);

            // string userName = "";
            // string password = "";
            // httpServer.BasicAuth("/logs", userName, password);
            httpServer.Run(port);
        }
    }
}
