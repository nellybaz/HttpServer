using System;
using System.Collections.Generic;
using System.IO;

namespace HttpServer.Library
{
  public class Response
  {

    public enum Header
    {
      Set_Cookie,
      Content_Range,
      Location,
      Authenticate,
      Allowed_Method,
      Content_Type,
      Content_Length
    }

    private Dictionary<String, String> _headers = new Dictionary<String, string>();
    private string newLine = Environment.NewLine;
    private string _status;
    public string Status
    {
      get => _status;
      set => _status = value;
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

    public int ContentLength
    {
      get => int.Parse(_headers[HeaderToString(Header.Content_Length)]);
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

    public Response()
    {
      string mime = "";
      string server = "XHTTPServer";
      _version = "HTTP/1.1";
      SetHeader(HeaderToString(Header.Content_Length), "0");
      SetHeader("Server", server);
      SetHeader(HeaderToString(Header.Content_Type), mime);
      SetHeader("Accept-Ranges", "bytes");
    }

    private string HeaderToString(Header header)
    {
      switch (header)
      {
        case Header.Set_Cookie:
          return "Set-Cookie";
        case Header.Content_Range:
          return "Content-Range";
        case Header.Location:
          return "Location";
        case Header.Authenticate:
          return "WWW-Authenticate";
        case Header.Allowed_Method:
          return "Allow";
        case Header.Content_Type:
          return "Content-Type";
        case Header.Content_Length:
          return "Content-Length";
        default:
          return "";
      }
    }

    public void Authenticate(bool value)
    {
      if (value) SetHeader(HeaderToString(Header.Authenticate), "Basic realm='Access to resource', charset='UTF-8'");
    }

    public void SetBody(string message)
    {
      _body = message;
      _bodyByte = System.Text.Encoding.ASCII.GetBytes(_body);
      SetHeader(HeaderToString(Header.Content_Length), BodyBytes.Length.ToString());
    }

    public void SetStatus(string status)
    {
      this._status = status;
    }
    public void SetBody(Byte[] messageByte)
    {
      _bodyByte = messageByte;
      _body = System.Text.Encoding.ASCII.GetString(messageByte);
      SetHeader(HeaderToString(Header.Content_Length), messageByte.Length.ToString());
    }

    public void Halt()
    {
      _halted = true;
    }
    public void SetHeader(Header header, String value)
    {
      SetHeader(HeaderToString(header), value);
    }

    public void SetHeader(String header, string value)
    {
      if (_headers.ContainsKey(header))
      {
        _headers[header] = value;
      }
      else
      {
        _headers.Add(header, value);
      }
    }
    private string FormatHeaders()
    {

      string headers = $"{_version} {_status}{newLine}";

      foreach (var item in _headers)
      {
        headers += $"{item.Key}: {item.Value}{newLine}";
      }
      return headers + newLine;
    }
  }
}