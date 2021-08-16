using HttpServer.Library;

public interface Controller{
  public void Run(Request request, Response response);
}