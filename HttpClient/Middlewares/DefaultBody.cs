using HttpServer.Library;

namespace HttpClient.Middlewares
{
    public class DefaultBody
    {
        public static void Run(Request request, Response response){
            response.SetBody("Client App");
        }
    }
}