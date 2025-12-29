using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.User
{
    public class CustomerDetailDto
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string ShippingAddress1 { get; set; } = string.Empty;
        public string ShippingAddress2 { get; set; } = string.Empty;
        public int UserId { get; set; } = 0;
        public string Country { get; set; } = string.Empty;
        public string ShippingCountry { get; set; } = string.Empty;
        public string Postal { get; set; } = string.Empty;
        public string ShippingPostal { get; set; } = string.Empty;
    }
}
