using HttpServer.Library;
using Xunit;
using System;

namespace HttpServer.Test.Client.Middlewares
{
  public class BasicAuthTest
  {
    string _staticPath = "/Users/nbassey/Development/owc/http-server/public";

    [Fact]
    public void Request_With_No_Auth_Headers_Returns_401()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("GET", "/logs"));

      //When
      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, "user", "pass");
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._401, response.Headers);
    }

    [Fact]
    public void Protected_Url_Has_WWW_Authenticate_Header()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "Basic abcd"));

      //When
      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, "user", "pass");
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains("WWW-Authenticate", response.Headers);
    }

    [Fact]
    public void Authenticated_Url_Returns_200_When_Authentication()
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

      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, userName, password);
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
      Assert.Contains("GET /logs HTTP/1.1", response.Body);
    }

    [Fact]
    public void Authenticated_Url_Returns_401_Error_If_Unauthenticated()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "Basic abcd"));

      //When
      string userName = "admin";
      string password = "hunter2";

      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, userName, password);
      httpServerCore.ProcessMiddleWares(request, response);


      //Then
      Assert.Contains(StatusCode._401, response.Headers);
    }


    [Fact]
    public void Authenticated_Url_Returns_401_Error_For_Bad_Authentication()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "abcdexd"));
      //When

      //When
      string userName = "admin";
      string password = "hunter2";

      string[] urls = { "/logs" };
      httpServerCore.SetBasicAuth(urls, userName, password);
      httpServerCore.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._401, response.Headers);
    }
  }
}