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
    [Fact(Skip = "Connecting to actual sockets affects it")]
    public void Run_Writes_Message_When_called()
    {
      int port = 5000;
      var output = new StringWriter();
      Console.SetOut(output);

      //   new HttpServerCore().Run(port);
      Assert.Equal("Server listening on port: " + port + "\n", output.ToString());
    }

    [Fact]
    public void Run_Writes_Message_On_TCP_Connection()
    {

      int port = 5050;
      string server = "127.0.0.1";

      var output = new StringWriter();
      Console.SetOut(output);

      Thread tcpThread = new Thread(()=> {new HttpServerCore().Run(port);});
      tcpThread.Start();

      Thread.Sleep(2000);
      TcpClient client = new TcpClient(server, port);

      NetworkStream clientStream = client.GetStream();
      Byte[] message = System.Text.Encoding.ASCII.GetBytes("");

      clientStream.Write(message, 0, message.Length);
      clientStream.Close();

      Assert.Equal("Server listening on port: " + port + "\nTCP connection received\n", output.ToString());
    }

    [Fact(Skip = "")]
    public void GetStreamData_Returns_Correct_Message_From_Strem()
    {

      string message = "GET, hi there";

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message);

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      string expectedMessage = new HttpServerCore().GetStreamData(clientStream);
      Assert.Equal(message, expectedMessage);
    }
  }
}
