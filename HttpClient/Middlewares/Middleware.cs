using HttpServer.Library;

namespace HttpClient.Middlewares
{
    public abstract class Middleware
    {
        public abstract void Run(Request request, Response response);
    }
}