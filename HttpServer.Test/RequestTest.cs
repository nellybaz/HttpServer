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

    [Fact]
    public void Gets_Authorization_Header_Present()
    {
      //Given
      string authorization = "Basic abcdef";

      //When
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/file1", authorization));

      //Then
      Assert.Equal(authorization, request.Authorization);
    }

    [Fact]
    public void Gets_Byte_Range()
    {
      //Given
      string range = "bytes=0-4";
      //When
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      //Then
      Assert.Equal("0-4", request.Range);
    }

    [Fact]
    public void Etag_Retrieved_From_Request()
    {
    //Given
    string hash = "dc50a0d27dda2eee9f65644cd7e4c9cf11de8bec";
    
    //When
    Request request = new Request(RequestFixtures.SamplePatchRequest(hash));
    
    //Then
    Assert.Equal(hash, request.Etag);
    }
  }
}