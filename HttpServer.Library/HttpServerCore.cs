using System;
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
        server.Stop();
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
      var validPath = new Dictionary<string, bool>{
        {"/", true},
        {"/file1", true},
        {"/file2", true}
      };

      var request = new Request(stream);
      string status = Status._200;
      string message = "";

      bool pathIsInvalid = !validPath.ContainsKey(request.Url);

      if (pathIsInvalid)
      {
        status = Status._404;
        message = "Page not found";
      }
      else
      {
        try
        {
          string fileForUrl = request.Url;
          if (request.Url == "/")
          {
            fileForUrl = "/index";
          }
          string path = Directory.GetCurrentDirectory() + "/public" + fileForUrl + ".html";
          message = File.ReadAllText(path);
        }
        catch (System.Exception _)
        {
          message = "Page not found";
        }

      }
      new Response(stream, status).Send(message);
    }
  }
}
