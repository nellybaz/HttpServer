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

      string userName = "admin";
      string password = "hunter2";
      string[] urls = { "/logs" };

      httpServer.SetBasicAuth(urls, userName, password);
      httpServer.Run(port);
    }
  }
}
