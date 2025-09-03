using System.Text;
using System.Text.Json;

namespace GameStore.BLL.Services.ExternalServices;

public abstract class BaseHttpClient(HttpClient httpClient)
{
    protected async Task<TResult> PostAsync<TRequest, TResult>(string url, TRequest requestBody, CancellationToken cancellationToken)
    {
        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrEmpty(jsonResponse) ? default : JsonSerializer.Deserialize<TResult>(jsonResponse);
    }

    protected async Task<TResult> GetAsync<TResult>(string url, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrEmpty(jsonResponse) ? default : JsonSerializer.Deserialize<TResult>(jsonResponse);
    }
}