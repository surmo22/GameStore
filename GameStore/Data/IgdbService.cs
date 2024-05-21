using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GameStore.Data
{
    public class IgdbService
    {
        private readonly HttpClient _httpClient;

        public IgdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Client-ID", "kfwdg2eg53h79yvd28bbgqbtzxqnl2");
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer f3yvdjzedzdnk44v45qrr7nmaltpti");
        }

        public async Task<JArray> GetGamesAsync(int limit = 500)
        {
            var query = $"fields name,summary,release_dates.human,involved_companies.company.name,platforms.name,cover.url,videos.video_id,screenshots.url,genres.name; limit {limit}; where category = 0 & platform = 6;"; // category = 0 filters out DLCs
            var content = new StringContent(query);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JArray.Parse(responseBody);
        }
    }
}