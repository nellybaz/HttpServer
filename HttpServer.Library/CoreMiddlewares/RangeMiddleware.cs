using System;

namespace HttpServer.Library.CoreMiddlewares
{
  public class RangeMiddleware : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      if (request.Range != null)
      {
        try
        {
          string[] rangeSplit = request.Range.Split("-");
          string start = rangeSplit[0];
          string end = rangeSplit[1];

          Index startRange = 0;
          Index endRange = 0;
          Byte[] currentByteData = response.BodyBytes;

          if (start == "")
          {
            startRange = ^Int32.Parse(end);
            endRange = ^0;
          }
          else
          {
            startRange = Int32.Parse(start);
            endRange = Int32.Parse(end) + 1;
          }

          Byte[] newByteData = currentByteData[startRange..endRange];

          //   newByteData = ;

          //   int newByteIndex = 0;
          //   for (int i = startRange; i <= endRange; i++)
          //   {
          //     newByteData[newByteIndex] = currentByteData[i];
          //     newByteIndex++;
          //   }

          response.SetBody(newByteData);
          response.SetStatus(StatusCode._206);
        }
        catch (System.Exception e)
        {
          // TODO
          Console.WriteLine(e);
        }
      }
    }
  }
}