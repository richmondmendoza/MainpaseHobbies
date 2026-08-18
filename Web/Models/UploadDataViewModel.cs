using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UploadDataViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string SetCode { get; set; } = string.Empty;
        public string SetName { get; set; } = string.Empty;
        public string CollectorNumber { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Foil { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int ManaBoxId { get; set; } = 0;
        public string ScryfallId { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PurchaseCurrency { get; set; } = "PHP";
        public bool Misprint { get; set; }
        public bool Tampered { get; set; }
        public string Condition { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ManaCost { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string IllustratedBy { get; set; } = string.Empty;
        public string CollectionGroup { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }

    public sealed class UploadDataMap : ClassMap<UploadDataViewModel>
    {
        public UploadDataMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.SetCode).Name("SetCode").Default("").Optional();
            Map(m => m.SetName).Name("SetName").Default("").Optional();
            Map(m => m.CollectorNumber).Name("Collector").Default("").Optional();
            Map(m => m.Language).Name("Language").Default("").Optional();
            Map(m => m.Foil).Name("FoilType").Default("").Optional();
            Map(m => m.Rarity).Name("Rarity").Default("").Optional();
            Map(m => m.ManaBoxId).Name("ManaBoxId").Default("0").Optional();
            Map(m => m.ScryfallId).Name("ScryfallId").Default("").Optional();
            Map(m => m.Price).Name("Price");
            Map(m => m.PurchaseCurrency).Name("Currency").Default("PHP").Optional();
            Map(m => m.Misprint).Name("Misprint").Default("").Optional();
            Map(m => m.Tampered).Name("Tampered").Default("").Optional();
            Map(m => m.Condition).Name("Condition").Default("").Optional();
            Map(m => m.Description).Name("Description").Default("").Optional();
            Map(m => m.Color).Name("Color").Default("").Optional();
            Map(m => m.ManaCost).Name("ManaCost").Default("").Optional();
            Map(m => m.CardType).Name("CardType").Default("").Optional();
            Map(m => m.IllustratedBy).Name("IllustratedBy").Default("").Optional();
            Map(m => m.CollectionGroup).Name("CollectionGroup").Default("").Optional();
            Map(m => m.Category).Name("Category").Default("").Optional();
            Map(m => m.Quantity).Name("Quantity").Default("1");
        }
    }
}