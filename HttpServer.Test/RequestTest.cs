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
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet(route));


      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);

      string dataFromStream = HttpServerCore.GetStreamData(clientStream);
      string actual = new Request(dataFromStream).Url;
      Assert.Equal(route, actual);
    }

    [Fact]
    public void Request_Returns_Method()
    {
      //Given
      string route = "/home";
      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet(route));


      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);

      string dataFromStream = HttpServerCore.GetStreamData(clientStream);

      //When
      string actual = new Request(dataFromStream).Method;

      //Then
      Assert.Equal("GET", actual);
    }
  }
}