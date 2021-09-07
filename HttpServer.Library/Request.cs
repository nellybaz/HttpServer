using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace HttpServer.Library
{
  public class Request
  {
    public string Url;
    public string Method;
    public bool IsPath;
    public bool IsRoute;

    private bool _authenticated;

    private string _body;
    public string Body
    {
      get => _body;
    }

    public bool Authenticated
    {
      get => _authenticated;
      set => _authenticated = value;
    }

    private string _range;
    public string Range
    {
      get => _range;
    }

    public string Query
    {
      get => DecodeQuery();
    }
    private string _etag;
    public string Etag { get => _etag; }

    public App App = new App();
    public string Authorization;


    public Request(string dataFromStream)
    {

      string newLine = Environment.NewLine;
      string[] dataSplit = dataFromStream.Split($"{newLine}{newLine}");
      if (dataSplit.Length == 1)
      {
        dataSplit = dataFromStream.Split("\r\n\r\n");
      }
      string headers = dataSplit[0];
      if (dataSplit.Length > 1)
      {
        _body = dataSplit[1];
      }

      string[] tokens = headers.Split(" ");
      string[] tokensByNewLine = headers.Split("\r\n");
      Method = tokens[0];
      Url = tokens[1];

      foreach (var item in tokensByNewLine)
      {
        if (item.Contains("Authorization"))
        {
          Authorization = item.Split(":")[1].TrimStart().Split("\r")[0];
        }
        if (item.Contains("Range"))
        {
          _range = item.Split("=")[1];
        }
        if (item.Contains("If-Match"))
        {
          _etag = item.Split(":")[1].TrimStart().Split("\r")[0];
        }
      }
    }

    internal void SetStaticPath(string value)
    {
      App.StaticPath = value;
    }

    public string DecodeQuery()
    {
      string queryString = this.Url.Split("?")[1];
      string decodedQuery = HttpUtility.UrlDecode(queryString);
      var res = HttpUtility.ParseQueryString(queryString); // {key:value}
      string output = "";
      bool firstSign = false;
      foreach (var character in decodedQuery)
      {
        if (character == '=' && !firstSign)
        {
          output += " " + character + " ";
          firstSign = true;
        }
        else { output += character; }
      }
      return output;
    }
  }
}