using System;
using Xunit;
using System.IO;
using HttpServer.Library;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using HttpServer.Library.CoreMiddlewares;

namespace HttpServer.Test
{
  public class HttpServerTest
  {

    string _staticPath = "/Users/nbassey/Development/owc/http-server/public";

    [Fact]
    public void Run_Writes_Message_On_TCP_Connection()
    {

      int port = 5050;
      string server = "127.0.0.1";

      var output = new StringWriter();
      Console.SetOut(output);

      Thread tcpThread = new Thread(() => { new HttpServerCore(_staticPath).Run(port); });
      tcpThread.Start();

      Thread.Sleep(1000);
      TcpClient client = new TcpClient(server, port);

      NetworkStream clientStream = client.GetStream();
      Byte[] message = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(message, 0, message.Length);
      clientStream.Close();

      Assert.Contains("Server listening on port: " + port + "\nTCP connection received\n", output.ToString());
    }

    [Fact]
    public void GetStreamData_Returns_Correct_Message_From_Strem()
    {

      string message = "GET, hi there\n";

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message);

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      string expectedMessage = HttpServerCore.GetStreamData(clientStream);
      Assert.Equal(message, expectedMessage);
    }

    [Fact]
    public void HandleRequest_Returns_StatusCode_For_Valid_Get_Request()
    {

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(byteMessage, 0, byteMessage.Length);

      var httpServerCore = new HttpServerCore(_staticPath);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);


      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = StatusCode._404;
      Assert.Contains(status, actual);

    }

    [Fact]
    public void HandleRequest_Returns_404_When_Path_Not_Found()
    {
      Stream clientStream = new MemoryStream(256);
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet("/not-found"));

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      var httpServerCore = new HttpServerCore(_staticPath);
      string actual0 = HttpServerCore.GetStreamData(clientStream);
      Assert.Contains("GET", actual0);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);

      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = "404 Not Found";
      Assert.Contains(status, actual);
    }

    [Fact]
    public void GeMessageFromPath_Returns_Message_In_File()
    {
      //Given
      var request = new Request(RequestFixtures.SampleGet("/file1"));
      request.App.StaticPath = _staticPath;
      var response = new Response();

      //When
      new PublicDirectory().Run(request, response);

      //Then
      Assert.Equal("file1 contents", response.Body);
      Assert.Contains(StatusCode._200, response.Headers);
    }

    [Fact]
    public void HandleRequest_Does_Not_Write_Body_To_Stream_For_Head_Method()
    {
      Stream clientStream = new MemoryStream(256);
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleHead("/file1"));

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      var httpServerCore = new HttpServerCore(_staticPath);
      string actual0 = HttpServerCore.GetStreamData(clientStream);
      Assert.Contains("HEAD", actual0);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);

      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = "file1 contents";
      Assert.DoesNotContain(status, actual);
    }

    [Fact]
    public void Header_Has_Allow_Section_For_Options_Method()
    {
      Stream clientStream = new MemoryStream(256);
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleOptions("/file1"));

      Request request = new Request(RequestFixtures.SampleOptions("/file1"));
      Response response = new Response();

      HttpServerCore httpServerCore = new HttpServerCore(_staticPath);
      httpServerCore.ProcessMiddleWares(request, response);
      string actual = response.Headers;
      string status = "Allow: GET, HEAD, OPTIONS, PUT, DELETE";
      Assert.Contains(status, actual);
    }

    [Fact]
    public void ProcessMethod_Sets_Response_Methods_For_Options_Request_Method()
    {
      //Given
      string requestData = RequestFixtures.SampleOptions("/file1");
      var request = new Request(requestData);
      var response = new Response();

      //When
      new HttpMethods().Run(request, response);

      //Then
      string expected = "Allow: GET, HEAD, OPTIONS, PUT, DELETE";
      Assert.Contains(expected, response.Headers);
    }

    [Fact]
    public void ProcessProtectedPath_Sets_Response_Methods_For_Options_Request_Method()
    {
      //Given
      // var httpServer =  new HttpServerCore(_staticPath);
      string requestData = RequestFixtures.SampleOptions("/logs");
      var request = new Request(requestData);
      var response = new Response();

      //When

      var allowedMethods = new Dictionary<String, String>();
      allowedMethods.Add("/logs", "GET, HEAD, OPTIONS");
      new HttpMethods(allowedMethods).Run(request, response);
      // httpServer.SetAllowedMethods(allowedMethods);

      //Then
      string expected = "Allow: GET, HEAD, OPTIONS";
      string unexpectedMethods = "PUT, DELETE";
      Assert.Contains(expected, response.Headers);
      Assert.DoesNotContain(unexpectedMethods, response.Headers);
    }

    [Fact]
    public void MimeType_Determines_File_Mime_Type()
    {
      //Given

      //When
      string fileMimeType = MimeType.GetMimeType("file1");
      string imageMimeType = MimeType.GetMimeType("image.jpeg");
      string htmlMimeType = MimeType.GetMimeType("file.html");
      string pngMimeType = MimeType.GetMimeType("file.png");
      string gifMimeType = MimeType.GetMimeType("file.gif");

      //Then
      Assert.Equal(MimeType.PlainText, fileMimeType);
      Assert.Equal(MimeType.Jpeg, imageMimeType);
      Assert.Equal(MimeType.Html, htmlMimeType);
      Assert.Equal(MimeType.Png, pngMimeType);
      Assert.Equal(MimeType.Gif, gifMimeType);
    }

    [Fact]
    public void Post_Method_To_Static_File_Returns_405_Error()
    {
      //Given
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("POST", "/file1"));
      request.App.StaticPath = _staticPath;
      //When

      new PublicDirectory().Run(request, response);
      // Middlewares.ProcessPublicDirectory(request, response);

      //Then
      Assert.Contains(StatusCode._405, response.Headers);
    }

    [Fact]
    public void Bogus_Request_To_Static_File_Returns_501_Error()
    {
      //Given
      var response = new Response();
      var request = new Request(RequestFixtures.Sample("BOGUS", "/file1"));
      //When

      new HttpMethods().Run(request, response);

      //Then
      Assert.Contains(StatusCode._501, response.Headers);
    }

    [Fact]
    public void Halted_Response_Skips_Subsequent_MiddleWares()
    {
      //Given

      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleGet());

      //When
      response.SetStatus(StatusCode._401);
      response.Halt();
      Helper.processMiddleWares(httpServerCore, request, response);


      //Then
      Assert.Contains(StatusCode._401, response.Headers);
    }

    [Fact]
    public void Put_Creates_New_File_If_Not_Available()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string data = "content for file";
      string path = "/no-there" + DateTime.Now.ToLongTimeString();
      var request = new Request(RequestFixtures.Sample("PUT", path, data));
      request.App.StaticPath = _staticPath;

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains("201 Created", response.Headers);

      var request2 = new Request(RequestFixtures.Sample("DELETE", path, data));
      request2.App.StaticPath = _staticPath;
      Helper.processMiddleWares(httpServerCore, request2, response);
    }

    [Fact]
    public void DELETE_Removes_File()
    {

      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string data = "content for file";
      string path = "/no-there" + DateTime.Now.ToLongTimeString();
      var request = new Request(RequestFixtures.Sample("PUT", path, data));
      request.App.StaticPath = _staticPath;

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("DELETE", path));
      request2.App.StaticPath = _staticPath;

      var response3 = new Response();
      var request3 = new Request(RequestFixtures.Sample("GET", path));
      request3.App.StaticPath = _staticPath;

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      Helper.processMiddleWares(httpServerCore, request2, response2);

      Helper.processMiddleWares(httpServerCore, request3, response3);

      //Then
      Assert.Contains("201 Created", response.Headers);
      Assert.Contains("200 OK", response2.Headers);
      Assert.DoesNotContain("content for file", response2.Body);
    }

    [Fact]
    public void Adding_Class_Middleware_Modifies_Server_Response()
    {
      //Given
      var server = new HttpServerCore(_staticPath);
      var request = new Request(RequestFixtures.SampleGet());
      request.App.StaticPath = _staticPath;
      var response = new Response();

      //When
      server.AddMiddleWare(new SetStatusToNotFound());
      server.ProcessMiddleWares(request, response);

      //Then
      Assert.Contains(StatusCode._404, response.Headers);
    }

    [Fact]
    public void Head_Request_To_No_File_Returns_404()
    {
      //Given
      var httpServer = new HttpServerCore(_staticPath);
      var request = new Request(RequestFixtures.Sample("HEAD", "/no_file_here.txt"));
      var response = new Response();

      //When

      httpServer.ProcessMiddleWares(request, response);
      //Then
      Assert.Contains(StatusCode._404, response.Headers);
    }

    [Fact]
    public void Partial_Content_Returns_For_Valid_Start_And_End_Ranges()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=0-4";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Equal(5, response.ContentLength);
    }

    [Fact]
    public void Range_Request_Without_Start_Index()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=-6";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = ^6;
      Index end = ^0;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }


    [Fact]
    public void Range_Request_Without_End_Index()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=4-";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = 4;
      Index end = 77;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }

        [Fact]
    public void Range_Start_To_End_Greater_Than_Content()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=75-80";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = 75;
      Index end = 77;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }

         [Fact]
    public void Range_No_Start_To_End_Greater_Than_Content()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=-78";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = 0;
      Index end = 77;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }

          [Fact]
    public void Range_Start_Greater_Than_End()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=10-0";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = 0;
      Index end = 77;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      Assert.Contains(StatusCode._416, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }

           [Fact]
    public void Request_With_Range_Returns_Content_Range_In_Response_Headers()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string range = "bytes=0-4";
      string path = "/partial_content.txt";
      var request = new Request(RequestFixtures.SampleRange("GET", path, range));
      request.App.StaticPath = _staticPath;

      string fileContents = "This is a file that contains text to read part of in order to fulfill a 206.\n";
      Byte[] contentBytes = System.Text.Encoding.ASCII.GetBytes(fileContents);
      Index start = 0;
      Index end = 5;
      var expected = contentBytes[start..end];

      //When
      Helper.processMiddleWares(httpServerCore, request, response);

      //Then
      string contentRange = "Content-Range: bytes 0-4/77";
      Assert.Contains(StatusCode._206, response.Headers);
      Assert.Contains(contentRange, response.Headers);
      Assert.Equal(expected, response.BodyBytes);
    }

    [Fact(Skip = "return later")]
    public void PATCH_Returns_204_For_Valid_Etag()
    {

      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string etag = "dc50a0d27dda2eee9f65644cd7e4c9cf11de8bec";
      string data = "patched content";
      string path = "/patch-content.txt";
      var request = new Request(RequestFixtures.SamplePatchRequest(path, data, etag));
      request.App.StaticPath = _staticPath;

      var response2 = new Response();
      var request2 = new Request(RequestFixtures.Sample("GET", path));
      request2.App.StaticPath = _staticPath;


      Helper.processMiddleWares(httpServerCore, request, response);

      Assert.Contains(StatusCode._204, response.Status);

      Helper.processMiddleWares(httpServerCore, request2, response2);

      Assert.Contains(data, response2.Body);
      Assert.Contains(StatusCode._200, response2.Status);
    }

    [Fact]
    public void PATCH_Returns_Fails_For_Invalid_Etag()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      string etag = "etag";
      string data = "patched content";
      string path = "/patch-content.txt";
      var request = new Request(RequestFixtures.SamplePatchRequest(path, data, etag));
      request.App.StaticPath = _staticPath;

      Helper.processMiddleWares(httpServerCore, request, response);

      Assert.Equal(StatusCode._412, response.Status);
    }

    [Fact]
    public void Router_Extension()
    {
      //Given

      var httpServer = new HttpServerCore("");
      Request request = new Request(RequestFixtures.SampleGet("/users"));
      Response response = new Response();
      string message = "users lists here";
      //When

      httpServer.Route("GET", "/users", (Request request, Response response) =>
      {
        response.SetBody(message);
      });

      httpServer.ProcessRoutes(request, response);

      //Then

      Assert.Equal(message, response.Body);
    }

    [Fact]
    public void Router_Does_Not_Mixup_Routes()
    {
      //Given

      var httpServer = new HttpServerCore("");
      Request request = new Request(RequestFixtures.SampleGet("/zyx"));
      Response response = new Response();
      string message = "message body";
      //When

      httpServer.Route("GET", "/", (Request request, Response response) =>
            {
              response.SetBody(message);
            });

      httpServer.ProcessRoutes(request, response);

      //Then

      Assert.DoesNotContain(message, response.Body);
    }
  }

  public class SetStatusToNotFound : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      response.SetStatus(StatusCode._404);
    }
  }

  public class Helper
  {
    public static void processMiddleWares(HttpServerCore httpServerCore, Request request, Response response)
    {
      httpServerCore.ProcessMiddleWares(request, response);
    }
  }
}
