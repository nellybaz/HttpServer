using System;
using System.IO;

namespace HttpServer.Library
{
  public class Request
  {
    public string Url;
    public string Method;
    public bool IsPath;

    private bool _authenticated;

    private string _body;
    public string Body {
      get => _body;
    }

    public bool Authenticated
    {
      get => _authenticated;
      set => _authenticated = value;
    }

    public string Authorization;
    public Request(string dataFromStream)
    {

      string newLine = Environment.NewLine;
      string[] dataSplit = dataFromStream.Split($"{newLine}{newLine}");
      if(dataSplit.Length == 1){
        dataSplit = dataFromStream.Split("\r\n\r\n");
      }
      string headers = dataSplit[0];
      if(dataSplit.Length > 1){
        _body = dataSplit[1];
      }

      string[] tokens = headers.Split(" ");
      string[] tokensByNewLine = headers.Split(newLine);
      Method = tokens[0];
      Url = tokens[1];
      if (tokensByNewLine[1].Contains("Authorization"))
      {
        Authorization = tokensByNewLine[1].Split(":")[1].TrimStart();
        // $"{tokens[3]} {tokens[4].Split("\n")[0]}";
      }

    }

  }
}