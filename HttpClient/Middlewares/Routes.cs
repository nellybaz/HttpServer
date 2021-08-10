using System.IO;
using HttpServer.Library;

namespace HttpClient.Middlewares
{
  public class Routes : IMiddleware
  {
    public void Run(Request request, Response response)
    {

      if (request.Url == "/" && request.Method == RequestMethod.GET)
      {
        request.IsRoute = true;
        string links = "";
        string[] files = Directory.GetFiles(request.App.StaticPath);
        foreach (var file in files)
        {
          string url = file.Split("public")[1];
          links += $"<a href='{url}'>{url}</a></br>";
        }
        string body = $"<html>{links}</html>";
        response.SetBody(body);
        return;
      }

      if (request.Url == "/logs")
      {
        request.IsRoute = true;
        if (request.Authenticated)
        {
          if (request.Method == RequestMethod.POST)
          {
            response.SetStatus(StatusCode._405);
          }
          else
          {
            response.SetStatus(StatusCode._200);
          }
          response.SetBody(request.Method + " " + request.Url + " " + response.Version);
          response.Mime = MimeType.PlainText;
          response.Halt();
        }
      }
    }
  }
}