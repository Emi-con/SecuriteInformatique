using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class ProductServiceProxy : IProductService
    {
        private readonly HttpClient _httpClient;

        private const string _produitApiUrl = "api/products/";

        private readonly ITokenAcquisition _tokenAcquisition;

        public ProductServiceProxy(HttpClient httpClient, ITokenAcquisition tokenAcquisition)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task Ajouter(Product product)
        {
            await PrepareAuthenticatedClient();

            StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_produitApiUrl, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task Modifier(Product product)
        {
            await PrepareAuthenticatedClient();
            throw new NotImplementedException();
        }

        public async Task<Product> Obtenir(int id)
        {
            await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<Product>(_produitApiUrl + id);
        }

        public async Task<List<Product>> ObtenirSelonFiltre(string? filtre)
        {
            await PrepareAuthenticatedClient();
            return await _httpClient.GetFromJsonAsync<List<Product>>(_produitApiUrl + "?filter=" + filtre);
        }

        public async Task Supprimer(int id)
        {
            await PrepareAuthenticatedClient();
            throw new NotImplementedException();
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new List<string>());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
