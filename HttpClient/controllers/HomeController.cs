using System.IO;
using HttpServer.Library;

public class HomeController:Controller{
  public void Run(Request request, Response response){
        string links = "";
        string[] files = Directory.GetFiles(request.App.StaticPath);
        foreach (var file in files)
        {
          string url = file.Split("public")[1];
          links += $"<a href='{url}'>{url}</a></br>";
        }
        string body = $"<html>{links}</html>";
        response.SetStatus(StatusCode._200);
        response.SetBody(body);
        return;
      }
}