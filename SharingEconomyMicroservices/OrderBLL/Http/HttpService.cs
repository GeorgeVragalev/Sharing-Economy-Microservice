using System.Text;

namespace OrderBLL.Http;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> Post(string url, string json)
    {
        var message = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            RequestUri = new Uri(url)
        };

        var response = await _httpClient.SendAsync(message);

        return response;
    }
}