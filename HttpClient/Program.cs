﻿using System;
using System.Collections.Generic;
using System.IO;
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

      httpServer.Route("GET", "/", new HomeController().Run);
      httpServer.Route("*", "/logs", new LogsController().Run);
      httpServer.Route("*", "/cat-form", new CatFormController().Run);
      httpServer.Route("GET", "/eat_cookie", new CookieController().Run);
      httpServer.Route("GET", "/cookie", new CookieController().Run2);
      httpServer.Route("GET", "/parameter", new ParameterController().Run);
      httpServer.Route("GET", "/redirect", new RedirectController().Run);
      httpServer.Route("GET", "/tea", new CoffeeTeaController().Tea);
      httpServer.Route("GET", "/coffee", new CoffeeTeaController().Run);

      httpServer.Run(port);
    }
  }
}
