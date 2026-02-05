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
            Map(m => m.SetCode).Name("Set code").Default("").Optional();
            Map(m => m.SetName).Name("Set name").Default("").Optional();
            Map(m => m.CollectorNumber).Name("Collector number").Default("").Optional();
            Map(m => m.Foil).Name("Foil").Default("normal").Optional();
            Map(m => m.Rarity).Name("Rarity").Default("").Optional();
            Map(m => m.Quantity).Name("Quantity");
            Map(m => m.ManaBoxId).Name("ManaBox ID").Default("0").Optional();
            Map(m => m.ScryfallId).Name("Scryfall ID").Default("").Optional();
            Map(m => m.PurchasePrice).Name("Purchase price").Default(0M).Optional();
            Map(m => m.Misprint).Name("Misprint").Default("FALSE").Optional();
            Map(m => m.Altered).Name("Altered").Default("FALSE").Optional();
            Map(m => m.Condition).Name("Condition").Default("near_mint").Optional();
            Map(m => m.Language).Name("Language").Default("en").Optional();
            Map(m => m.PurchasePriceCurrency).Name("Purchase price currency").Default("PHP").Optional();
        }
    }
}