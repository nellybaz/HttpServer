using System;
using System.IO;

namespace HttpServer.Library
{
  public class Response
  {
    private string newLine = Environment.NewLine;
    private string _status;
    public string Status{
      get => _status;
      set => _status = value;
    }
    

    private string _mime;
    public string Mime{
      get => _mime;
      set => _mime = value;
    }

    private int _contentLength;
    public int ContentLength{
      get => _contentLength;
      set => _contentLength = value;
    }

    private string _headers;
    public string Headers{
      get => 
      $@"{_version} {_status}{newLine};
      Server: {_server}{newLine};
      Content-Type: {_mime} {_encoding}{newLine};
      Accept-Ranges: bytes{newLine}
      Content-Length: {_contentLength}{newLine}{newLine}";
    }

    private string _version;
    private string _encoding;
    private string _server;


    // public Response(string status)
    // {
    //   _status = status;
    //   _mime = "text/html; ";
    //   _version = "HTTP/1.1";
    // }

    public Response()
    {
      _status = StatusCode._200;
      _mime = "text/html;";
      _version = "HTTP/1.1";
      _encoding = "charset=utf-8";
      _contentLength = 0;
      _server = "XHTTPServer";
    }
    // private void WriteHeader(int contentLength)
    // {
    //   string status = this.status;
    //   string version = "HTTP/1.1";
    //   string mime = "text/html; ";
    //   string encoding = "charset=utf-8";

    //   string headers = "";
    //   headers = headers + version + " " + status + "\r\n";
    //   headers = headers + "Server: cx1193719-b\r\n";
    //   headers = headers + "Content-Type: " + mime + " " + encoding + "\r\n";
    //   headers = headers + "Accept-Ranges: bytes\r\n";
    //   headers = headers + "Content-Length: " + contentLength + "\r\n\r\n";

    //   Byte[] headersByte = System.Text.Encoding.ASCII.GetBytes(headers);
    //   stream.Write(headersByte, 0, headersByte.Length);
    // }
  }
}