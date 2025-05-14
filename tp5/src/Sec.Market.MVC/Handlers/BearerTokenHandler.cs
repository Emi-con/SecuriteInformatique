using Sec.Market.MVC.Services;
using System.Net.Http.Headers;

namespace Sec.Market.MVC.Handlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly TokenService _tokenService;
        private readonly IConfiguration _configuration;

        public BearerTokenHandler(TokenService tokenService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenService.GetTokenAsync();

            Console.WriteLine($"TOKEN USED: {token}"); // ← À garder pour debug

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var subscriptionKey = _configuration["ApiSettings:SubscriptionKey"];
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            Console.WriteLine("==== Requête sortante ====");
            Console.WriteLine($"URL: {request.RequestUri}");
            Console.WriteLine($"Authorization: {request.Headers.Authorization}");
            if (request.Headers.Contains("Ocp-Apim-Subscription-Key"))
            {
                Console.WriteLine($"Ocp-Apim-Subscription-Key: {request.Headers.GetValues("Ocp-Apim-Subscription-Key").First()}");
            }
            else
            {
                Console.WriteLine("⚠️ Subscription key NON présente dans les headers !");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
