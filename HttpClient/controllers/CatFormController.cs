using System.Collections.Generic;
using HttpServer.Library;

  public class CatFormController : Controller
  {
    private Dictionary<string, string> data = new Dictionary<string, string>();
    public void Run(Request request, Response response)
    {

      Dictionary<string, string> resources = new Dictionary<string, string>();
      if (request.Method == RequestMethod.POST)
      {
        string key = request.Body.Split("=")[0];
        string value = request.Body.Split("=")[1];
        data.Add(key, value);
        response.SetStatus(StatusCode._201);
        string location = request.Url + "/" + key;
        response.SetHeader(Response.Header.Location, location);
      }

      if (request.Method == RequestMethod.GET && request.Url.Contains("data"))
      {
        if (this.data.ContainsKey("data"))
        {
          string body = this.data["data"];
          response.SetBody($"data={body}");
          response.SetStatus(StatusCode._200);
        }
        else
        {
          response.SetStatus(StatusCode._404);
        }
      }

      if (request.Method == RequestMethod.PUT && request.Url.Contains("data"))
      {
        string key = request.Body.Split("=")[0];
        string value = request.Body.Split("=")[1];
        data[key] = value;
        response.SetStatus(StatusCode._200);
      }

      if (request.Method == RequestMethod.DELETE && request.Url.Contains("data"))
      {
        data.Remove("data");
        response.SetStatus(StatusCode._200);
      }
    }
  }
