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
        {"/", "/index.html"},
        {"/file1", "/file1.html"},
        {"/file2", "/file2.html"},
      };

      string dataFromStream = GetStreamData(stream);
      // Console.WriteLine(dataFromStream);
      var request = new Request(dataFromStream);
      string status = StatusCode._200;
      string message = "";

      bool pathIsInvalid = !validPath.ContainsKey(request.Url);

      try
      {
        if (pathIsInvalid)
        {
          string path = Directory.GetCurrentDirectory() + "/public" + "/404.html";
          status = StatusCode._404;
          message = File.ReadAllText(path);
        }
        else
        {
          string path = Directory.GetCurrentDirectory() + "/public" + validPath[request.Url];
          message = File.ReadAllText(path);
        }
      }
      catch (System.Exception)
      {
        status = StatusCode._404;
        message = "<html><h2>Page not found</h2></html>";
      }

      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);

      Response response = new Response();
      response.ContentLength = messageByte.Length;
      string headers = response.Headers;

      // Console.WriteLine(headers);
      Byte[] headersByte = System.Text.Encoding.ASCII.GetBytes(headers);
      stream.Write(headersByte, 0, headersByte.Length);

      
      stream.Write(messageByte, 0, messageByte.Length);
    }
  }
}
