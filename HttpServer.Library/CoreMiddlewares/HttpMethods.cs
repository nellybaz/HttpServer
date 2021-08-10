using System.IO;
using System.Text;

namespace HttpServer.Library.CoreMiddlewares
{
  public class HttpMethods : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      if (!RequestMethod.IsValid(request.Method))
      {
        response.Status = StatusCode._501;
      }

      if (request.Method == RequestMethod.OPTIONS)
      {
        response.Methods = "GET, HEAD, OPTIONS, PUT, DELETE";
        response.Status = StatusCode._200;
      }
      if (request.Method == RequestMethod.HEAD) response.SetBody("");

      string path = request.App.StaticPath + request.Url;

      if (request.Method == RequestMethod.PUT)
      {
        using (FileStream fs = File.Create(path))
        {
          byte[] info = new UTF8Encoding(true).GetBytes(request.Body);
          fs.Write(info, 0, info.Length);

          if (request.IsPath)
          {
            response.SetStatus(StatusCode._200);
            response.SetBody("Updated");
          }
          else
          {
            response.SetStatus(StatusCode._201);
            response.SetBody("Created");
          }
        }
      }
      if (request.Method == RequestMethod.DELETE && request.IsPath)
      {
        File.Delete(path);
        response.SetStatus(StatusCode._200);
        response.SetBody("Deleted");
      }
    }
  }
}