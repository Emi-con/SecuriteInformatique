using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class OrderServiceProxy : IOrderService
    {
        private readonly HttpClient _httpClient;

        private const string _orderApiUrl = "api/orders/";

        private readonly ITokenAcquisition _tokenAcquisition;

        public OrderServiceProxy(HttpClient httpClient, ITokenAcquisition tokenAcquisition)
        {
            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
        }
        public async Task Ajouter(OrderData orderData)
        {
            await PrepareAuthenticatedClient();

            StringContent content = new StringContent(JsonConvert.SerializeObject(orderData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_orderApiUrl, content);

            response.EnsureSuccessStatusCode();
        }

        public async Task<Order> Obtenir(int id)
        {
            await PrepareAuthenticatedClient();

            throw new NotImplementedException();
        }

        public async Task<List<Order>> ObtenirSelonUser()
        {
            await PrepareAuthenticatedClient();

            return await _httpClient.GetFromJsonAsync<List<Order>>(_orderApiUrl + "me");
            //return await _httpClient.GetFromJsonAsync<List<Order>>(_orderApiUrl + "?userId=" + userId);
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
