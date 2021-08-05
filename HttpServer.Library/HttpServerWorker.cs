using System;
using System.IO;

namespace HttpServer.Library
{
   class HttpServerWorker
  {
    private Stream stream;
    public Request request;

    public Response response;

    public HttpServerWorker(Stream stream, Request request, Response response)
    {
      this.stream = stream;
      this.request = request;
      this.response = response;
    }

    public void Write()
    {
      try
      {
        stream.Write(response.HeadersByte, 0, response.HeadersByte.Length);
        stream.Write(response.BodyBytes, 0, response.BodyBytes.Length);
      }
      catch (System.Exception ex)
      {
        // TODO
        Console.WriteLine(ex);
      }
    }

  }
}