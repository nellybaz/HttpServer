using System;
using System.Collections.Generic;
using HttpClient.Middlewares;
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
      httpServer.AddMiddleWare(new Routes());

      string userName = "admin";
      string password = "hunter2";
      string[] urls = { "/logs" };

      httpServer.SetBasicAuth(urls, userName, password);

      var allowedMethods = new Dictionary<String, String>();
      allowedMethods.Add("/logs", "GET, HEAD, OPTIONS");
      httpServer.SetAllowedMethods(allowedMethods);
      
      httpServer.Run(port);
    }
  }
}
