using System;
using System.IO;
using HttpServer.Library;
using Xunit;

namespace HttpServer.Test
{
  public class ResponseTest
  {
    [Fact]
    public void Headers_Formatted_According_To_Standard()
    {
      //Given
      Response response = new Response();
      //When
      string status = "200 OK";
      string version = "HTTP/1.1";
      string mime = "text/html;";
      string encoding = "charset=utf-8";

      string headers = "";
      headers = headers + version + " " + status + "\n";
      headers = headers + "Server: XHTTPServer\n";
      headers = headers + "Content-Type: " + mime + " " + encoding + "\n";
      headers = headers + "Accept-Ranges: bytes\n";
      headers = headers + "Content-Length: " + "0" + "\n\n";

      //Then
      string expected = headers;
      Assert.Equal(expected, response.Headers);
    }

    [Fact]
    public void BodyByte_Returns_Bytes_Array_From_Response_Body()
    {
      //Given
      string message = "hi there";
      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);
      //When
      var response = new Response();
      response.Body = message;
      Byte[] actual = response.BodyBytes;
      //Then
      Assert.Equal(messageByte, actual);
    }
  }
}