using System;
using System.IO;

namespace HttpServer.Library
{
  public class Request
  {
    public string Url;
    public Request(Stream stream)
    {
      string[] tokens = HttpServerCore.GetStreamData(stream).Split(" ");
      Url = tokens[1];
    }

  }
}