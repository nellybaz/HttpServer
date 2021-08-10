using System;

namespace HttpServer.Library.CoreMiddlewares
{
  public class BasicAuthentication : IMiddleware
  {
    private string[] urls;
    private string username;
    private string password;


    public BasicAuthentication()
    {
    }


    public BasicAuthentication(string[] urls, string username, string password)
    {
      this.urls = urls;
      this.username = username;
      this.password = password;

    }

    public void Run(Request request, Response response)
    {
      if (this.urls == null || this.urls.Length < 1) return;

      if (request.Method == RequestMethod.OPTIONS) return;
      string userName = this.username;
      string password = this.password;
      bool urlIsProctected = Array.Exists(this.urls, (url) => url == request.Url);

      if (urlIsProctected)
      {
        Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
        string base64 = Convert.ToBase64String(byteData);

        try
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
            response.SetStatus(StatusCode._200);
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
}