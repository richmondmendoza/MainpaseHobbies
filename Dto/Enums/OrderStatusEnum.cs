using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Enums
{
    public enum OrderStatusEnum
    {
        Pending = 0,
        Processing = 1,
        Completed,
        Cancelled,
        Refunded
    }

    public enum DeliveryStatusEnum
    {
        Pending = 0,
        Processing = 1,
        Completed = 2
    }

    public enum PaymentMethodEnum
    {
        Cash = 0,
        CreditCard = 1,
        PayPal,
        BankTransfer
    }
}
