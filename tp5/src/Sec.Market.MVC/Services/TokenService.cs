using System.Text.Json;

namespace Sec.Market.MVC.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetTokenAsync()
        {

            var clientId = _config["AzureAd:ClientId"];
            var clientSecret = _config["AzureAd:ClientSecret"];
            var tenantId = _config["AzureAd:TenantId"];
            var scope = "api://fbb85a29-8a6e-47bb-92bb-084c9afedf24/.default";

            //var scope = _config["AzureAd:ApiTokenScope"]; // ex: api://xxx/.default

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", scope),
        });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("access_token").GetString()!;
        }
    }
}
