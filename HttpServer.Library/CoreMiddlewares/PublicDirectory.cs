using System;
using System.IO;

namespace HttpServer.Library.CoreMiddlewares
{
  public class PublicDirectory : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      response.SetBody("");
      if (request.Url == "/") return;
      try
      {
        string path = request.App.StaticPath + request.Url;
        Byte[] byteData = File.ReadAllBytes(path);
        response.SetBody(byteData);
        response.Mime = MimeType.GetMimeType(request.Url);
        request.IsPath = true;
      }
      catch (System.Exception)
      {
        string message = "<html><h2>Page not found</h2></html>";
        response.SetBody(message);
        response.Status = StatusCode._404;
      }

      if (request.IsPath && request.Method == RequestMethod.POST)
      {
        response.Status = StatusCode._405;
        response.SetBody("");
      }
    }
  }
}