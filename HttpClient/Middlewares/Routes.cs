using System;
using System.Collections.Generic;
using System.IO;
using HttpServer.Library;

namespace HttpClient.Middlewares
{
  public class Routes : IMiddleware
  {
    private Dictionary<string, string> data = new Dictionary<string, string>();
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

      if (request.Url.Contains("/cat-form"))
      {

        Dictionary<string, string> resources = new Dictionary<string, string>();
        if (request.Method == RequestMethod.POST)
        {
          string key = request.Body.Split("=")[0];
          string value = request.Body.Split("=")[1];
          data.Add(key, value);
          response.SetStatus(StatusCode._201);
          string location = request.Url + "/" + key;
          response.SetLocation(location);
        }

        if (request.Method == RequestMethod.GET && request.Url.Contains("data"))
        {
          if (this.data.ContainsKey("data"))
          {
            string body = this.data["data"];
            response.SetBody($"data={body}");
            response.SetStatus(StatusCode._200);
          }
          else
          {
            response.SetStatus(StatusCode._404);
          }
        }

        if (request.Method == RequestMethod.PUT && request.Url.Contains("data"))
        {
          string key = request.Body.Split("=")[0];
          string value = request.Body.Split("=")[1];
          data[key] = value;
          response.SetStatus(StatusCode._200);
        }

        if (request.Method == RequestMethod.DELETE && request.Url.Contains("data"))
        {
          data.Remove("data");
          response.SetStatus(StatusCode._200);
        }
      }
    }
  }
}