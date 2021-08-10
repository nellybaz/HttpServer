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

      if (Array.Exists(routes.ToArray(), (url) => request.Url == url))
      {
        request.IsRoute = true;
        response.SetStatus(StatusCode._200);
      }

      if (request.Url == "/" && request.Method == RequestMethod.GET)
      {
        string links = "";
        string[] files = Directory.GetFiles(request.App.StaticPath);
        foreach (var file in files)
        {
          string url = file.Split("public")[1];
          links += $"<a href='{url}'>{url}</a></br>";
        }
        string body = $"<html>{links}</html>";
        response.SetStatus(StatusCode._200);
        response.SetBody(body);
        return;
      }

      if (request.Url == "/logs")
      {
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