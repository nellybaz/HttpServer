using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpServer.Library
{
  public class Middlewares
  {

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
  }
}