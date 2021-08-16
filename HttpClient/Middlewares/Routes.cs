using System;
using System.Collections.Generic;
using System.IO;
using HttpServer.Library;

namespace HttpClient.Middlewares
{
  public class Routes : IMiddleware
  {
    
    public void Run(Request request, Response response)
    {
      List<string> routes = new List<string>();
      routes.Add("/");
      routes.Add("/logs");
      routes.Add("/cat-form");

      if (Array.Exists(routes.ToArray(), (url) => request.Url == url))
      {
        request.IsRoute = true;
        response.SetStatus(StatusCode._200);
      }
    }
  }
}