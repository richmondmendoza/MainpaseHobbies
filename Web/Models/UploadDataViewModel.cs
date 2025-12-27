using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class UploadDataViewModel
    {
        public string Name { get; set; }
        public string SetCode { get; set; }
        public string SetName { get; set; }
        public string CollectorNumber { get; set; }
        public string Foil { get; set; }
        public string Rarity { get; set; }
        public int Quantity { get; set; }
        public string ManaBoxId { get; set; }
        public string ScryfallId { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool Misprint { get; set; }
        public bool Altered { get; set; }
        public string Condition { get; set; }
        public string Language { get; set; }
        public string PurchasePriceCurrency { get; set; }
    }

    public sealed class UploadDataMap : ClassMap<UploadDataViewModel>
    {
        public UploadDataMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.SetCode).Name("Set code");
            Map(m => m.SetName).Name("Set name");
            Map(m => m.CollectorNumber).Name("Collector number");
            Map(m => m.Foil).Name("Foil");
            Map(m => m.Rarity).Name("Rarity");
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.ManaBoxId).Name("ManaBox ID");
            Map(m => m.ScryfallId).Name("Scryfall ID");
            Map(m => m.PurchasePrice).Name("Purchase price").Default(0M);
            Map(m => m.Misprint).Name("Misprint");
            Map(m => m.Altered).Name("Altered");
            Map(m => m.Condition).Name("Condition");
            Map(m => m.Language).Name("Language");
            Map(m => m.PurchasePriceCurrency).Name("Purchase price currency");
        }
    }
}