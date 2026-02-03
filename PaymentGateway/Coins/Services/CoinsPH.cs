using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace PaymentGateway.Coins.Services
{
    public class CoinsPH
    {
        private HttpClient _client;

        public CoinsPH()
        {
            _client = new HttpClient();

            _client.DefaultRequestHeaders.Remove("X-COINS-APIKEY");
            _client.DefaultRequestHeaders.Remove("Timestamp");
            _client.DefaultRequestHeaders.Remove("Signature");

            _client.DefaultRequestHeaders.Add("X-COINS-APIKEY", CoinModel.ApiKey);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", CoinModel.ApiKey);
        }

        public CoinsCreateCheckoutResponse CreatePayment(CoinsCreateCheckoutRequest payload)
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var signature = ComputeHmacSha256(jsonPayload);

            _client.DefaultRequestHeaders.Add("Timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());
            _client.DefaultRequestHeaders.Add("Signature", signature);
            var response = _client.PostAsync(CoinModel.WebPayUrl + "/openapi/fiat/v1/checkout/create-checkout", content).Result;

            response.EnsureSuccessStatusCode();
            var resultJson = response.Content.ReadAsStringAsync().Result;

            dynamic result = JsonConvert.DeserializeObject<CoinsCreateCheckoutResponse>(resultJson);
            //var checkoutUrl = result.RootElement.GetProperty("checkoutUrl").GetString();
            //var checkoutUrl = result.data.checkoutUrl;

            return result;
        }

        public CoinsCheckoutStatusResponse GetCheckoutStatusAsync(string checkoutId = null, string requestId = null)
        {
            var path = "/openapi/fiat/v1/checkout/status-check";
            var qs = checkoutId != null ? $"checkoutId={Uri.EscapeDataString(checkoutId)}"
                                          : $"requestId={Uri.EscapeDataString(requestId)}";

            // Many GET samples sign the query string including timestamp.
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var toSign = qs; // adjust if sandbox expects "qs&timestamp={ts}" or only raw qs
            var signature = ComputeHmacSha256(toSign);
            var url = $"{CoinModel.WebPayUrl}{path}?{qs}";

            _client.DefaultRequestHeaders.Add("Timestamp", timestamp.ToString());
            _client.DefaultRequestHeaders.Add("Signature", signature);

            var response = _client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<CoinsCheckoutStatusResponse>(json);
        }


        public static string ComputeHmacSha256(string data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(CoinModel.SecretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return computedSignature;
            }
        }
    }
}
