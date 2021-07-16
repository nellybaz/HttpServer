using System;
using System.IO;
using HttpServer.Library;
using Xunit;

namespace HttpServer.Test
{
  public class RequestTest
  {

    [Fact]
    public void Request_Returns_Correct_Url_From_Connection()
    {

      string route = "/home";
      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(Request.SampleGet(route));


      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);

      string actual = new Request(clientStream).Url;
      Assert.Equal(route, actual);
    }
  }
}