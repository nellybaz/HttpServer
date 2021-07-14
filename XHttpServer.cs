//using System;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//namespace http_server
//{
//  class HttpServer.Library
//  {
//    private TcpListener listener;
//    private bool serverRunning;
//    public static string host;
//    public static string version;


//    public HttpServer.Library(int port)
//    {
//      serverRunning = false;
//      listener = new TcpListener(IPAddress.Any, port);
//      host = "http://localhost:" + port;
//      version = "HTTP/1.1";
//    }
//    public void Start()
//    {
//      listener.Start();

//      serverRunning = true;
//      while (serverRunning)
//      {
//        Console.WriteLine("Waiting for connection");
//        TcpClient tcpClient = listener.AcceptTcpClient();
//        Console.WriteLine("Connected to client");
//        HandleConnection(tcpClient);
//        tcpClient.Close();
//      }

//      serverRunning = false;
//      listener.Stop();
//    }


//    public void HandleConnection(TcpClient client)
//    {
//      Stream stream = client.GetStream();
//      StreamReader reader = new StreamReader(client.GetStream());
//      String message = "";
//      while (reader.Peek() != -1)
//      {
//        message += reader.ReadLine() + "\n";
//      }


//      string[] tokens = message.Split(" ");
//      string status = "200 OK";
//      string page = @"
//          <html><h1>Ok healty</h1></html>
//        ";

//      foreach (var item in tokens)
//      {
//        Console.WriteLine(item.ToString());
//      }

//      string requestType = tokens[0];
//      string url = tokens[1];
//      if (string.Compare("/", url) != 0)
//      {
//        page = @"
//          <html><h1>Page not found</h1></html>
//        ";
//        status = "404 Not Found";
//      }

//      if (string.Compare("GET", requestType) != 0)
//      {
//        page = @"
//          <html><h1>Unexpected Server Error</h1></html>
//        ";
//        status = "501 Not Implemented";
//      }
//      NetworkStream netStream = client.GetStream();

//      if (netStream.CanWrite)
//      {
//        Console.WriteLine("You can write data to this stream.");
//        string mime = "text/html";

//        string sBuffer = "";
//        sBuffer = sBuffer + HttpServer.Library.version + " " + status + "\r\n";
//        sBuffer = sBuffer + "Server: cx1193719-b\r\n";
//        sBuffer = sBuffer + "Content-Type: " + mime + "\r\n";
//        sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
//        sBuffer = sBuffer + "Content-Length: " + 50 + "\r\n\r\n";


//        Byte[] sendBytes = Encoding.UTF8.GetBytes(sBuffer);
//        Byte[] body = Encoding.UTF8.GetBytes(page);


//        Console.WriteLine(sBuffer);
//        netStream.Write(sendBytes, 0, sendBytes.Length);
//        netStream.Write(body, 0, body.Length);
//      }
//      else
//      {
//        Console.WriteLine("You cannot write data to this stream.");
//        client.Close();
//        netStream.Close();
//      }
//    }
//  }
//}
