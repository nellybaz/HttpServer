using System;
using System.IO;

namespace HttpServer.Library.CoreMiddlewares
{
  public class PublicDirectory : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      if (request.IsRoute) return;

      response.SetBody("");

      try
      {
        string path = request.App.StaticPath + request.Url;
        Byte[] byteData = File.ReadAllBytes(path);
        response.SetStatus(StatusCode._200);
        response.SetBody(byteData);
        response.SetHeader(Response.Header.Content_Type, MimeType.GetMimeType(request.Url));
        request.IsPath = true;
      }
      catch (System.Exception)
      {
        if (response.Status == null)
        {
          string message = "<html><h2>Page not found</h2></html>";
          response.SetBody(message);
          response.Status = StatusCode._404;
        }
      }

      if (request.IsPath && request.Method == RequestMethod.POST)
      {
        response.Status = StatusCode._405;
        response.SetBody("");
      }
    }
  }
}