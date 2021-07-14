using System;

namespace HttpServer.Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = 5050;
            new HttpServerCore().Run(port);
        }
    }
}
