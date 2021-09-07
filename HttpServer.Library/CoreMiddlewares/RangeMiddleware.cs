using System;

namespace HttpServer.Library.CoreMiddlewares
{
  public class RangeMiddleware : IMiddleware
  {
    public void Run(Request request, Response response)
    {
      bool notSatifiable = false;
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

          if (IsEmpty(start))
          {
            if (int.Parse(end) > currentByteData.Length)
            {
              startRange = 0;
            }
            else
            {
              startRange = currentByteData.Length - int.Parse(end);
            }
            endRange = currentByteData.Length - 1;

          }
          else if (IsEmpty(end))
          {
            endRange = currentByteData.Length - 1;
            startRange = Int32.Parse(start);
            response.SetHeader(Response.Header.Content_Range, $"bytes {start}-{end}/{currentByteData.Length}");
          }
          else
          {
            if (int.Parse(start) > int.Parse(end))
            {
              startRange = 0;
              endRange = currentByteData.Length - 1;
              notSatifiable = true;
            }
            else
            {
              startRange = Int32.Parse(start);
              endRange = Int32.Parse(end);
            }
            response.SetHeader(Response.Header.Content_Range, $"bytes {start}-{end}/{currentByteData.Length}");
          }

          if (endRange.Value > currentByteData.Length)
          {

            endRange = currentByteData.Length - 1;
          }

          Byte[] newByteData = currentByteData[startRange..(endRange.Value + 1)];

          response.SetBody(newByteData);
          if (notSatifiable)
          {
            response.SetStatus(StatusCode._416);
          }
          else
          {
            response.SetStatus(StatusCode._206);
          }
        }
        catch (System.Exception e)
        {
          Console.WriteLine(e);
        }
      }
    }

    private bool IsEmpty(string value)
    {
      return value == "";
    }
  }
}