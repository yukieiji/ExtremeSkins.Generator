using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ExtremeSkins.Generator.Panel.Interfaces;

namespace ExtremeSkins.Generator.Panel.Models;

public sealed class ApiServerModel : IApiServerModel
{
    private readonly HttpClient client;
    private const string URL = "http://localhost:57700/exs/";

    public ApiServerModel()
    {
        this.client = new HttpClient();
        this.client.DefaultRequestHeaders.Add("User-Agent", "ExS.Gen");
    }

    public Task<HttpResponseMessage> GetAmongUsStatusAsync()
        => this.client.GetAsync(URL);

    public Task<HttpResponseMessage> GetAsync(string route)
        => this.client.GetAsync(createUrl(route));

    public Task<HttpResponseMessage> PostAsync<T>(string route, T jsonData, JsonSerializerOptions? options = default)
    {
        var content = CreateJsonContent(jsonData, options);
        string url = createUrl(route);
        return this.client.PostAsync(url, content);
    }

    public Task<HttpResponseMessage> PutAsync<T>(string route, T jsonData, JsonSerializerOptions? options = default)
    {
        var content = CreateJsonContent(jsonData, options);
        string url = createUrl(route);
        return this.client.PutAsync(url, content);
    }

    private static StringContent CreateJsonContent<T>(T jsonData, JsonSerializerOptions? options = default)
    {
        string jsonStr = JsonSerializer.Serialize(jsonData, options);
        var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");

        return content;
    }

    private static string createUrl(string route)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(URL);
        sb.Append(route);
        return sb.ToString();
    }
}
