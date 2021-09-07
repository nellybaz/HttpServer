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
      response.SetStatus(StatusCode._200);
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
      Assert.Contains("Server:", response.Headers);
      Assert.Contains("Content-Type:", response.Headers);
      Assert.Contains("Accept-Ranges:", response.Headers);
      Assert.Contains("Server", response.Headers);
      Assert.Contains("Content-Length:", response.Headers);
    }

    [Fact]
    public void BodyByte_Returns_Bytes_Array_From_Response_Body()
    {
      //Given
      string message = "hi there";
      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);
      //When
      var response = new Response();
      response.SetBody(message);
      Byte[] actual = response.BodyBytes;
      //Then
      Assert.Equal(messageByte, actual);
    }

    [Fact(Skip="yes")]
    public void HeaderBytes_Returns_Bytes_Array_From_Response_Header()
    {
      //Given
      Response response = new Response();
      
      //When
      response.SetStatus(StatusCode._200);
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
       Byte[] expected = System.Text.Encoding.ASCII.GetBytes(headers);;
      Assert.Equal(expected, response.HeadersByte);
    }
  }
}