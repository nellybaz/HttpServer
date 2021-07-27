using System;
using System.IO;
using HttpServer.Library;
using Xunit;

namespace HttpServer.Test
{
  public class ResponseTest
  {
    [Fact(Skip = "Writing to stream no more a property of response")]
    public void Send_Writes_Valid_Headers_With_Message_To_The_Stream()
    {
      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      string message = "Ok";
      // new Response(clientStream, StatusCode._200).Send(message);

      string status = "200 OK";
      string version = "HTTP/1.1";
      string mime = "text/html";


      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      Assert.Contains(status, actual);
      Assert.Contains(version, actual);
      Assert.Contains(mime, actual);
      Assert.Contains(message, actual);
    }
  }
}