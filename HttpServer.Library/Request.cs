namespace HttpServer.Library
{
  public class Request
  {
    public static string SampleGet()
    {
      string status = "200 OK";
      string version = "HTTP/1.1";
      string mime = "text/html";

      string sBuffer = "";
      sBuffer = sBuffer + version + " " + status + "\r\n";
      sBuffer = sBuffer + "Server: cx1193719-b\r\n";
      sBuffer = sBuffer + "Content-Type: " + mime + "\r\n";
      sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
      sBuffer = sBuffer + "Content-Length: " + 50 + "\r\n\r\n";


      return sBuffer;
    }
  }
}