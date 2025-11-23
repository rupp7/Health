using System.Text.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HealthMaui.Utilities;

public sealed class WebRequestHandler
{
    private readonly string _baseUrl; // e.g. "http://localhost:7009"

    public WebRequestHandler(string baseUrl)
    {
        _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    private static HttpClient CreateClient() => new HttpClient();



    public async Task<T> Get<T>(string url)
    {
        using var client = CreateClient();
        var full = $"{_baseUrl}{url}";
            try
            {
                var s = await client.GetStringAsync(full).ConfigureAwait(false);
                return JsonSerializer.Deserialize<T>(s, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (Exception)
            {
                throw;
            }
    }

    public async Task<T> Post<T>(string url, object body)
    {
        using var client = CreateClient();
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var resp = await client.PostAsync($"{_baseUrl}{url}", content).ConfigureAwait(false);
        var full = $"{_baseUrl}{url}";
        try
        {
            var s = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<T>(s, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Put(string url, object body)
    {
        using var client = CreateClient();
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var full = $"{_baseUrl}{url}";
        try
        {
            using var resp = await client.PutAsync(full, content).ConfigureAwait(false);
            var s = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Delete(string url)
    {
        using var client = CreateClient();
        var full = $"{_baseUrl}{url}";
        try
        {
            using var resp = await client.DeleteAsync(full).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
