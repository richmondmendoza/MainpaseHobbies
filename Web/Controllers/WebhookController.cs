using PaymentGateway.Coins;
using PaymentGateway.Coins.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class WebhookController : Controller
    {
        private readonly string _apiSecret;

        public WebhookController()
        {
            _apiSecret = CoinModel.SecretKey;
        }

        [HttpPost]
        [Route("api/coins/webhook")]
        [ValidateInput(false)] // allow raw JSON
        public async Task<ActionResult> CoinsWebhook()
        {
            // 1) Read raw body
            string body;
            using (var sr = new StreamReader(Request.InputStream))
                body = await sr.ReadToEndAsync();

            // 2) Parse JSON to dictionary
            var doc = JsonDocument.Parse(body);
            var pairs = doc.RootElement.EnumerateObject()
                .Select(p => (p.Name, p.Value.ToString()))
                .ToList();

            // 3) Build canonical string: sort lexicographically by key, concat as {"key":"value",...} OR "key=value&..."
            // The guide says: sort params lexicographically and concatenate "in the same format".
            // We'll use key=value&key2=value2... (confirm with sandbox/Postman)
            var canonical = string.Join("&", pairs.OrderBy(p => p.Name).Select(p => $"{p.Name}={p.Item2}"));

            // 4) Compute signature
            var expectedSig = CoinsPH.ComputeHmacSha256(canonical);

            // 5) Compare with header-signature (Coins sends a signature header)
            var headerSig = Request.Headers["Signature"] ?? Request.Headers["signature"];
            if (!string.Equals(expectedSig, headerSig, StringComparison.OrdinalIgnoreCase))
            {
                // signature mismatch -> do not process
                return Json(new { received = false, errorCode = "SIG_MISMATCH" });
            }

            // 6) Process: parse status, update your order to SUCCEEDED/FAILED/CANCELED/EXPIRED
            // Checkout webhook sample fields: checkoutId, requestId, totalAmount, feeAmount, status, completedAt, errorCode, errorMsg
            // Persist and mark final state in your DB.
            // ...

            // 7) Acknowledge
            return Json(new { received = true });
        }

    }
}