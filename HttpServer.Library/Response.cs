using System;
using System.IO;

namespace HttpServer.Library
{
  public class Response
  {
    private string newLine = Environment.NewLine;
    private string _status;
    public string Status
    {
      get => _status;
      set => _status = value;
    }


    private string _mime;
    public string Mime
    {
      get => _mime;
      set => _mime = value;
    }

    private string _methods;
    public string Methods
    {
      get => _methods;
      set => _methods = value;
    }

    private string _body;
    public string Body
    {
      get => _body;
    }

    public string Version
    {
      get => _version;
    }

    private Byte[] _bodyByte;
    public Byte[] BodyBytes
    {
      get => _bodyByte;
    }

    private bool _authenticate;
    public bool Authenticate
    {
      get => _authenticate;
      set => _authenticate = value;
    }

    private int _contentLength;
    public int ContentLength
    {
      get => _contentLength;
      set => _contentLength = value;
    }

    public string Headers
    {
      get => FormatHeaders();
    }

    public Byte[] HeadersByte
    {
      get => System.Text.Encoding.ASCII.GetBytes(Headers);
    }

    private bool _halted;
    public bool Halted
    {
      get => _halted;
    }


    private string _version;
    private string _encoding;
    private string _server;


    public Response()
    {
      // _status = StatusCode._200;
      _mime = "text/html;";
      _version = "HTTP/1.1";
      _encoding = "charset=utf-8";
      _contentLength = 0;
      _server = "XHTTPServer";
    }

    public void SetBody(string message)
    {
      _body = message;
      _bodyByte = System.Text.Encoding.ASCII.GetBytes(_body);
      _contentLength = BodyBytes.Length;
    }

    public void SetStatus(string status)
    {
      this._status = status;
    }
    public void SetBody(Byte[] messageByte)
    {
      _bodyByte = messageByte;
      _body = System.Text.Encoding.ASCII.GetString(messageByte);
      _contentLength = messageByte.Length;
    }

    public void Halt()
    {
      _halted = true;
    }
    private string FormatHeaders()
    {
      string method = "";
      if (_methods != null) method = $"Allow: {_methods}{newLine}";

      string authenticate = "";
      if (_authenticate) authenticate = $"WWW-Authenticate: Basic realm='Access to resource', charset='UTF-8'{newLine}";
      string headers = $"{_version} {_status}{newLine}{method}Server: {_server}{newLine}Content-Type: {_mime} {_encoding}{newLine}{authenticate}Accept-Ranges: bytes{newLine}Content-Length: {_contentLength}{newLine}{newLine}";
      return headers;
    }
  }
}