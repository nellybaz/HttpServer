using System;
using HttpServer.Library;

namespace HttpClient.Middlewares
{
    public class BasicAuth : Middleware
    {
        public override void Run(Request request, Response response){
                  if(request.Method == RequestMethod.OPTIONS) return;
      string userName = "admin";
      string password = "hunter2";

      Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
      string base64 = Convert.ToBase64String(byteData);

      try
      {
        if (request.Url == "/logs")
        {
          response.Authenticate = true;
          string authenticatedPayload = request.Authorization.Split(" ")[1];

          if (base64 != authenticatedPayload)
          {
            response.SetStatus(StatusCode._401);
            response.Halt();
          }
          else
          {
            request.Authenticated = true;
          }
        }
      }
      catch (System.Exception)
      {
        response.SetStatus(StatusCode._401);
        response.Halt();
      }
        }
  }
}