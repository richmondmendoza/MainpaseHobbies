using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class CartDto
    {
        public int Id { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int InventoryId { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int Quantity { get; set; } = 0;
        public string UserSessionId { get; set; }
    }

    public class CartDetailsDto : CartDto
    {
        public CartDetailsDto() : base() { }
        public CartDetailsDto(CartDto dto) : base()
        {
            this.Id = dto.Id;
            this.UserId = dto.UserId;
            this.InventoryId = dto.InventoryId;
            this.DateCreated = dto.DateCreated;
            this.Quantity = dto.Quantity;
            this.UserSessionId = dto.UserSessionId;
        }

        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Stock { get; set; } = 0;
        public string FoilType { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = new byte[0];
        public string Currency { get; set; } = string.Empty;

        public string ImageBase64 { get { return Convert.ToBase64String(ImageData); } }
        public string EncId { get { return Fletcher.Encrypt(this.Id.ToString()); } }

    }
}
