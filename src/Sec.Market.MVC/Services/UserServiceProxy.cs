using Newtonsoft.Json;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Models;
using System.Text;

namespace Sec.Market.MVC.Services
{
    public class UserServiceProxy : IUserService
    {
        private readonly HttpClient _httpClient;

        private const string _userApiUrl = "api/users/";

        public UserServiceProxy(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task Ajouter(User user)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_userApiUrl, content);

            response.EnsureSuccessStatusCode();
        }

        public Task<User> Obtenir(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> Obtenir(string email, string pwd)
        {
            var parametre = new { Email = email, Password = pwd };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(parametre), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_userApiUrl + "GetUser", jsonContent);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<User>();
            else
                return null;
        }

        public async Task<List<User>> ObtenirTout()
        {
            return await _httpClient.GetFromJsonAsync<List<User>>(_userApiUrl);
        }
    }
}
