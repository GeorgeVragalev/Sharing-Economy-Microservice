namespace OrderBLL.Http;

public interface IHttpService
{
    Task<HttpResponseMessage> Post(string url, string json);
}