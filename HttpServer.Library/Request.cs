using System;
using System.IO;

namespace HttpServer.Library
{
  public class Request
  {
    public string Url;
    public Request(Stream stream)
    {
      string streamData = HttpServerCore.GetStreamData(stream);
      string[] tokens = streamData.Split(" ");
      Url = tokens[1];
    }

  }
}