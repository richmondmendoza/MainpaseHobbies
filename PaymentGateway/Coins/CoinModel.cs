using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Coins
{
    public class CoinModel
    {
        public static string ApiKey { get { return ConfigurationManager.AppSettings["CoinsApiKey"]; } }
        public static string SecretKey { get { return ConfigurationManager.AppSettings["CoinsApiSecret"]; } }
        public static string WebPayUrl { get { return ConfigurationManager.AppSettings["CoinsWebPayUrl"]; } }
    }

    public sealed class CoinsCreateCheckoutRequest
    {
        public string requestId { get; set; }           // 15–19 chars, your idempotent key
        public string totalAmount { get; set; }         // includes fees
        public string amount { get; set; }              // excludes fees
        public string feeAmount { get; set; }           // optional
        public string currency { get; set; } = "PHP";
        public string subMerchantId { get; set; }       // optional
        public string subMerchantReqRefNo { get; set; } // optional
        public string merchantName { get; set; }        // optional (displayed on checkout)
        public RedirectUrls redirectUrl { get; set; }
        public List<ProductDetails> productDetails { get; set; }
        public string expireSeconds { get; set; }       // 300–1800, default 1800
        public string remark { get; set; }              // optional
    }

    public sealed class RedirectUrls
    {
        public string success { get; set; }
        public string failure { get; set; }
        public string cancel { get; set; }
        public string defaultUrl { get; set; }          // required fallback
    }

    public sealed class ProductDetails
    {
        public string name { get; set; }                // required
        public string type { get; set; }                // required (e.g., retail_goods)
        public string quantity { get; set; }            // optional, default 1
        public string code { get; set; }                // optional
        public string desc { get; set; }                // optional
        public string amount { get; set; }              // required
    }

    public sealed class CoinsCreateCheckoutResponse
    {
        public int status { get; set; }
        public string error { get; set; }
        public CoinsCreateCheckoutData data { get; set; }
    }

    public sealed class CoinsCreateCheckoutData
    {
        public string requestId { get; set; }
        public string checkoutId { get; set; }
        public string checkoutUrl { get; set; }         // redirect here
        public string checkoutDeeplinkUrl { get; set; } // optional
    }

    public sealed class CoinsCheckoutStatusResponse
    {
        public int status { get; set; }
        public string error { get; set; }
        public CoinsCheckoutStatusData data { get; set; }
    }

    public sealed class CoinsCheckoutStatusData
    {
        public string checkoutId { get; set; }
        public string checkoutUrl { get; set; }
        public string requestId { get; set; }
        public string subMerchantId { get; set; }
        public string merchantName { get; set; }
        public string subMerchantReqRefNo { get; set; }
        public string totalAmount { get; set; }
        public string feeAmount { get; set; }
        public string status { get; set; }              // PENDING | SUCCEEDED | FAILED | CANCELED | EXPIRED
        public string completedAt { get; set; }         // UNIX seconds when final
        public string errorCode { get; set; }
        public string errorMsg { get; set; }
    }

}
