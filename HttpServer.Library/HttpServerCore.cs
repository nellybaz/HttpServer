﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace HttpServer.Library
{
  public class HttpServerCore
  {
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

    public void HandleRequest(Stream stream)
    {

      // dataFromBytes | tokens [path, method] -> Request ->  verifyPath | Route -> handleMethods -> response -> middlewares -> bytesFromData

      var validPath = new Dictionary<string, string>{
        {"/", "/index"},
        {"/file1", "/file1"},
        {"/file2", "/file2"},
      };

      string dataFromStream = GetStreamData(stream);
      Request request = new Request(dataFromStream);
      Response response = new Response();

      string message = "";

      bool pathIsInvalid = !validPath.ContainsKey(request.Url);

      try
      {
        if (pathIsInvalid)
        {
          string path = Directory.GetCurrentDirectory() + "/public" + "/404.html";
          message = File.ReadAllText(path);
          response.Mime = MimeTypes.html;
        }
        else
        {
          string path = Directory.GetCurrentDirectory() + "/public" + validPath[request.Url];
          message = File.ReadAllText(path);
          response.Mime = MimeTypes.plainText;
        }
      }
      catch (System.Exception)
      {
        message = "<html><h2>Page not found</h2></html>";
      }

      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);

      response.ContentLength = messageByte.Length;
      string headers = response.Headers;

      Byte[] headersByte = System.Text.Encoding.ASCII.GetBytes(headers);
      stream.Write(headersByte, 0, headersByte.Length);

      stream.Write(messageByte, 0, messageByte.Length);
    }
  }
}
