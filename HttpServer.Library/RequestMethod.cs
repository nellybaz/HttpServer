using System.Collections.Generic;

namespace HttpServer.Library
{
  public class RequestMethod
  {

    public static readonly string GET = "GET";

    public static readonly string POST = "POST";

    public static readonly string PUT = "PUT";
    public static readonly string HEAD = "HEAD";
    public static readonly string OPTIONS = "OPTIONS";
    public static readonly string DELETE = "DELETE";

    public static bool IsValid(string method)
    {
      Dictionary<string, bool> validMethods = new Dictionary<string, bool>{
                 {"POST", true},
                 {"GET", true},
                 {"OPTIONS", true},
                 {"PUT", true},
                 {"HEAD", true},
                 {"DELETE", true},
             };

      return validMethods.ContainsKey(method);
    }
  }
}