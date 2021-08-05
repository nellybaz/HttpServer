using System.Collections.Generic;

namespace HttpServer.Library
{
  public class MimeType
  {
    public static readonly string PlainText = "text/plain";
    public static readonly string Html = "text/html";

    public static readonly string Jpeg = "image/jpeg";
    public static readonly string Png = "image/png";
    public static readonly string Gif = "image/gif";

    public static string GetMimeType(string path)
    {
      try
      {
        string extension = path.Split(".")[1];
        Dictionary<string, string> mimeHash = new Dictionary<string, string> {
        { "", MimeType.PlainText },
        {"jpeg", MimeType.Jpeg},
        {"png", MimeType.Png},
        {"gif", MimeType.Gif},
        {"html", MimeType.Html},
        {"txt", MimeType.PlainText}
        };

        return mimeHash[extension];
      }
      catch (System.Exception)
      {
        return MimeType.PlainText;
      }
    }

  }

  // public enum MimeType{
  //     PlainText
  // }
}