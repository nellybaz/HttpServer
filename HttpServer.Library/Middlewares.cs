using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HttpServer.Library
{
  public class Middlewares
  {


    public static void ProcessRanges(Request request, Response response)
    {
      if (request.Range != null)
      {
        string[] rangeSplit = request.Range.Split("-");
        int startRange = Int32.Parse(rangeSplit[0]);
        int endRange = Int32.Parse(rangeSplit[1]);
        Byte[] newByteData = new Byte[startRange + endRange + 1];

        Byte[] currentByteData = response.BodyBytes;
        int newByteIndex = 0;
        for (int i = startRange; i <= endRange; i++)
        {
          newByteData[newByteIndex] = currentByteData[i];
          newByteIndex++;
        }

        response.SetBody(newByteData);
        response.SetStatus(StatusCode._206);
      }
    }
  }
}