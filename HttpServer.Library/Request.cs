using System;
using System.IO;

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
        if(item.Contains("Cookie")){
            
        }
      }
    }

    internal void SetStaticPath(string value)
    {
      App.StaticPath = value;
    }
  }
}