using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

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

    private List<Middleware> _classMiddlewares = new List<Middleware>();

    public HttpServerCore(string staticPath)
    {
      this._staticPath = staticPath;
      // this._middlewares.Add(Middlewares.BasicAuthentication);
      this._middlewares.Add(Middlewares.AllowedMethod);
      this._middlewares.Add(Middlewares.ProcessPublicDirectory);
      this._middlewares.Add(Middlewares.ProcessMethods);
      this._middlewares.Add(Middlewares.ProcessRoutes);
      this._middlewares.Add(Middlewares.ProcessPublicDirectoryRestrictions);
      this._middlewares.Add(Middlewares.ProcessRanges);
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
      return data;
    }

    public void HandleRequest(Stream stream)
    {
      string dataFromStream = GetStreamData(stream);
      Request request = new Request(dataFromStream);
      request.SetStaticPath(this._staticPath);
      Response response = new Response();

      ProcessMiddleWares(this._middlewares, request, response);
      ProcessMiddleWares(this._classMiddlewares, request, response);

      HttpServerWorker httpServerWorker = new HttpServerWorker(stream, request, response);
      httpServerWorker.Write();
    }
    public void ProcessMiddleWares(List<Action<Request, Response>> middlewares, Request request, Response response)
    {
      foreach (var action in middlewares)
      {
        if (!response.Halted)
          action(request, response);
      }
    }

    public void ProcessMiddleWares(List<Middleware> middlewares, Request request, Response response)
    {
      foreach (var action in middlewares)
      {
        if (!response.Halted)
          action.Run(request, response);
      }
    }

    public void ProcessMiddleWares(Request request, Response response)
    {

      foreach (var action in this._middlewares)
      {
        if (!response.Halted)
          action(request, response);
      }

      foreach (var action in this._classMiddlewares)
      {
        if (!response.Halted)
          action.Run(request, response);
      }
    }

    public void AddMiddleWare(Action<Request, Response> middleware)
    {
      this._middlewares.Add(middleware);
    }

    public void AddMiddleWare(Middleware middleware)
    {
      _classMiddlewares.Add(middleware);
    }
  }
}

