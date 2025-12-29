using Dto.Dto;
using Dto.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class CheckoutViewModel
    {
        public int Id { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } = null;
        public string OrderNumber { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        public string Address1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address cont. is required.")]
        public string Address2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail address is required.")]
        public string ContactEmail { get; set; } = string.Empty;
        public PaymentMethodEnum PaymentMethod { get; set; } = PaymentMethodEnum.Cash;
        public decimal SubTotal { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
        public decimal Shipping { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string BookAccountNumber { get; set; } = string.Empty;
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public DeliveryStatusEnum DeliveryStatus { get; set; } = DeliveryStatusEnum.Pending;
        public DeliveryMethodEnum DeliveryMethod { get; set; } = DeliveryMethodEnum.StorePickup;
        public string DeliveryNote { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        public OrderDetailsDto ToDto()
        {
            return new OrderDetailsDto()
            {
                Id = Id,
                UserId = UserId,
                DateCreated = DateCreated,
                DueDate = DueDate,
                OrderNumber = OrderNumber,
                InvoiceNumber = InvoiceNumber,
                CustomerName = CustomerName,
                Address1 = Address1,
                Address2 = Address2,
                ContactNumber = ContactNumber,
                ContactEmail = ContactEmail,
                PaymentMethod = PaymentMethod,
                SubTotal = SubTotal,
                Tax = Tax,
                Shipping = Shipping,
                Total = Total,
                BookAccountNumber = BookAccountNumber,
                Status = Status,
                DeliveryStatus = DeliveryStatus,
                Items = Items,
                DeliveryMethod = DeliveryMethod,
                DeliveryNote = DeliveryNote,
                Currency = Currency,
            };
        }
    }
}