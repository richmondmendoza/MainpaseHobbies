using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class SalesDisplayDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public bool IsRefund { get; set; } = false;
    }
}
