using System;

namespace HttpServer.Library
{
  public class App
  {
    public void JwtToken(Request request, Response response)
    {
      Console.WriteLine("Authenticating with token here");
      response.Status = StatusCode._501;
      response.SetBody("Unexpected Server error");
    }

    public void Routes(Request request, Response response)
    {
      switch (request.Url)
      {
        case "/hit":
          response.SetBody("I am hitting this real good");
          break;

        case "/login":
          response.SetBody("You are logged in thanks!");
          break;

        case "/admin":
          response.SetBody("Admin Dashboard");
          break;
        case "/register":
          response.SetBody("You are registered!! Yay!!!");
          break;
        default:
          response.SetBody("Page not found");
          break;
      }

    }
    public void Run()
    {

      var staticPath = "/Users/nbassey/Development/owc/http-server/public";
      int port = 5000;
      HttpServerCore server = new HttpServerCore(staticPath);


      // server.AddMiddleWare(JwtToken);
      server.AddMiddleWare(Routes);
      server.Run(port);
    }
  }
}