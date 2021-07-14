using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;

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

        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("TCP connection received");

        string streamData = GetStreamData(client.GetStream());
        client.Close();
        Console.WriteLine(streamData);

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

    public string GetStreamData(Stream stream)
    {
      int i;
      Byte[] bytes = new Byte[256];
      string data = null;
      while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
      {
        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
        Console.WriteLine("Received: {0}", data);
      }
      return data;

    }
  }
}
