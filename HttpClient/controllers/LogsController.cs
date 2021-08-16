using HttpServer.Library;

public class LogsController : Controller
{
  public void Run(Request request, Response response)
  {
    if (request.Authenticated)
      {
        if (request.Method == RequestMethod.POST)
        {
          response.SetStatus(StatusCode._405);
        }
        else
        {
          response.SetStatus(StatusCode._200);
        }
        response.SetBody(request.Method + " " + request.Url + " " + response.Version);
        response.Mime = MimeType.PlainText;
        response.Halt();
      }
  }
}