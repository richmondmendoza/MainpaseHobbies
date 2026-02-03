using Database.SQL;
using Dto;
using Dto.Dto;
using Dto.Enums;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Repository.Repo
{
    public class InventoryRepo
    {
        HttpClient _client;
        private decimal _attemptCount = 1.0M;

        public InventoryDetailsDto ToDetails(Inventory inventory, DbSet<Database.SQL.User> users, bool isPHPDisplay = false)
        {
            var dto = new InventoryDetailsDto(ToDto(inventory, isPHPDisplay));

            if (dto == null) return null;

            if (inventory.Inventory_Count.Any())
            {
                dto.InventoryCounts = inventory.Inventory_Count
                    .Where(ic => !ic.IsDeleted)
                    .ToList()
                    .Select(ic => new InventoryCountRepo().ToDto(ic))
                    .ToList();
            }

            if (users != null && users.Any())
            {
                var owner = users.FirstOrDefault(a => a.Id == (dto.OwnerId));
                if (owner != null)
                {
                    dto.OwnerName = $"{owner.Firstname} {owner.LastName}";
                }
            }

            return dto;
        }

        public InventoryDto ToDto(Inventory inventory, bool isPHPDisplay = false)
        {
            if (inventory == null) return null;

            return new InventoryDto
            {
                Id = inventory.Id,
                Image = inventory.Image,
                Name = inventory.Name,
                SetCode = inventory.SetCode,
                SetName = inventory.SetName,
                Collector = inventory.Collector,
                Language = inventory.Language,
                FoilType = inventory.FoilType,
                Rarity = inventory.Rarity,
                ManaboxId = inventory.ManaboxId,
                ScryfallId = inventory.ScryfallId,
                Price = inventory.Price,
                Misprint = inventory.Misprint,
                Tampered = inventory.Tampered,
                Condition = inventory.Condition,
                PurchaseCurrency = isPHPDisplay ? "PHP" : inventory.PurchaseCurrency,
                DateCreated = inventory.DateCreated,
                CreatedBy = inventory.CreatedBy,
                IsDeleted = inventory.IsDeleted,
                Color = inventory.Color,
                Description = inventory.Description,
                IllustratedBy = inventory.IllustratedBy,
                CardType = inventory.CardType,
                ManaCost = inventory.ManaCost,
                CollectionGroup = inventory.CollectionGroup,
                OwnerId = inventory.OwnerId,
                Category = inventory.Category,
                IsPhpDisplay = isPHPDisplay,
            };
        }

        public InventoryDisplayDto ToDisplayDto(Inventory inventory, bool isPHPDisplay = false)
        {
            if (inventory == null) return null;

            return new InventoryDisplayDto
            {
                Id = inventory.Id,
                Name = inventory.Name,
                FoilType = inventory.FoilType,
                Price = inventory.Price,
                Currency = !isPHPDisplay ? inventory.PurchaseCurrency : "PHP",
                ImageBase64 = inventory.Image != null ? Convert.ToBase64String(inventory.Image) : string.Empty,
                MimeType = inventory.Image != null ? inventory.Image.GetImageExtension().Replace(".", "") : "png",
                Stock = (int)inventory.Inventory_Count.Sum(ic => ic.Quantity),
                IsPhpDisplay = isPHPDisplay,
            };
        }

        public InventoryDto GetById(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
                return ToDto(record);
            }
        }

        public InventoryDetailsDto GetById(string cardId, bool isPhpDisplay)
        {
            try
            {
                var id = int.Parse(Fletcher.Decrypt(cardId));

                using (IMSEntities context = new IMSEntities())
                {
                    var record = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);
                    return ToDetails(record, context.Users, isPhpDisplay);
                }
            }
            catch
            {
                return null;
            }
        }

        public InventoryDetailsDto GetDetailsById(int id)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var record = context.Inventories.Where(i => i.Id == id && !i.IsDeleted).Include(b => b.Inventory_Count);
                return ToDetails(record.FirstOrDefault(), context.Users);
            }
        }

        public IEnumerable<InventoryDetailsDto> GetList(bool isPHPDisplay = false)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var users = context.Users;
                var records = context.Inventories.Where(i => !i.IsDeleted).Include(b => b.Inventory_Count).ToList();
                return records.Select(r => ToDetails(r, users)).ToList();
            }
        }

        public IEnumerable<InventoryDisplayDto> GetListRandom(int count = 10)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var skip = new Random().Next(0, Math.Max(0, context.Inventories.Count(i => !i.IsDeleted) - count));

                var users = context.Users;
                var records = context.Inventories
                    .Where(i => !i.IsDeleted)
                    .OrderBy(a => a.Id)
                    .Skip(skip)
                    .Take(count)
                    .Include(b => b.Inventory_Count).ToList();
                return records.Select(r => ToDisplayDto(r, true)).ToList();
            }
        }

        public IEnumerable<CardDetailsDto> GetSearchResult(string searchParam = "", string colors = "",
            string rarities = "", string foilTypes = "", string cardTypes = "", string setName = "", int take = -1)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var records = context.Inventories.Where(i => !i.IsDeleted);

                if (!string.IsNullOrEmpty(searchParam))
                {
                    records = records.Where(i => i.Name.ToLower().Contains(searchParam.ToLower()) || i.SetName.ToLower().Contains(searchParam.ToLower()) || i.SetCode.ToLower().Contains(searchParam.ToLower()));
                }

                if (!string.IsNullOrEmpty(colors))
                {
                    var colorList = colors.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => colorList.Any(c => i.Color.ToLower().Contains(c.ToLower())));
                }

                if (!string.IsNullOrEmpty(rarities))
                {
                    var rarityList = rarities.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => rarityList.Any(r => i.Rarity.ToLower().Replace(" ", "") == r.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(foilTypes))
                {
                    var foilTypeList = foilTypes.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => foilTypeList.Any(f => i.FoilType.ToLower().Replace(" ", "") == f.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(cardTypes))
                {
                    var cardTypeList = cardTypes.Split('|').Where(a => !string.IsNullOrEmpty(a)).ToList();
                    records = records.Where(i => cardTypeList.Any(f => i.CardType.ToLower().Replace(" ", "") == f.ToLower().Replace(" ", "")));
                }

                if (!string.IsNullOrEmpty(setName))
                {
                    records = records.Where(i => i.SetName.ToLower().Contains(setName.ToLower()) || i.SetCode.ToLower().Contains(setName.ToLower()));
                }

                if (take > 0)
                {
                    records = records.Take(take);
                }

                return records.Include(a => a.Inventory_Count).Select(r => new CardDetailsDto()
                {
                    Id = r.Id,
                    Image = r.Image,
                    Name = r.Name,
                    Price = r.Price,
                    CardType = r.CardType,
                    Count = r.Inventory_Count.Where(ic => !ic.IsDeleted).Sum(ic => ic.Quantity),
                    Rarity = r.Rarity,
                    FoilType = r.FoilType,
                    PurchaseCurrency = r.PurchaseCurrency,
                }).ToList();
            }
        }

        public ReturnValue Create(InventoryDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = new Inventory
                {
                    Image = dto.Image,
                    Name = dto.Name,
                    SetCode = dto.SetCode,
                    SetName = dto.SetName,
                    Collector = dto.Collector,
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity,
                    Condition = dto.Condition,
                    CreatedBy = dto.CreatedBy,
                    DateCreated = DateTime.Now,
                    IsDeleted = dto.IsDeleted,
                    ManaboxId = dto.ManaboxId,
                    Misprint = dto.Misprint,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    Price = dto.Price,
                    ScryfallId = dto.ScryfallId,
                    Tampered = dto.Tampered,
                    Color = dto.Color,
                    Description = dto.Description,
                    CardType = dto.CardType,
                    IllustratedBy = dto.IllustratedBy,
                    ManaCost = dto.ManaCost,
                    CollectionGroup = dto.CollectionGroup,
                    OwnerId = dto.OwnerId,
                    Category = dto.Category,
                };

                context.Inventories.Add(inventory);

                Db.SaveChanges(context, result, "Inventory created successfully.");
            }

            return result;
        }

        public ReturnValue CreateBulk(IEnumerable<InventoryDetailsDto> dtos)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var conversionRate = context.Conversions.FirstOrDefault(a => a.IsActive)?.Amount ?? 0;
                foreach (var dto in dtos)
                {
                    _attemptCount = _attemptCount + 0.1M;

                    switch (dto.CollectionGroup.ToLower().Replace(" ", ""))
                    {
                        case "grandarchive":
                            FetchClassDetails_GA(context, dto, conversionRate);
                            break;
                        case "magicthegathering":
                        default:
                            FetchClassDetails_MTG(context, dto, conversionRate);
                            break;
                    }
                }

                Db.SaveChanges(context, result, "Bulk inventory created successfully.");
            }

            return result;
        }

        public ReturnValue Update(InventoryDetailsDto dto)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == dto.Id && !i.IsDeleted);

                if (inventory == null)
                    return new ReturnValue("Unable to find inventory item.");

                if (dto.Image.Length > 0)
                    inventory.Image = dto.Image;

                inventory.Name = dto.Name;
                inventory.SetCode = dto.SetCode;
                inventory.SetName = dto.SetName;
                inventory.Collector = dto.Collector;
                inventory.Language = dto.Language;
                inventory.FoilType = dto.FoilType;
                inventory.Rarity = dto.Rarity;
                inventory.Condition = dto.Condition;
                inventory.ManaboxId = dto.ManaboxId;
                inventory.Misprint = dto.Misprint;
                inventory.PurchaseCurrency = dto.PurchaseCurrency;
                inventory.Price = dto.Price;
                inventory.ScryfallId = dto.ScryfallId;
                inventory.Tampered = dto.Tampered;
                inventory.Color = dto.Color;
                inventory.Description = dto.Description;
                inventory.CardType = dto.CardType;
                inventory.IllustratedBy = dto.IllustratedBy;
                inventory.ManaCost = dto.ManaCost;
                inventory.OwnerId = dto.OwnerId;
                inventory.CollectionGroup = dto.CollectionGroup;
                inventory.Category = dto.Category;

                Db.SaveChanges(context, result, "Inventory updated successfully.");
            }

            return result;
        }

        public static void UpdatePrice(int id, decimal amount)
        {
            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == id);

                if (inventory != null)
                {
                    inventory.Price = amount;
                    Db.SaveChanges(context);
                }
            }
        }

        public ReturnValue Delete(int id)
        {
            var result = new ReturnValue();

            using (IMSEntities context = new IMSEntities())
            {
                var inventory = context.Inventories.FirstOrDefault(i => i.Id == id && !i.IsDeleted);

                if (inventory == null)
                    return new ReturnValue("Unable to find inventory item.");

                inventory.IsDeleted = true;
                Db.SaveChanges(context, result, "Inventory deleted successfully.");
            }

            return result;
        }




        private async Task<Tuple<byte[], ScryfallCard>> FetchCardDetailsAsync_Scryfall(string scryfallId, string cardfaceName)
        {
            _client = new HttpClient();
            var imageBytes = new byte[0];
            var card = new ScryfallCard();
            string cardUrl = $"https://api.scryfall.com/cards/{scryfallId}";

            _client.DefaultRequestHeaders.UserAgent.ParseAdd($"MyApp{_attemptCount.ToString("n0")}/{_attemptCount}");

            var cardJson = _client.GetStringAsync(cardUrl).Result;
            card = JsonConvert.DeserializeObject<ScryfallCard>(cardJson);


            if (card.CardFaces != null & card.CardFaces.Any())
            {
                //var cardFace = card.CardFaces.Where(a => a.Name == cardfaceName).FirstOrDefault();
                var cardFace = card.CardFaces.FirstOrDefault();

                if (cardFace != null)
                {
                    if (!string.IsNullOrEmpty(cardFace.ImageUris.Png))
                        imageBytes = _client.GetByteArrayAsync(cardFace.ImageUris.Png).Result;

                    //card.OracleText = cardFace?.OracleText ?? card.OracleText;
                    //card.Artist = cardFace?.Artist ?? card.Artist;
                    //card.ManaCost = cardFace?.ManaCost ?? card.ManaCost;
                    //card.TypeLine = cardFace?.TypeLine ?? card.TypeLine;
                    //card.ColorIdentity = cardFace?.Colors ?? card.ColorIdentity;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(card.ImageUris.Png))
                    imageBytes = _client.GetByteArrayAsync(card.ImageUris.Png).Result;
            }

            _client.Dispose();
            return Tuple.Create(imageBytes, card);
        }
        private async Task<Tuple<byte[], GrandArchiveCard>> FetchCardDetailsAsync_GrandArchive(string slugName)
        {
            _client = new HttpClient();
            var imageBytes = new byte[0];
            var card = new GrandArchiveCard();
            string cardUrl = $"https://api.gatcg.com/cards/{slugName}";

            var cardJson = _client.GetStringAsync(cardUrl).Result;
            card = JsonConvert.DeserializeObject<GrandArchiveCard>(cardJson);

            if (card.editions != null & card.editions.Any())
            {
                var cardFace = card.editions.FirstOrDefault();

                if (cardFace != null)
                {
                    imageBytes = _client.GetByteArrayAsync($"https://api.gatcg.com/{cardFace.image}").Result;
                }
            }

            _client.Dispose();
            return Tuple.Create(imageBytes, card);
        }

        private string GetColorIdentityString(List<string> colorIdentity)
        {
            return string.Join("", colorIdentity.Select(a => "{" + a + "}"));
        }

        private void FetchClassDetails_MTG(IMSEntities context, InventoryDetailsDto dto, decimal conversionRate)
        {
            string faceName = dto.Name;
            if (dto.Name.Contains("//"))
            {
                faceName = dto.Name.Split(new string[] { "//" }, StringSplitOptions.None)[1].Trim();
            }

            var existing = context.Inventories.FirstOrDefault(i => i.ScryfallId == dto.ScryfallId && !i.IsDeleted);

            if (existing != null)
            {
                Tuple<byte[], ScryfallCard> cardDetails = FetchCardDetailsAsync_Scryfall(dto.ScryfallId, faceName).Result;

                if (existing.Image == null || existing.Image.Length == 0)
                    existing.Image = cardDetails.Item1;

                if (string.IsNullOrEmpty(existing.Description))
                    existing.Description = cardDetails.Item2.OracleText ?? "";

                if (string.IsNullOrEmpty(existing.Color))
                    dto.Color = GetColorIdentityString(cardDetails.Item2.ColorIdentity);

                if (string.IsNullOrEmpty(existing.ManaCost))
                    existing.ManaCost = cardDetails.Item2.ManaCost;

                if (string.IsNullOrEmpty(existing.CardType))
                    existing.CardType = cardDetails.Item2.TypeLine;

                if (string.IsNullOrEmpty(existing.IllustratedBy))
                    existing.IllustratedBy = cardDetails.Item2.Artist;

                var currentPrice = existing.Price / conversionRate;
                var isFoiled = existing.FoilType.ToLower() != "non-foil" & existing.FoilType.ToLower() != "normal";
                var scryfallPrice = Convert.ToDecimal(isFoiled ? (cardDetails.Item2.Prices?.UsdFoil ?? dto.Price.ToString()) : (cardDetails.Item2.Prices?.Usd ?? dto.Price.ToString()));
                if (currentPrice != scryfallPrice)
                    existing.Price = scryfallPrice * conversionRate;

                existing.Name = dto.Name;
                existing.SetCode = dto.SetCode;
                existing.SetName = dto.SetName;
                existing.Collector = dto.Collector;
                existing.Language = dto.Language;
                existing.FoilType = dto.FoilType;
                existing.Rarity = dto.Rarity;
                existing.ManaboxId = dto.ManaboxId;
                existing.ScryfallId = dto.ScryfallId;
                existing.Misprint = dto.Misprint;
                existing.Tampered = dto.Tampered;
                existing.Condition = dto.Condition;
                existing.PurchaseCurrency = dto.PurchaseCurrency;
                existing.Color = dto.Color;
                existing.ManaCost = dto.ManaCost;
                existing.CardType = dto.CardType;
                existing.IllustratedBy = dto.IllustratedBy;
                existing.CollectionGroup = dto.CollectionGroup;
                existing.OwnerId = dto.OwnerId;
                existing.Category = dto.Category;

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }
            }
            else
            {
                var cardDetails = FetchCardDetailsAsync_Scryfall(dto.ScryfallId, faceName).Result;

                dto.Color = GetColorIdentityString(cardDetails.Item2.ColorIdentity);

                var inventory = new Inventory
                {
                    Image = cardDetails.Item1,
                    Name = dto.Name,
                    SetCode = dto.SetCode,
                    SetName = dto.SetName,
                    Collector = dto.Collector,
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity,
                    ManaboxId = dto.ManaboxId,
                    ScryfallId = dto.ScryfallId,
                    Price = dto.Price,
                    Misprint = dto.Misprint,
                    Tampered = dto.Tampered,
                    Condition = dto.Condition,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    DateCreated = dto.DateCreated,
                    CreatedBy = dto.CreatedBy,
                    IsDeleted = dto.IsDeleted,
                    Color = dto.Color,
                    Description = cardDetails.Item2.OracleText,
                    CardType = cardDetails.Item2.TypeLine,
                    IllustratedBy = cardDetails.Item2.Artist,
                    ManaCost = cardDetails.Item2.ManaCost,
                    OwnerId = dto.OwnerId,
                    CollectionGroup = dto.CollectionGroup,
                    Category = dto.Category,
                };

                var currentPrice = inventory.Price / conversionRate;
                var isFoiled = inventory.FoilType.ToLower() != "non-foil" & inventory.FoilType.ToLower() != "normal";
                var scryfallPrice = Convert.ToDecimal(isFoiled ? (cardDetails.Item2.Prices?.UsdFoil ?? dto.Price.ToString()) : (cardDetails.Item2.Prices?.Usd ?? dto.Price.ToString()));
                if (currentPrice != scryfallPrice)
                {
                    inventory.Price = scryfallPrice * conversionRate;
                    inventory.PurchaseCurrency = "USD";
                }

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(inventory, countDto);
                    }
                }

                context.Inventories.Add(inventory);
            }
        }

        private void FetchClassDetails_GA(IMSEntities context, InventoryDetailsDto dto, decimal conversionRate)
        {
            var existing = context.Inventories.FirstOrDefault(i => i.ScryfallId == dto.ScryfallId && !i.IsDeleted);
            var cardNameAfterSlash = dto.Name.Contains("-") ? Regex.Replace(dto.Name, "-.*$", "") : dto.Name;
            var slugName = cardNameAfterSlash.ToLower().Replace(" ", "-").Replace(",", "").Replace("'", "").Replace(":", "").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss");
            Tuple<byte[], GrandArchiveCard> cardDetails = FetchCardDetailsAsync_GrandArchive(slugName).Result;

            if (existing != null)
            {
                if (existing.ScryfallId != slugName)
                {
                    existing.ScryfallId = slugName;
                }

                if (existing.Image == null || existing.Image.Length == 0)
                    existing.Image = cardDetails.Item1;

                if (string.IsNullOrEmpty(existing.Description))
                    existing.Description = cardDetails.Item2.effect ?? "";

                if (string.IsNullOrEmpty(existing.ManaCost))
                    existing.ManaCost = $"{cardDetails.Item2.cost.type}|{cardDetails.Item2.cost.value}";

                if (string.IsNullOrEmpty(existing.Color))
                    existing.Color = string.Join(", ", cardDetails.Item2.elements).TrimEnd();

                if (string.IsNullOrEmpty(existing.CardType))
                    existing.CardType = string.Join(", ", cardDetails.Item2.classes).TrimEnd();

                if (string.IsNullOrEmpty(existing.IllustratedBy))
                    existing.IllustratedBy = cardDetails.Item2.editions.FirstOrDefault()?.illustrator ?? "";

                existing.Name = dto.Name;
                existing.SetCode = dto.SetCode;
                existing.SetName = dto.SetName;
                existing.Collector = dto.Collector;
                existing.Language = dto.Language;
                existing.FoilType = dto.FoilType;
                existing.Rarity = dto.Rarity;
                existing.ManaboxId = dto.ManaboxId;
                existing.Misprint = dto.Misprint;
                existing.Tampered = dto.Tampered;
                existing.Condition = dto.Condition;
                existing.PurchaseCurrency = dto.PurchaseCurrency;
                existing.Color = dto.Color;
                existing.ManaCost = dto.ManaCost;
                existing.CardType = dto.CardType;
                existing.IllustratedBy = dto.IllustratedBy;
                existing.CollectionGroup = dto.CollectionGroup;
                existing.OwnerId = dto.OwnerId;
                existing.Category = dto.Category;
                existing.Price = dto.Price;

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(existing, countDto);
                    }
                }
            }
            else
            {
                var inventory = new Inventory
                {
                    Name = dto.Name,
                    SetCode = dto.SetCode,
                    SetName = dto.SetName,
                    Collector = dto.Collector,
                    Language = dto.Language,
                    FoilType = dto.FoilType,
                    Rarity = dto.Rarity,
                    ManaboxId = dto.ManaboxId,
                    Price = dto.Price,
                    Misprint = dto.Misprint,
                    Tampered = dto.Tampered,
                    Condition = dto.Condition,
                    PurchaseCurrency = dto.PurchaseCurrency,
                    DateCreated = dto.DateCreated,
                    CreatedBy = dto.CreatedBy,
                    IsDeleted = dto.IsDeleted,
                    Color = dto.Color,
                    OwnerId = dto.OwnerId,
                    CollectionGroup = dto.CollectionGroup,
                    Category = dto.Category,
                };

                inventory.ScryfallId = slugName;
                inventory.Image = cardDetails.Item1;
                inventory.Description = cardDetails.Item2.effect ?? "";
                inventory.ManaCost = $"{cardDetails.Item2.cost.type}|{cardDetails.Item2.cost.value}";
                inventory.Color = string.Join(", ", cardDetails.Item2.elements).TrimEnd();
                inventory.CardType = string.Join(", ", cardDetails.Item2.classes).TrimEnd();
                inventory.IllustratedBy = cardDetails.Item2.editions.FirstOrDefault()?.illustrator ?? "";

                if (dto.InventoryCounts.Any())
                {
                    foreach (var countDto in dto.InventoryCounts)
                    {
                        new InventoryCountRepo().Create(inventory, countDto);
                    }
                }

                context.Inventories.Add(inventory);
            }
        }

    }
}
