using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HttpServer.Library.CoreMiddlewares
{
  public class HttpMethods : IMiddleware
  {
    private Dictionary<String, String> allowedMethods = new Dictionary<String, String>();
    private byte[] _currentFileContent = new Byte[600];

    public HttpMethods() { }
    public HttpMethods(Dictionary<String, String> allowedMethods)
    {
      this.allowedMethods = allowedMethods;
    }

    public void Run(Request request, Response response)
    {
      if (!RequestMethod.IsValid(request.Method))
      {
        response.Status = StatusCode._501;
      }

      if (request.Method == RequestMethod.OPTIONS)
      {
        response.Methods = "GET, HEAD, OPTIONS, PUT, DELETE";
        response.Status = StatusCode._200;
      }

      SetAllowedMethods(request, response);

      if (request.Method == RequestMethod.HEAD && (request.IsPath || request.IsRoute))
      {
        response.SetStatus(StatusCode._200);
        response.SetBody("");
      }

      string path = request.App.StaticPath + request.Url;

      if (MethodAllowsUpdate(request))
      {
        try
        {
          this._currentFileContent = File.ReadAllBytes(path);
        }
        catch (System.Exception){}

        try
        {
          using (FileStream fs = File.OpenWrite(path))
          {
            byte[] info = new UTF8Encoding(true).GetBytes(request.Body);
            WriteToFile(request, response, fs, info);

            if (request.IsPath)
            {
              SetStatusCode(request, response);
              response.SetBody("Updated");
            }
            else
            {
              response.SetStatus(StatusCode._201);
              response.SetBody("Created");
            }
          }
        }
        catch (System.Exception)
        {
          // TODO
          response.SetStatus(StatusCode._501);
        }
      }
      if (request.Method == RequestMethod.DELETE && request.IsPath)
      {
        File.Delete(path);
        response.SetStatus(StatusCode._200);
        response.SetBody("Deleted");
      }
    }

    private void WriteToFile(Request request, Response response, FileStream fs, byte[] info)
    {
      if (request.Method == RequestMethod.PATCH)
      {
        if (ValidEtag(request))
        {
          fs.Write(info, 0, info.Length);
          response.SetStatus(StatusCode._204);
        }
        else
        {
          response.SetStatus(StatusCode._412);
        }
      }
      else
      {
        fs.Write(info, 0, info.Length);
      }

    }

    private bool ValidEtag(Request request)
    {
      string etag = request.Etag;
      string currentFileContent = SHA1HashedContent(this._currentFileContent);
      return etag.ToLower() == currentFileContent.ToLower();
    }

    private string SHA1HashedContent(byte[] byteData)
    {
      try
      {
        var hashedByte = SHA1.HashData(byteData);
        return BitConverter.ToString(hashedByte).Replace("-", String.Empty);
      }
      catch (System.Exception)
      {
        return "";
      }
    }

    private void SetStatusCode(Request request, Response response)
    {
      if (request.Method != RequestMethod.PATCH)
      {
        response.SetStatus(StatusCode._200);
      }

    }

    private void SetAllowedMethods(Request request, Response response)
    {
      if (request.Method == RequestMethod.OPTIONS && this.allowedMethods.ContainsKey(request.Url))
      {
        response.Methods = this.allowedMethods[request.Url];
        response.Status = StatusCode._200;
        return;
      }
    }

    private bool MethodAllowsUpdate(Request request)
    {
      return request.Method == RequestMethod.PUT || request.Method == RequestMethod.PATCH;
    }
  }
}