using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExtremeSkins.Generator.Panel.Interfaces;

public interface IApiServerModel
{
    public Task<HttpResponseMessage> GetAsync(string route);
    public Task<HttpResponseMessage> PostAsync<T>(string route, T jsonData, JsonSerializerOptions? options = default);
    public Task<HttpResponseMessage> PutAsync<T>(string route, T jsonData, JsonSerializerOptions? options = default);
}
