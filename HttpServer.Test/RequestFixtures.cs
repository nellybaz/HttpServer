using System;

namespace HttpServer.Test
{
  public class RequestFixtures
  {

    public static string Sample(string method, string route)
    {
      string sampleGetRequest = method + " " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }

    public static string Sample(string method, string route, string data)
    {
      string sampleGetRequest = method + " " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n\r\n" + data;
      return sampleGetRequest;
    }

      public static string SampleRange(string method, string route, string range)
    {
      string sampleGetRequest = method + " " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\nRange: " + range + "\r\n\r\n";
      return sampleGetRequest;
    }

    public static string SampleGet()
    {
      string sampleGetRequest = "GET / HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }

    internal static string SamplePatchRequest(string hash)
    {
      return $"PATCH / HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nIf-Match: {hash}\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
    }

       internal static string SamplePatchRequest(string path, string data, string hash)
    {
      return $"PATCH {path} HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nIf-Match: {hash}\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n\r\n" + data;
    }

    public static string SampleHead(string route)
    {
      string sampleGetRequest = "HEAD " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }

    internal static string SampleAuthorized(string method, string route, string authorization)
    {
      string sampleGetRequest = method + " " + route + " HTTP/1.1\r\nAuthorization: " + authorization + "\r\n" + "Host: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }

    public static string SampleOptions(string route)
    {
      string sampleGetRequest = "OPTIONS " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }

    public static string SampleGet(string route)
    {
      string sampleGetRequest = "GET " + route + " HTTP/1.1\r\nHost: localhost:5050\r\nUser-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10.15; rv:89.0) Gecko/20100101 Firefox/89.0\r\nAccept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\nAccept-Language: en-US,en;q=0.5\r\nAccept-Encoding: gzip, deflate\r\nConnection: keep-alive\r\nCookie: textwrapon=false; textautoformat=false; wysiwyg=textarea\r\nUpgrade-Insecure-Requests: 1\r\n";
      return sampleGetRequest;
    }
  }
}