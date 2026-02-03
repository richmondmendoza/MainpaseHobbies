using Dto.BaseSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ConversionViewModel
    {
        public ConversionViewModel() { }

        public ConversionViewModel(ConversionDto item)
        {
            if (item != null)
            {
                Id = item.Id;
                Date = item.Date;
                Amount = item.Amount;
                IsActive = item.IsActive;
            }
        }

        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Conversion amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ConversionDto ToDto()
        {
            return new ConversionDto()
            {
                Id = this.Id,
                Date = this.Date,
                Amount = this.Amount,
                IsActive = this.IsActive
            };
        }
    }
}