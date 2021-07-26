using System;
using Xunit;
using System.IO;
using HttpServer.Library;
using System.Net.Sockets;
using System.Threading;

namespace HttpServer.Test
{
  public class HttpServerTest
  {

    [Fact]
    public void Run_Writes_Message_On_TCP_Connection()
    {

      int port = 5050;
      string server = "127.0.0.1";

      var output = new StringWriter();
      Console.SetOut(output);

      Thread tcpThread = new Thread(() => { new HttpServerCore().Run(port); });
      tcpThread.Start();

      Thread.Sleep(2000);
      TcpClient client = new TcpClient(server, port);

      NetworkStream clientStream = client.GetStream();
      Byte[] message = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(message, 0, message.Length);
      clientStream.Close();

      Assert.Equal("Server listening on port: " + port + "\nTCP connection received\n", output.ToString());
    }

    [Fact]
    public void GetStreamData_Returns_Correct_Message_From_Strem()
    {

      string message = "GET, hi there\n";

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message);

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      string expectedMessage = HttpServerCore.GetStreamData(clientStream);
      Assert.Equal(message, expectedMessage);
    }

    [Fact(Skip = "relook")]
    public void HandleRequest_Returns_OK_Response_For_Valid_Get_Request()
    {

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(byteMessage, 0, byteMessage.Length);

      var httpServerCore = new HttpServerCore();
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);


      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = "200 OK";
      Assert.Contains(status, actual);

    }

    [Fact(Skip = "Find a way to read right files on test")]
    public void HandleRequest_Returns_404_When_Path_Not_Found()
    {
      Stream clientStream = new MemoryStream(256);
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet("/not-found"));

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      var httpServerCore = new HttpServerCore();
      string actual0 = HttpServerCore.GetStreamData(clientStream);
      Assert.Contains("GET", actual0);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);

      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = "404 Not found";
      Assert.Contains(status, actual);
    }
  }
}
