using System;

namespace HttpServer.Library
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = 5000;
            new HttpServerCore().Run(port);
        }
    }
}
