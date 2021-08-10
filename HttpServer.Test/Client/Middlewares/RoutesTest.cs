using System;
using HttpClient.Middlewares;
using HttpServer.Library;
using Xunit;

namespace HttpServer.Test.Client.Middlewares
{
  public class RoutesTest
  {
    string _staticPath = "/Users/nbassey/Development/owc/http-server/public";

    [Fact]
    public void ProcessRoutes_List_Files_For_Index_Url()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleGet());
      request.App.StaticPath = _staticPath;

      //When
      new Routes().Run(request, response);

      //Then
      Assert.Contains("<a href='/file1'", response.Body);
      Assert.Contains("<a href='/file2'", response.Body);
      Assert.Contains("<a href='/image.gif'", response.Body);
      Assert.Contains("<a href='/image.png'", response.Body);
      Assert.Contains("<a href='/image.jpeg'", response.Body);
    }

    [Fact]
    public void Authenticated_Requests_To_Logs_Route_Returns_Result()
    {
      //Given

      string userName = "admin";
      string password = "hunter2";

      Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
      string base64 = Convert.ToBase64String(byteData);

      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "Basic " + base64));

      //When
      httpServerCore.AddMiddleWare(new Routes());


      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, userName, password);


      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Contains("GET /logs HTTP/1.1", response.Body);
    }
  }
}