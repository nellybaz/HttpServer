
using HttpServer.Library;

public class RedirectController : Controller
{
  public void Run(Request request, Response response)
  {
    response.SetStatus(StatusCode._302);
    response.SetHeader(Response.Header.Location, "/");
  }
}
