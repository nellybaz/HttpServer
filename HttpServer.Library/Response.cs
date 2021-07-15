using System;
using System.IO;

namespace HttpServer.Library
{
  public class Response
  {
    Stream stream;
    public void Send(string message)
    {
      WriteHeader();
      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);
      stream.Write(messageByte, 0, messageByte.Length);
    }


    public Response(Stream _stream)
    {
      stream = _stream;
    }
    private void WriteHeader()
    {
      string status = "200 OK";
      string version = "HTTP/1.1";
      string mime = "text/html";

      string headers = "";
      headers = headers + version + " " + status + "\r\n";
      headers = headers + "Server: cx1193719-b\r\n";
      headers = headers + "Content-Type: " + mime + "\r\n";
      headers = headers + "Accept-Ranges: bytes\r\n";
      headers = headers + "Content-Length: " + 50 + "\r\n\r\n";

      Byte[] headersByte = System.Text.Encoding.ASCII.GetBytes(headers);
      stream.Write(headersByte, 0, headersByte.Length);
    }
  }
}