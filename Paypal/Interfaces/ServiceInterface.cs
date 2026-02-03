using Paypal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paypal.Interfaces
{
    public interface IServices
    {
        Task<PaypalReturnModel> CreateOrder(string retUrl, string cancelUrl, string currency, decimal amount, string websiteName);

        Task<bool> CaptureOrder(string orderId);
    }
}
