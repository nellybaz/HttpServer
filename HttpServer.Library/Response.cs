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

    private string _body;
    public string Body{
      get => _body;
    }

    public Byte[] BodyBytes{
      get => System.Text.Encoding.ASCII.GetBytes(_body);
    }

    private int _contentLength;
    public int ContentLength{
      get => _contentLength;
      set => _contentLength = value;
    }

    public string Headers{
      get =>$"{_version} {_status}{newLine}Server: {_server}{newLine}Content-Type: {_mime} {_encoding}{newLine}Accept-Ranges: bytes{newLine}Content-Length: {_contentLength}{newLine}{newLine}";
    }

     public Byte[] HeadersByte{
      get => System.Text.Encoding.ASCII.GetBytes(Headers);
    }
    

    private string _version;
    private string _encoding;
    private string _server;

    public Response()
    {
      _status = StatusCode._200;
      _mime = "text/html;";
      _version = "HTTP/1.1";
      _encoding = "charset=utf-8";
      _contentLength = 0;
      _server = "XHTTPServer";
    }

    public void SetBody(string message){
      _body = message;
      _contentLength = Body.Length;
    }
  }
}