﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Net.Mime;

namespace HttpServer.Library
{
  public class HttpServerCore
  {
    private string _staticPath;

    string StaticPath
    {
      get => _staticPath;
    }
    public HttpServerCore(string staticPath)
    {
      this._staticPath = staticPath;
    }


    public void Run(int port)
    {
      TcpListener server = null;

      Console.WriteLine("Server listening on port: " + port);
      try
      {

        server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        server.Start();

        while (true)
        {
          TcpClient client = server.AcceptTcpClient();
          Console.WriteLine("TCP connection received");

          var stream = client.GetStream();
          HandleRequest(stream);
          client.Close();
        }
        // server.Stop();
      }
      catch (SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e);
      }
      finally
      {
        server.Stop();
      }

    }

    public static string GetStreamData(Stream stream)
    {

      String data = null;
      StreamReader reader = new StreamReader(stream);
      while (reader.Peek() != -1)
      {
        data += reader.ReadLine() + "\n";
      }
      return data;
    }

    public void ProcessPublicDirectory(Request request, Response response)
    {
      response.SetBody("");
      if (request.Url == "/") return;
      try
      {
        string path = this._staticPath + request.Url;
        Byte[] byteData = File.ReadAllBytes(path);
        response.SetBody(byteData);
        response.Mime = GetMimeType(request.Url);
        // request.IsPath = true;
      }
      catch (System.Exception)
      {
        string message = "<html><h2>Page not found</h2></html>";
        response.SetBody(message);
        response.Status = StatusCode._404;
      } 
    }

    private Byte[] BytesFromArray(string data)
    {
      return System.Text.Encoding.ASCII.GetBytes(data);
    }
    public void HandleRequest(Stream stream)
    {

      // dataFromBytes | tokens [path, method] -> Request ->  verifyPath | Route -> handleMethods -> response -> middlewares -> bytesFromData

      List<Action<Request, Response>> middlewares = new List<Action<Request, Response>>();
      middlewares.Add(ProcessPublicDirectory);
      middlewares.Add(ProcessMethods);
      middlewares.Add(ProcessRoutes);

      string dataFromStream = GetStreamData(stream);
      Request request = new Request(dataFromStream);
      Response response = new Response();

      ProcessMiddleWares(middlewares, request, response);

      HttpServerWorker httpServerWorker = new HttpServerWorker(stream, request, response);
      httpServerWorker.Write();

    }

    public void ProcessMethods(Request request, Response response)
    {
      if (request.Method == RequesetMethod.OPTIONS)
      {
        response.Methods = "GET, HEAD, OPTIONS, PUT, DELETE";
        response.Status = StatusCode._200;
      }
      if (request.Method == RequesetMethod.HEAD) response.SetBody("");
    }

    public void ProcessMiddleWares(List<Action<Request, Response>> middlewares, Request request, Response response)
    {
      foreach (var action in middlewares)
      {
        action(request, response);
      }
    }

    public void ProcessRoutes(Request request, Response response)
    {
      var protectedPath = new Dictionary<String, String>();
      protectedPath.Add("/logs", "GET, HEAD, OPTIONS");

      if (request.Method == RequesetMethod.OPTIONS && protectedPath.ContainsKey(request.Url))
      {
        response.Methods = protectedPath[request.Url];
        response.Status = StatusCode._200;
      }

      if (request.Url == "/" && request.Method == RequesetMethod.GET)
      {
        string body = "<html><a href='http://localhost:5000/file1'>file1</a></html>";
        response.SetBody(body);
      }
    }

    public string GetMimeType(string path)
    {
      try
      {
        string extension = path.Split(".")[1];
        Dictionary<string, string> mimeHash = new Dictionary<string, string> {
        { "", MimeType.PlainText },
        {"jpeg", MimeType.Jpeg},
        {"html", MimeType.Html},
        {"txt", MimeType.PlainText}
        };

        return mimeHash[extension];
      }
      catch (System.Exception)
      {
        return MimeType.PlainText;
      }
    }
  }

  class HttpServerWorker
  {
    private Stream stream;
    public Request request;

    public Response response;

    public HttpServerWorker(Stream stream, Request request, Response response)
    {
      this.stream = stream;
      this.request = request;
      this.response = response;
    }

    public void Write()
    {
      stream.Write(response.HeadersByte, 0, response.HeadersByte.Length);
      stream.Write(response.BodyBytes, 0, response.BodyBytes.Length);
    }

  }
}
