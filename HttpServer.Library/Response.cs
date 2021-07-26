using System;
using System.IO;

namespace HttpServer.Library
{
  public class Response
  {
    Stream stream;
    string status;
    public void Send(string message)
    {
      
      Byte[] messageByte = System.Text.Encoding.ASCII.GetBytes(message);
      WriteHeader(message.Length);
      stream.Write(messageByte, 0, messageByte.Length);
    }


    public Response(Stream _stream, string _status)
    {
      stream = _stream;
      status = _status;
    }
    private void WriteHeader(int contentLength)
    {
      string status = this.status;
      string version = "HTTP/1.1";
      string mime = "text/html; ";
      string encoding = "charset=utf-8";

      string headers = "";
      headers = headers + version + " " + status + "\r\n";
      headers = headers + "Server: cx1193719-b\r\n";
      headers = headers + "Content-Type: " + mime + encoding + "\r\n";
      headers = headers + "Accept-Ranges: bytes\r\n";
      headers = headers + "Content-Length: " + contentLength + "\r\n\r\n";

      Byte[] headersByte = System.Text.Encoding.ASCII.GetBytes(headers);
      stream.Write(headersByte, 0, headersByte.Length);
    }
  }
}