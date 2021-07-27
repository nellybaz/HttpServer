using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

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

    public string GeMessageFromPath(Request request, Response response)
    {
      var validPath = new Dictionary<string, string>{
        {"/", "/index"},
        {"/file1", "/file1"},
        {"/file2", "/file2"},
      };
      string message = "";

      bool pathIsInvalid = !validPath.ContainsKey(request.Url);

      try
      {
        if (pathIsInvalid)
        {
          string path = Directory.GetCurrentDirectory() + "/public" + "/404.html";
          message = File.ReadAllText(path);
          response.Mime = MimeType.html;
          response.Status = StatusCode._404;
        }
        else
        {
          string path = Directory.GetCurrentDirectory() + "/public" + validPath[request.Url];
          message = File.ReadAllText(path);
          response.Mime = MimeType.plainText;
        }
      }
      catch (System.Exception e)
      {
        Console.WriteLine(e);
        message = "<html><h2>Page not found</h2></html>";
      }

      return message;
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

      string message = GeMessageFromPath(request, response);

      Byte[] messageByte = BytesFromArray(message);

      response.ContentLength = messageByte.Length;

      Byte[] headersByte = BytesFromArray(response.Headers);
      stream.Write(headersByte, 0, headersByte.Length);

      if (request.Method != RequesetMethod.HEAD)
      {
        stream.Write(messageByte, 0, messageByte.Length);
      }
    }
  }
}
