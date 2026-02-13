using Dto.Dto;
using Dto.Enums;
using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Web.Utils;
using Repository.Extensions;
using Repository.Repo.User;

namespace Web.Models
{
    public class InventoryViewModel
    {
        public InventoryViewModel() { }

        public InventoryViewModel(InventoryDetailsDto record = null)
        {
            if (record != null)
            {
                Id = record.Id;
                Image = record.Image;
                Name = record.Name;
                SetCode = record.SetCode;
                SetName = record.SetName;
                Collector = record.Collector;
                Language = record.Language;
                FoilType = record.FoilType;
                Rarity = record.Rarity;
                Condition = record.Condition;
                CreatedBy = record.CreatedBy;
                DateCreated = record.DateCreated;
                IsDeleted = record.IsDeleted;
                ManaboxId = record.ManaboxId;
                Misprint = record.Misprint;
                PurchaseCurrency = record.PurchaseCurrency;
                Price = record.Price;
                ScryfallId = record.ScryfallId;
                Tampered = record.Tampered;
                Description = record.Description;
                Color = record.Color;
                ManaCost = record.ManaCost;
                CardType = record.CardType;
                IllustratedBy = record.IllustratedBy;
                OwnerId = record.OwnerId;
                Category = record.Category;
                CollectionGroup = record.CollectionGroup;

                ImageFile = (HttpPostedFileBase)new MemoryPostedFile(this.Image);
            }
        }

        public int Id { get; set; }
        public byte[] Image { get; set; } = new byte[0];
        public HttpPostedFileBase ImageFile { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        public string SetCode { get; set; }
        public string SetName { get; set; }
        public string Collector { get; set; }
        public string Language { get; set; }

        [Required(ErrorMessage = "Foil type is required.")]
        public string FoilType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rarity is required.")]
        public string Rarity { get; set; } = string.Empty;
        public int ManaboxId { get; set; } = 0;
        public string ScryfallId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter item price.")]
        public decimal Price { get; set; } = 0;
        public bool Misprint { get; set; } = false;
        public bool Tampered { get; set; } = false;
        public string Condition { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item currency is required.")]
        public string PurchaseCurrency { get; set; } = "PHP";
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public string Description { get; set; }
        public string Color { get; set; }
        public string ManaCost { get; set; }
        public string CardType { get; set; }
        public string IllustratedBy { get; set; }
        public int OwnerId { get; set; } = 0;
        public string Category { get; set; } = string.Empty;
        public string CollectionGroup { get; set; } = string.Empty;

        public string ImageBase64
        {
            get
            {
                return Convert.ToBase64String(Image);
            }
        }

        public string ImageExtension
        {
            get
            {
                return Image.GetImageExtension().Replace(".", "");
            }
        }

        public IEnumerable<SelectListItem> FoilTypeList
        {
            get
            {
                var items = new FoilTypeRepo().GetList().Select(a => new SelectListItem() { Value = a.Name, Text = a.Name, Selected = (this.Category != null && this.Category.ToLower() == a.Name.ToLower()) });
                return items;
            }
        }

        public IEnumerable<SelectListItem> RarityList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem { Value = "Common", Text = "Common", Selected = this.Rarity.ToLower() == "common" },
                    new SelectListItem { Value = "Uncommon", Text = "Uncommon", Selected = this.Rarity.ToLower() == "uncommon" },
                    new SelectListItem { Value = "Rare", Text = "Rare", Selected = this.Rarity.ToLower() == "rare" },
                    new SelectListItem { Value = "Mythic Rare", Text = "Mythic Rare", Selected = this.Rarity.ToLower() == "mythic rare" },
                    new SelectListItem { Value = "Special", Text = "Special", Selected = this.Rarity.ToLower() == "special" }
                };

                return items;
            }
        }

        public IEnumerable<SelectListItem> ConditionList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem { Value = "Mint", Text = "Mint", Selected = this.Rarity.ToLower() == "mint" },
                    new SelectListItem { Value = "Near Mint", Text = "Near Mint", Selected = this.Rarity.ToLower() == "near mint" },
                    new SelectListItem { Value = "Lightly Played", Text = "Lightly Played", Selected = this.Rarity.ToLower() == "lightly played" },
                    new SelectListItem { Value = "Moderately Played", Text = "Moderately Played", Selected = this.Rarity.ToLower() == "moderately played" },
                    new SelectListItem { Value = "Heavily Played", Text = "Heavily Played", Selected = this.Rarity.ToLower() == "heavily played" },
                    new SelectListItem { Value = "Damaged", Text = "Damaged", Selected = this.Rarity.ToLower() == "damaged" }
                };

                return items;
            }
        }

        public IEnumerable<SelectListItem> ColorList
        {
            get
            {
                return new List<SelectListItem>
                {
                    new SelectListItem { Value = "White", Text = "White", Selected = this.Color == "White" },
                    new SelectListItem { Value = "Blue", Text = "Blue", Selected = this.Color == "Blue" },
                    new SelectListItem { Value = "Black", Text = "Black", Selected = this.Color == "Black" },
                    new SelectListItem { Value = "Red", Text = "Red", Selected = this.Color == "Red" },
                    new SelectListItem { Value = "Green", Text = "Green", Selected = this.Color == "Green" },
                    new SelectListItem { Value = "Colorless", Text = "Colorless", Selected = this.Color == "Colorless" },
                    new SelectListItem { Value = "Multicolor", Text = "Multicolor", Selected = this.Color == "Multicolor" }
                };
            }
        }

        public IEnumerable<SelectListItem> CategoryList
        {
            get
            {
                var items = new CategoryRepo().GetCategories().Select(a => new SelectListItem() { Value = a.Name, Text = a.Name, Selected = (this.Category != null && this.Category.ToLower() == a.Name.ToLower()) });
                return items;
            }
        }

        public IEnumerable<SelectListItem> CollectionGroupList
        {
            get
            {
                var items = new List<SelectListItem>()
                {
                    new SelectListItem { Value = "Magic the Gathering", Text = "Magic the Gathering", Selected = (this.CollectionGroup?.ToLower() ?? "") == "magic the gathering" },
                    new SelectListItem { Value = "Grand Archive", Text = "Grand Archive", Selected = (this.CollectionGroup?.ToLower() ?? "") == "grand archive" },
                };

                return items;
            }
        }

        public IEnumerable<SelectListItem> SetNames { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> OwnerNames
        {
            get
            {
                return new UserRepo().GetNames().Select(a => new SelectListItem() { Value = a.Item1.ToString(), Text = a.Item2 });
            }
        }

        public InventoryDetailsDto ToDto()
        {
            if (this.ImageFile != null)
            {
                using (Stream inputStream = this.ImageFile.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    this.Image = memoryStream.ToArray();
                }
            }

            return new InventoryDetailsDto
            {
                Id = this.Id,
                Image = this.Image,
                Name = this.Name,
                SetCode = this.SetCode,
                SetName = this.SetName,
                Collector = this.Collector ?? "",
                Language = this.Language ?? "",
                FoilType = this.FoilType,
                Rarity = this.Rarity,
                ManaboxId = this.ManaboxId,
                ScryfallId = this.ScryfallId,
                Price = this.Price,
                Misprint = this.Misprint,
                Tampered = this.Tampered,
                Condition = this.Condition,
                PurchaseCurrency = this.PurchaseCurrency,
                DateCreated = this.DateCreated,
                CreatedBy = this.CreatedBy,
                IsDeleted = this.IsDeleted,
                Color = this.Color,
                Description = this.Description,
                OwnerId = this.OwnerId,
                Category = this.Category,
                IllustratedBy = this.IllustratedBy,
                CardType = this.CardType,
                ManaCost = this.ManaCost,
                CollectionGroup = this.CollectionGroup,
            };
        }
    }
}