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

    public bool Authenticated{
      get => _authenticated;
      set => _authenticated = value;
    }

    public string Authorization;
    public Request(string dataFromStream)
    {
      string[] tokens = dataFromStream.Split(" ");
      string[] tokensByNewLine = dataFromStream.Split("\n");
      Method = tokens[0];
      Url = tokens[1];
      if(tokensByNewLine[1].Contains("Authorization")){
        Authorization = tokensByNewLine[1].Split(":")[1].TrimStart();
      // $"{tokens[3]} {tokens[4].Split("\n")[0]}";
      }
      
    }

  }
}