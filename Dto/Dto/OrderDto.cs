using Dto.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class OrderDto
    {
        public int Id { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } = null;
        public string OrderNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.Cash;
        public decimal SubTotal { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
        public decimal Shipping { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string BookAccountNumber { get; set; } = string.Empty;
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public DeliveryStatusEnum DeliveryStatus { get; set; } = DeliveryStatusEnum.Pending;
    }

    public class OrderDetailsDto : OrderDto
    {
        public OrderDetailsDto() : base() { }
        public OrderDetailsDto(OrderDto dto) : base()
        {
            this.Id = dto.Id;
            this.UserId = dto.UserId;
            this.DateCreated = dto.DateCreated;
            this.DueDate = dto.DueDate;
            this.OrderNumber = dto.OrderNumber;
            this.InvoiceNumber = dto.InvoiceNumber;
            this.CustomerName = dto.CustomerName;
            this.Address1 = dto.Address1;
            this.Address2 = dto.Address2;
            this.ContactNumber = dto.ContactNumber;
            this.ContactEmail = dto.ContactEmail;
            this.PaymentMethod = dto.PaymentMethod;
            this.SubTotal = dto.SubTotal;
            this.Tax = dto.Tax;
            this.Shipping = dto.Shipping;
            this.Total = dto.Total;
            this.BookAccountNumber = dto.BookAccountNumber;
            this.Status = dto.Status;
            this.DeliveryStatus = dto.DeliveryStatus;
        }

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
