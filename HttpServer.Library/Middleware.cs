namespace HttpServer.Library
{
    public abstract class Middleware
    {
        public abstract void Run(Request request, Response response);
    }
}