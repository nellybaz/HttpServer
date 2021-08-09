namespace HttpServer.Library
{
    public interface IMiddleware
    {
        void Run(Request request, Response response);
    }
}