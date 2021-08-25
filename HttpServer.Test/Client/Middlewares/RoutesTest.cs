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
      httpServerCore.Route("GET", "/", new HomeController().Run);
      httpServerCore.ProcessRoutes(request, response);

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
      httpServerCore.Route("*", "/logs", new LogsController().Run);


      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, userName, password);


      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

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
      httpServerCore.Route("*", "/cat-form", new CatFormController().Run);

      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

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
      // httpServerCore.AddMiddleWare(new Routes());

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
      httpServerCore.Route("*", "/cat-form", new CatFormController().Run);

      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      //When

      httpServerCore.ProcessMiddleWares(request2, response2);
      httpServerCore.ProcessRoutes(request2, response2);

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
      httpServerCore.Route("*", "/cat-form", new CatFormController().Run);


      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);


      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      httpServerCore.ProcessMiddleWares(request2, response2);
      httpServerCore.ProcessRoutes(request2, response2);


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
      httpServerCore.Route("*", "/cat-form", new CatFormController().Run);

      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", "/cat-form/data"));
      request2.App.StaticPath = _staticPath;

      //When


      httpServerCore.ProcessMiddleWares(request2, response2);
      httpServerCore.ProcessRoutes(request2, response2);

      //Then
      Assert.Contains(StatusCode._200, response2.Headers);
      Assert.Contains("data=fatcat", response2.Body);


      var response3 = new Response();
      var request3 = new Request(RequestFixtures.Sample("DELETE", "/cat-form/data"));
      request3.App.StaticPath = _staticPath;

      //When

      httpServerCore.ProcessMiddleWares(request3, response3);
      httpServerCore.ProcessRoutes(request3, response3);

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

      httpServerCore.Route("GET", "/cookie", new CookieController().Run2);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

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
      httpServerCore.Route("GET", "/eat_cookie", new CookieController().Run);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Contains("mmmm chocolate", response.Body);
      Assert.Contains("Set-Cookie: type=chocolate", response.Headers);
    }

    [Fact]
    public void FourEighteen_Status_Code_For_Coffee_Route()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/coffee"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.Route("GET", "/coffee", new CoffeeTeaController().Run);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      Assert.Contains(StatusCode._418, response.Headers);
      Assert.Contains("I'm a teapot", response.Body);
    }

    [Fact]
    public void Status_Code_For_Tea_Route()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/tea"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.Route("GET", "/tea", new CoffeeTeaController().Tea);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
    }

    [Fact]
    public void Decode_Request_With_Parameter()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string url =
      "/parameters?variable_1=a%20query%20string%20parameter";
      var request = new Request(RequestFixtures.Sample("GET", url));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.Route("GET", "/parameter", new ParameterController().Run);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      string expected = "variable_1 = a query string parameter";
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Equal(expected, response.Body);
    }



    [Fact]
    public void Decode_Request_With_Parameter_2()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string url =
      "/parameters?variable_1=Operators%20%3C%2C%20%3E%2C%20%3D%2C%20!%3D%3B%20%2B%2C%20-%2C%20*%2C%20%26%2C%20%40%2C%20%23%2C%20%24%2C%20%5B%2C%20%5D%3A%20%22is%20that%20all%22%3F&variable_2=stuff";
      var request = new Request(RequestFixtures.Sample("GET", url));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.Route("GET", "/parameter", new ParameterController().Run);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      string expected = "variable_1 = Operators <, >, =, !=; +, -, *, &, @, #, $, [, ]: \"is that all\"?";
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Equal(expected, response.Body);
    }

    [Fact]
    public void Redirect_Request_Returns_302()
    {
      // Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/redirect"));
      request.App.StaticPath = _staticPath;

      //When
      httpServerCore.Route("GET", "/redirect", new RedirectController().Run);
      httpServerCore.ProcessMiddleWares(request, response);
      httpServerCore.ProcessRoutes(request, response);

      //Then
      Assert.Contains(StatusCode._302, response.Headers);
      Assert.Contains("/", response.Headers);
    }
  }
}