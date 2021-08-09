using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpServer.Library
{
  public class Middlewares
  {
    public static void AllowedMethod(Request request, Response response)
    {
      if (!RequestMethod.IsValid(request.Method))
      {
        response.Status = StatusCode._501;
      }
    }

    public static void ProcessPublicDirectoryRestrictions(Request request, Response response)
    {
      if (request.IsPath && request.Method == RequestMethod.POST)
      {
        response.Status = StatusCode._405;
        response.SetBody("");
      }
    }

    public static void ProcessRoutes(Request request, Response response)
    {
      var protectedPath = new Dictionary<String, String>();
      protectedPath.Add("/logs", "GET, HEAD, OPTIONS");

      if (request.Method == RequestMethod.OPTIONS && protectedPath.ContainsKey(request.Url))
      {
        response.Methods = protectedPath[request.Url];
        response.Status = StatusCode._200;
        return;
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
        response.SetBody(body);
        return;
      }

      if (request.Url == "/logs")
      {
        // BasicAuthentication(request, response);
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

    public static void ProcessRanges(Request request, Response response)
    {
      if (request.Range != null)
      {
        string[] rangeSplit = request.Range.Split("-");
        int startRange = Int32.Parse(rangeSplit[0]);
        int endRange = Int32.Parse(rangeSplit[1]);
        Byte[] newByteData = new Byte[startRange + endRange + 1];

        Byte[] currentByteData = response.BodyBytes;
        int newByteIndex = 0;
        for (int i = startRange; i <= endRange; i++)
        {
          newByteData[newByteIndex] = currentByteData[i];
          newByteIndex++;
        }

        response.SetBody(newByteData);
        response.SetStatus(StatusCode._206);
      }
    }

    public static void ProcessMethods(Request request, Response response)
    {
      if (request.Method == RequestMethod.OPTIONS)
      {
        response.Methods = "GET, HEAD, OPTIONS, PUT, DELETE";
        response.Status = StatusCode._200;
      }
      if (request.Method == RequestMethod.HEAD) response.SetBody("");

      string path = request.App.StaticPath + request.Url;

      if (request.Method == RequestMethod.PUT)
      {
        using (FileStream fs = File.Create(path))
        {
          byte[] info = new UTF8Encoding(true).GetBytes(request.Body);
          fs.Write(info, 0, info.Length);

          if (request.IsPath)
          {
            response.SetStatus(StatusCode._200);
            response.SetBody("Updated");
          }
          else
          {
            response.SetStatus(StatusCode._201);
            response.SetBody("Created");
          }
        }
      }
      if (request.Method == RequestMethod.DELETE && request.IsPath)
      {
        File.Delete(path);
        response.SetStatus(StatusCode._200);
        response.SetBody("Deleted");
      }
    }

    public static void ProcessPublicDirectory(Request request, Response response)
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
    }
  }
}