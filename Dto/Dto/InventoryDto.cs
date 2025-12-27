using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto.Dto
{
    public class InventoryDto
    {
        public int Id { get; set; }
        public byte[] Image { get; set; } = new byte[0];
        public string Name { get; set; } = string.Empty;
        public string SetCode { get; set; }
        public string SetName { get; set; }
        public string Collector { get; set; }
        public string Language { get; set; }
        public string FoilType { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int ManaboxId { get; set; } = 0;
        public string ScryfallId { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public bool Misprint { get; set; } = false;
        public bool Tampered { get; set; } = false;
        public string Condition { get; set; } = string.Empty;
        public string PurchaseCurrency { get; set; } = "USD";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string Description { get; set; }
        public string Color { get; set; }
        public string ManaCost { get; set; }
        public string CardType { get; set; }
        public string IllustratedBy { get; set; }


    }

    public class InventoryDetailsDto : InventoryDto
    {
        public InventoryDetailsDto() : base() { }

        public InventoryDetailsDto(InventoryDto inventory) : base()
        {
            Id = inventory.Id;
            Image = inventory.Image;
            Name = inventory.Name;
            SetCode = inventory.SetCode;
            SetName = inventory.SetName;
            Collector = inventory.Collector;
            Language = inventory.Language;
            FoilType = inventory.FoilType;
            Rarity = inventory.Rarity;
            ManaboxId = inventory.ManaboxId;
            ScryfallId = inventory.ScryfallId;
            Price = inventory.Price;
            Misprint = inventory.Misprint;
            Tampered = inventory.Tampered;
            Condition = inventory.Condition;
            PurchaseCurrency = inventory.PurchaseCurrency;
            DateCreated = inventory.DateCreated;
            CreatedBy = inventory.CreatedBy;
            IsDeleted = inventory.IsDeleted;
            Color = inventory.Color;
            Description = inventory.Description;
            ManaCost = inventory.ManaCost;
            CardType = inventory.CardType;
            IllustratedBy = inventory.IllustratedBy;
        }

        public string ImageBase64
        {
            get
            {
                return Convert.ToBase64String(Image);
            }
        }

        public List<InventoryCountDto> InventoryCounts { get; set; } = new List<InventoryCountDto>();

        public decimal Count { get { return InventoryCounts.Sum(a => a.Quantity); } }
    }

    public class CardDetailsDto
    {
        public int Id { get; set; }
        public byte[] Image { get; set; } = new byte[0];
        public string Name { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public decimal Count { get; set; }
        public string CardType { get; set; }
        public decimal Price { get; set; } = 0;
        public string PurchaseCurrency { get; set; } = "USD";
        public string FoilType { get; set; } = string.Empty;

        public string ImageBase64 { get { return Convert.ToBase64String(Image); } }

        public string EncId { get { return Fletcher.Encrypt(Id.ToString()) ?? string.Empty; } }
    }

}
