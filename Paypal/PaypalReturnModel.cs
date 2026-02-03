using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paypal
{
    public class PaypalReturnModel
    {
        public PaypalReturnModel(string message, bool success = false, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
