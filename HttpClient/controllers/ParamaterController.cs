using HttpServer.Library;

public class ParameterController : Controller
{
  public void Run(Request request, Response response)
  {
    response.SetStatus(StatusCode._200);
    response.SetBody(request.Query);
  }
}