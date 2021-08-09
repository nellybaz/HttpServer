using HttpServer.Library;

namespace HttpClient.Middlewares
{
  public class DefaultBody : Middleware
  {

    public override void Run(Request request, Response response)
    {
      response.SetBody("Client App");
    }
  }
}