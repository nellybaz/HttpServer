using System;

namespace HttpServer.Library
{
  public class App
  {
    public string StaticPath;

    public void BasicAuth(string userName, string password, Request request, Response response)
    {
      try
      {
        Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
        string base64 = Convert.ToBase64String(byteData);
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
      catch (System.Exception)
      {
        response.SetStatus(StatusCode._401);
        response.Halt();
      }
    }
  }
}