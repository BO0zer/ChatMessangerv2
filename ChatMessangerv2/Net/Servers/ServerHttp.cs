using ChatMessangerv2.MVVM.ViewModel;
using ChatMessangerv2.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ChatMessangerv2.Net
{
    class ServerHttp
    {
        HttpClient _httpClient;
        private Uri uri = new Uri("https://localhost:44367/api/");
        public ServerHttp()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = uri;
        }
        public async Task<HttpResponseMessage> Register(NetUser user)
        {
            string url = $"User/{user.Login}/{user.Password}"; ;
            return await _httpClient.PostAsync(url, null);
        }
        public async Task<HttpResponseMessage> Authorise(NetUser user)
        {
            string url = $"User/{user.Login}/{user.Password}";
            return await _httpClient.GetAsync(url);

        }
        public async Task<HttpResponseMessage> SearchUser(string login, int offset, int portion)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"User/{login}/{portion}/{offset}";
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetChats(int offset, int portion)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"Chat/{portion}/{offset}";
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> GetMessages(Guid id, int portion, int offset)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"Message/{id}/{portion}/{offset}";
            return await _httpClient.GetAsync(url);
        }
        public async Task<HttpResponseMessage> CreateChat(NetChat chat)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"Chat";
            return await _httpClient.PutAsJsonAsync(url, chat);
        }
        public async Task<HttpResponseMessage> NewPassword(NetUser user, string password)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"User/{password}";
            return await _httpClient.PutAsJsonAsync(url, user);
        }
        public async Task<HttpResponseMessage> NewLogin(NetUser user)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"User";
            return await _httpClient.PutAsJsonAsync(url, user);
        }
        public async Task<HttpResponseMessage> DeleteChat(Guid id)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"Chat/{id}";
            return await _httpClient.DeleteAsync(url);
        }
    }
}
