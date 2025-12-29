using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repo.Services
{
    public class PayoneerTokenService
    {
        private string _clientId = "YOUR_CLIENT_ID";
        private string _clientSecret = "YOUR_CLIENT_SECRET";
        private string _payoonerURL = "https://api.sandbox.payoneer.com/v4/oauth2/token";
        private HttpClient _client;
        private HttpRequestMessage _request;

        public PayoneerTokenService()
        {
            _client = new HttpClient();
            _request = new HttpRequestMessage(HttpMethod.Post, _payoonerURL);

            var body = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials" },
                {"client_id", _clientId},
                {"client_secret", _clientSecret}
            };

            _request.Content = new FormUrlEncodedContent(body);
            
        }

        async Task<string> GetPayoneerAccessToken()
        {
            var response = await _client.SendAsync(_request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.acess_token;
        }


    }
}
