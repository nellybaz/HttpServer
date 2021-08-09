using System;
using HttpServer.Library;

namespace HttpClient.Middlewares
{
  public class BasicAuth : Middleware
  {
    public override void Run(Request request, Response response)
    {
      if (request.Method == RequestMethod.OPTIONS) return;
      string userName = "admin";
      string password = "hunter2";

      if(request.Url == "/logs") request.App.BasicAuth(userName, password, request, response);
    }
  }
}