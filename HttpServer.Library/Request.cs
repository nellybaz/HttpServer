using System;
using System.IO;

namespace HttpServer.Library
{
  public class Request
  {
    public string Url;
    public string Method;
    public Request(string dataFromStream)
    {
      string[] tokens = dataFromStream.Split(" ");
      Method = tokens[0];
      Url = tokens[1];
    }

  }
}