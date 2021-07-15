namespace HttpServer.Library
{
  public class Request
  {
    public static string SampleGet()
    {
      //   string status = "200 OK";
      //   string version = "HTTP/1.1";
      //   string mime = "text/html";

      //   string sBuffer = "";
      //   sBuffer = sBuffer + version + " " + status + "\r\n";
      //   sBuffer = sBuffer + "Server: cx1193719-b\r\n";
      //   sBuffer = sBuffer + "Content-Type: " + mime + "\r\n";
      //   sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
      //   sBuffer = sBuffer + "Content-Length: " + 50 + "\r\n\r\n";


      string sampleGetRequest = "GET / HTTP/1.1\nHost: localhost:5050\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\nAccept-Language: en-US,en;q=0.5\nAccept-Encoding: gzip, deflate\nConnection: keep-alive\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\nUpgrade-Insecure-Requests: 1\n";

      return sampleGetRequest;
    }
  }
}