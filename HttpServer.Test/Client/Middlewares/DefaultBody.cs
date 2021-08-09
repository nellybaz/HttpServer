using Xunit;
using HttpServer.Library;
using HttpClient;
namespace HttpServer.Test.Client.Middlewares
{
  public class DefaultBody
  {
    [Fact]
    public void Run_Sets_Response_Body_To_Default_Message()
    {
      //Given
      var request = new Request(RequestFixtures.SampleGet());
      var response = new Response();

      //When
      new HttpClient.Middlewares.DefaultBody().Run(request, response);

      //Then
      Assert.Contains(response.Body, "Client App");
    }
  }
}