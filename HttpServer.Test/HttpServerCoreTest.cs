﻿using System;
using Xunit;
using System.IO;
using HttpServer.Library;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

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
    public void HandleRequest_Returns_OK_Response_For_Valid_Get_Request()
    {

      Stream clientStream = new MemoryStream();
      Byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(RequestFixtures.SampleGet());

      clientStream.Write(byteMessage, 0, byteMessage.Length);

      var httpServerCore = new HttpServerCore(_staticPath);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);


      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
      string status = StatusCode._200;
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

      Middlewares.ProcessPublicDirectory(request, response);
      //Then
      Assert.Equal("file1 contents", response.Body);
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

      clientStream.Write(byteMessage, 0, byteMessage.Length);
      clientStream.Seek(0, SeekOrigin.Begin);
      var httpServerCore = new HttpServerCore(_staticPath);
      string actual0 = HttpServerCore.GetStreamData(clientStream);
      Assert.Contains("OPTIONS", actual0);
      clientStream.Seek(0, SeekOrigin.Begin);
      httpServerCore.HandleRequest(clientStream);

      clientStream.Seek(0, SeekOrigin.Begin);
      string actual = HttpServerCore.GetStreamData(clientStream);
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
      Middlewares.ProcessMethods(request, response);
      //Then
      string expected = "Allow: GET, HEAD, OPTIONS, PUT, DELETE";
      Assert.Contains(expected, response.Headers);
    }

    [Fact]
    public void ProcessProtectedPath_Sets_Response_Methods_For_Options_Request_Method()
    {
      //Given
      string requestData = RequestFixtures.SampleOptions("/logs");
      var request = new Request(requestData);
      var response = new Response();
      //When
      Middlewares.ProcessRoutes(request, response);
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

      Middlewares.ProcessPublicDirectory(request, response);
      Middlewares.ProcessPublicDirectoryRestrictions(request, response);

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

      Middlewares.AllowedMethod(request, response);

      //Then
      Assert.Contains(StatusCode._501, response.Headers);
    }

    [Fact]
    public void Authenticated_Url_Returns_401_Error_If_Unauthenticated()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "Basic abcd"));
      //When

      Middlewares.BasicAuthentication(request, response);

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

      Middlewares.BasicAuthentication(request, response);

      //Then
      Assert.Contains(StatusCode._401, response.Headers);
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

      Middlewares.BasicAuthentication(request, response);

      //Then
      Assert.Contains(StatusCode._200, response.Headers);
    }

    [Fact]
    public void Halted_Response_Skips_Subsequent_MiddleWares()
    {
      //Given

      string userName = "admin";
      string password = "hunter";

      Byte[] byteData = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
      string base64 = Convert.ToBase64String(byteData);

      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleAuthorized("GET", "/logs", "Basic " + base64));

      //When
      Helper.processMiddleWares(httpServerCore, request, response);


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
      Middlewares.BasicAuthentication(request, response);

      //Then
      Assert.Contains("WWW-Authenticate", response.Headers);
    }

    [Fact]
    public void ProcessRoutes_List_Files_For_Index_Url()
    {
      //Given
      var httpServerCore = new HttpServerCore(_staticPath);
      var response = new Response();
      var request = new Request(RequestFixtures.SampleGet());
      request.App.StaticPath = _staticPath;

      //When
      Middlewares.ProcessRoutes(request, response);

      //Then
      Assert.Contains("<a href='/file1'", response.Body);
      Assert.Contains("<a href='/file2'", response.Body);
      Assert.Contains("<a href='/image.gif'", response.Body);
      Assert.Contains("<a href='/image.png'", response.Body);
      Assert.Contains("<a href='/image.jpeg'", response.Body);
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
      Assert.Contains("206 Partial Content", response.Headers);
      Assert.Equal(5, response.ContentLength);
    }
  }

  public class Helper
  {
    public static void processMiddleWares(HttpServerCore httpServerCore, Request request, Response response)
    {
      List<Action<Request, Response>> middlewares = new List<Action<Request, Response>>();
      middlewares.Add(Middlewares.AllowedMethod);
      middlewares.Add(Middlewares.ProcessPublicDirectory);
      middlewares.Add(Middlewares.ProcessMethods);
      middlewares.Add(Middlewares.ProcessRoutes);
      middlewares.Add(Middlewares.ProcessPublicDirectoryRestrictions);
      middlewares.Add(Middlewares.ProcessRanges);

      httpServerCore.ProcessMiddleWares(middlewares, request, response);
    }
  }
}
