
using HttpServer.Library;

public class CoffeeTeaController : Controller
{
  public void Run(Request request, Response response)
  {
      response.SetStatus(StatusCode._418);
      response.SetBody("I'm a teapot");
  }

  public void Tea(Request request, Response response)
  {
      response.SetStatus(StatusCode._200);
  }
}
