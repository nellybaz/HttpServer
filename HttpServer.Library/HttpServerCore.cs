using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Library
{
  public class HttpServerCore
  {
    private string _staticPath;

    string StaticPath
    {
      get => _staticPath;
    }

    private List<Action<Request, Response>> _middlewares = new List<Action<Request, Response>>();
    public HttpServerCore(string staticPath)
    {
      this._staticPath = staticPath;
      this._middlewares.Add(AllowedMethod);
      // this._middlewares.Add(BasicAuthentication);
      this._middlewares.Add(ProcessPublicDirectory);
      this._middlewares.Add(ProcessMethods);
      this._middlewares.Add(ProcessRoutes);
      this._middlewares.Add(ProcessPublicDirectoryRestrictions);
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

      String data = String.Empty;
      StreamReader reader = new StreamReader(stream);
      Byte[] byteData = new Byte[256 * 2];

      int currentIndex = -1;
      currentIndex = stream.Read(byteData, 0, byteData.Length);
      data += System.Text.Encoding.ASCII.GetString(byteData, 0, currentIndex);
      // while (reader.Peek() != -1)
      // {
      //   data += reader.ReadLine() + "\n";
      // }
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
        request.IsPath = true;
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

      string dataFromStream = GetStreamData(stream);
      Request request = new Request(dataFromStream);
      Response response = new Response();

      ProcessMiddleWares(this._middlewares, request, response);

      HttpServerWorker httpServerWorker = new HttpServerWorker(stream, request, response);
      httpServerWorker.Write();

    }

    public void ProcessMethods(Request request, Response response)
    {
      if (request.Method == RequestMethod.OPTIONS)
      {
        response.Methods = "GET, HEAD, OPTIONS, PUT, DELETE";
        response.Status = StatusCode._200;
      }
      if (request.Method == RequestMethod.HEAD) response.SetBody("");

      if (request.Method == RequestMethod.PUT && !request.IsPath)
      {
        string path = this._staticPath + request.Url;
        using (FileStream fs = File.Create(path))
        {
          byte[] info = new UTF8Encoding(true).GetBytes(request.Body);
          fs.Write(info, 0, info.Length);
        }
      }
    }

    public void ProcessMiddleWares(List<Action<Request, Response>> middlewares, Request request, Response response)
    {
      foreach (var action in middlewares)
      {
        if (!response.Halted)
          action(request, response);
      }
    }

    public void ProcessRoutes(Request request, Response response)
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
        string[] files = Directory.GetFiles(this._staticPath);
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
        BasicAuthentication(request, response);
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

      // if (request.Url == "/requests" || request.Url == "/these")
      // {
      //   response.SetBody(request.Method + " " + request.Url + " " + response.Version);
      //   response.Mime = MimeType.PlainText;
      //   response.Halt();
      // }
    }

    public string GetMimeType(string path)
    {
      try
      {
        string extension = path.Split(".")[1];
        Dictionary<string, string> mimeHash = new Dictionary<string, string> {
        { "", MimeType.PlainText },
        {"jpeg", MimeType.Jpeg},
        {"png", MimeType.Png},
        {"gif", MimeType.Gif},
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

    public void ProcessPublicDirectoryRestrictions(Request request, Response response)
    {
      if (request.IsPath && request.Method == RequestMethod.POST)
      {
        response.Status = StatusCode._405;
        response.SetBody("");
      }
    }

    public void AllowedMethod(Request request, Response response)
    {
      if (!RequestMethod.IsValid(request.Method))
      {
        response.Status = StatusCode._501;
      }
    }

    public void AddMiddleWare(Action<Request, Response> middleware)
    {
      this._middlewares.Add(middleware);
    }

    public void BasicAuthentication(Request request, Response response)
    {
      string userName = "admin";
      string password = "hunter2";

      Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
      string base64 = Convert.ToBase64String(byteData);

      try
      {
        if (request.Url == "/logs")
        {
          response.Authenticate = true;
          string authenticatedPayload = request.Authorization.Split(" ")[1];

          if (base64 != authenticatedPayload)
          {
            response.SetStatus(StatusCode._401);
          }
          else
          {
            request.Authenticated = true;
          }
        }
      }
      catch (System.Exception)
      {
        response.SetStatus(StatusCode._401);
      }

      if (request.Url == "/logs" && request.Method != RequestMethod.OPTIONS)
      {
        response.Halt();
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
      try
      {
        stream.Write(response.HeadersByte, 0, response.HeadersByte.Length);
        stream.Write(response.BodyBytes, 0, response.BodyBytes.Length);
      }
      catch (System.Exception ex)
      {
        // TODO
        Console.WriteLine(ex);
      }
    }

  }
}
