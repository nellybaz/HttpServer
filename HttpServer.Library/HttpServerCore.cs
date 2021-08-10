﻿using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using HttpServer.Library.CoreMiddlewares;

namespace HttpServer.Library
{
  public class HttpServerCore
  {
    private string _staticPath;

    string StaticPath
    {
      get => _staticPath;
    }

    private List<IMiddleware> _middlewares = new List<IMiddleware>();
    private List<IMiddleware> _coreMiddlewares = new List<IMiddleware>();

    private IMiddleware _basicAuthMiddleware = new BasicAuthentication();
    private IMiddleware _httpMethodsMiddleware = new HttpMethods();

    public HttpServerCore(string staticPath)
    {
      this._staticPath = staticPath;
    }

    public void SetAllowedMethods(Dictionary<string, string> allowedMethods)
    {
      this._httpMethodsMiddleware = new HttpMethods(allowedMethods);
    }

    // default of no-auth
    public void SetBasicAuth(string[] urls, string userName, string password)
    {
      this._basicAuthMiddleware = new BasicAuthentication(urls, userName, password);
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

      ProcessMiddleWares(request, response);

      HttpServerWorker httpServerWorker = new HttpServerWorker(stream, request, response);
      httpServerWorker.Write();
    }
    public void ProcessMiddleWares(Request request, Response response)
    {
      RegisterMiddleWares();

      foreach (var action in this._coreMiddlewares)
      {
        if (!response.Halted)
          action.Run(request, response);
      }

      foreach (var action in this._middlewares)
      {
        if (!response.Halted)
          action.Run(request, response);
      }
    }

    private void RegisterMiddleWares()
    {
      if (this._coreMiddlewares.ToArray().Length < 1)
      {
        this._coreMiddlewares.Add(this._basicAuthMiddleware);
        this._coreMiddlewares.Add(new PublicDirectory());
        this._coreMiddlewares.Add(this._httpMethodsMiddleware);
      }

    }

    public void AddMiddleWare(IMiddleware middleware)
    {
      _middlewares.Add(middleware);
    }
  }
}

