using HttpServer.Library;
public class CookieController : Controller
{
  public void Run(Request request, Response response)
  {
    response.SetStatus(StatusCode._200);
    response.SetBody("mmmm chocolate");
    response.SetHeader(Response.Header.Set_Cookie, "type=chocolate");

  }

  public void Run2(Request request, Response response)
  {
    response.SetStatus(StatusCode._200);
    response.SetBody("Eat");
  }
}
