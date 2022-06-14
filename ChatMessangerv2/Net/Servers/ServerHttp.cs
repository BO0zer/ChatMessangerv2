using ChatMessangerv2.MVVM.ViewModel;
using ChatMessangerv2.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public async Task<HttpResponseMessage> GetMessages(int portion, int offset)
        {
            _httpClient.DefaultRequestHeaders.Add("Token", StartViewModel.Token);
            var url = $"Message/{portion}/{offset}";
            return await _httpClient.GetAsync(url);
        }
    }
}
