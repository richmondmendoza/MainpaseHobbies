using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public string PayoneerId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CustomerDetailId { get; set; }
    }
}
