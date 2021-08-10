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
      Assert.Contains(StatusCode._200, response.Headers);
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


    [Fact]
    public void Head_Request_To_Index_Returns_200()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("HEAD", "/"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
    }

    [Fact]
    public void Post_To_CatForm_Url()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("POST", "/cat-form", "data=fatcat"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._201, response.Headers);
      Assert.Contains("Location:", response.Headers);
      Assert.Contains("/cat-form/data", response.Headers);
    }

    [Fact]
    public void Get_To_CatForm_Url()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._404, response.Headers);
    }

    [Fact]
    public void Get_To_CatForm_Url_After_Post()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("POST", "/cat-form", "data=fatcat"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      //When
      // httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request2, response2);

      //Then
      Assert.Contains(StatusCode._200, response2.Headers);
      Assert.Contains("data=fatcat", response2.Body);
    }

    [Fact]
    public void Put_To_CatForm_Url()
    {
      //Given
      string data = "data=newData";
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("PUT", "/cat-form/data", data));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);


      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      httpServerCore.ProcessMiddleWares(request2, response2);

      Assert.Contains(StatusCode._200, response2.Headers);
      Assert.Contains(data, response2.Body);
    }

    [Fact]
    public void Delete_To_CatForm_Url()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("POST", "/cat-form", "data=fatcat"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());

      httpServerCore.ProcessMiddleWares(request, response);

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      //When

      httpServerCore.ProcessMiddleWares(request2, response2);

      //Then
      Assert.Contains(StatusCode._200, response2.Headers);
      Assert.Contains("data=fatcat", response2.Body);


      var response3 = new Response();
      var request3 = new Request(RequestFixtures.Sample("DELETE", "/cat-form/data"));
      request3.App.StaticPath = _staticPath;

      //When

      httpServerCore.ProcessMiddleWares(request3, response3);

      //Then
      Assert.Contains(StatusCode._200, response3.Headers);



      var response4 = new Response();
      var request4 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request4.App.StaticPath = _staticPath;

      httpServerCore.ProcessMiddleWares(request4, response4);

      //Then
      Assert.Contains(StatusCode._404, response4.Headers);
    }

    [Fact]
    public void Cookie_Route()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/cookie?type=chocolate"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Contains("Eat", response.Body);
    }


    [Fact]
    public void Eat_Cookie_Route()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/eat_cookie"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.AddMiddleWare(new Routes());
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Contains("mmmm chocolate", response.Body);
      Assert.Contains("Set-Cookie:", response.Headers);
    }
  }
}