using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Coins
{
    public class QrModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string QrId { get; set; } = string.Empty;
        public string QrImageUrl { get; set; } = string.Empty;
        public string QrContent { get; set; } = string.Empty;
    }
}
