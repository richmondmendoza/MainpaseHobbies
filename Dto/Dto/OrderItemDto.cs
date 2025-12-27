using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class OrderItemDto
    {
        public int Id { get; set; } = 0;
        public int OrderId { get; set; } = 0;
        public string ProductName { get; set; } = string.Empty;
        public string SubName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        public decimal Total { get; set; } = 0;
    }
}
